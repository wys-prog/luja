#include "debug.hpp"
#include "../c/lua.hpp"
#include "lua2exp.hpp"
#include <iomanip>
#include <iostream>
#include <sstream>
#include <string>
#include <vector>

namespace lua2::debug {
  static std::string tostring(lua_State* L, int idx) {
    switch (lua_type(L, idx)) {
      case LUA_TNIL: return "<nil>";
      case LUA_TBOOLEAN: return lua_toboolean(L, idx) ? "<true>" : "<false>";
      case LUA_TFUNCTION: return "<function>";
      case LUA_TLIGHTUSERDATA: return "<light userdatum>";
      case LUA_TUSERDATA: return "<userdatum>";
      case LUA_TTABLE: return "<table>";
      case LUA_TTHREAD: return "<thread>";
      case LUA_TNONE: return "<none>";
      case LUA_TSTRING: return lua_tostring(L, idx);
      case LUA_TNUMBER: {
        if (lua_isinteger(L, idx)) return std::to_string(lua_tointeger(L, idx));
        return std::to_string(lua_tonumber(L, idx));
      }
      default: return "<?>";
    }
  }

  std::vector<frameinfo> dump(lua_State *L) {
    std::vector<frameinfo> list{};
    int top = lua_gettop(L);

    for (int i = 1; i <= top; ++i) {
      list.push_back(frameinfo{
          .index = i,
          .type = luaL_typename(L, i),
          .value = tostring(L, i)
      });
    }

    return list;
  }

  extern "C++" {
    void dump_stack(std::ostream &ss, lua_State *L, bool colors) {
      auto dumped = dump(L);
      const auto maxindx =
          dumped.empty() ? 1 : std::to_string(dumped.back().index).size();
      const unsigned long maxtype = 14;
      const auto colidx = colors ? "\033[31m" : "";
      const auto coltyp = colors ? "\033[32m" : "";
      const auto colval = colors ? "\033[34m" : "";
      const auto reset = colors ? "\033[0m" : "";

      for (const auto &e : dumped) {
        ss << colidx << std::setw(maxindx) << e.index << reset << ' ' << coltyp
          << std::left << std::setw(maxtype) << e.type << reset << colval
          << e.value << reset << std::endl;
      }
    }
  }
} // namespace lua2::debug

typedef int32_t (*lua2_StreamWrite)(const char *, uint32_t);

extern "C" {
  LUA2 void lua2_dump(lua_State *L, int32_t colors, uint32_t chunksz, lua2_StreamWrite writer) {
    std::stringstream ss;
    lua2::debug::dump_stack(ss, L, colors);

    const std::string buff = ss.str();

    if (!writer || chunksz == 0) {
      return;
    }

    size_t i = 0;

    while (i < buff.size()) {
      const size_t n = std::min(static_cast<size_t>(chunksz), buff.size() - i);

      writer(buff.data() + i, static_cast<uint32_t>(n));
      i += n;
    }
  }

  LUA2 void lua2_dumpstdout(lua_State *L, int32_t colors) {
    lua2::debug::dump_stack(std::cout, L, colors);
  }
}