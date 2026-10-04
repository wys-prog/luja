#include <string>
#include <vector>
#include <iomanip>
#include <sstream>
#include <iostream>
#include "../c/lua.hpp"
#include "debug.hpp"
#include "lua2exp.hpp"

namespace lua2::debug {
  std::vector<frameinfo> dump(lua_State* L) {
    std::vector<frameinfo> list{};
    int top = lua_gettop(L);
    
    for (int i = 1; i <= top; ++i) {
      auto tostr = lua_tostring(L, i);
      list.push_back(frameinfo{
        .index = i,
        .type = luaL_typename(L, i),
        .value = tostr ? tostr : "<?>",
      });
    }

    return list;
  }

  extern "C++" {
    void dump_stack(std::ostream &ss, lua_State* L, bool colors) {
      auto dumped = dump(L);
      const unsigned long maxindx = std::to_string(dumped.back().index).size();
      const unsigned long maxtype = 14;
      const auto colidx = colors ? "\033[31m" : "";
      const auto coltyp = colors ? "\033[32m" : "";
      const auto colval = colors ? "\033[34m" : "";
      const auto reset  = colors ? "\033[0m" : "";

      for (const auto&e: dumped) {
        ss << colidx << std::setw(maxindx) << e.index << reset << ' '
        << coltyp << std::left << std::setw(maxtype) << e.type << reset
        << colval << e.value << reset << std::endl;
      }
    }
  }
}

typedef int32_t(*lua2_StreamWrite)(const char*, uint32_t);

extern "C" {
  LUA2 void lua2_dump(lua_State* L, int32_t colors, int32_t chunksz, lua2_StreamWrite writer) {
    std::string buff; /* tmp */ {
      std::stringstream ss;
      lua2::debug::dump_stack(ss, L, colors);
    }

    size_t i{0};
    while (i < buff.size()) {
      writer(buff.data() + i, chunksz);
      i += sizeof(char) * chunksz;
    }
  }

  LUA2 void lua2_dumpstdout(lua_State* L, int32_t colors) {
    lua2::debug::dump_stack(std::cout, L, colors);
  }
}