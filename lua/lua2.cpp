/**
 * This file is recognized as a shared property of Wys.
 * You may use this file freely, as you wish, but you shall not
 * call this work as exclusively yours.
 * This file is free and open-source.
 * You may not resell this file without including it in a project,
 * or bringing it any modification.
 * Credits are not necessary, but HIGHLY appreciated. You can credit
 * me at https://github.com/wys-prog.
 */

#include "c/lua.hpp"

#if defined(_WIN32) || defined(__CYGWIN__)
#  if defined(__GNUC__) || defined(__clang__)
#    define LUA2 __attribute__((dllexport))
#  else
#    define LUA2 __declspec(dllexport)
#  endif
#elif defined(__GNUC__) || defined(__clang__)
#  define LUA2 __attribute__((visibility("default")))
#else
#  define LUA2
#endif

/**
 * LUA2 is simply a dummy C interface that internally calls Lua's C API.
 * The purpose of this is to simplify and reduce interop-languages calls
 * between C# and C-world, by exposing supposed function-like Lua C macros
 * as C-visible functions. For convention, we use lua2_<name> when the macro
 * name is lua_<name>, or luaL_<name>.
 */
extern "C" {
  LUA2 int lua2_dostring(lua_State* L, const char* code) {
    return luaL_dostring(L, code);
  }

  LUA2 int lua2_dofile(lua_State* L, const char* path) {
    return luaL_dofile(L, path);
  }

  LUA2 void lua2_newtable(lua_State* L) {
    lua_newtable(L);
  }

  LUA2 int lua2_docall(lua_State* L, int argc) {
    lua_call(L, argc, LUA_MULTRET);
  }

  LUA2 int lua2_isboolean(lua_State* L, int idx) {
    return lua_isboolean(L, idx);
  }

  LUA2 void lua2_pushcfunction(lua_State* L, lua_CFunction func) {
    lua_pushcfunction(L, func);
  }
}