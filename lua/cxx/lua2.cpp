/**
 * This file is the result of the hard works of Wys, belonging then to him.
 * You may use this file freely, as you wish, but you shall not
 * call this work as exclusively yours.
 * This file is free and open-source.
 * You may not resell this file without including it in a project,
 * or bringing it any modification.
 * Credits are not necessary, but HIGHLY appreciated. You can credit
 * me at https://github.com/wys-prog.
 */

#include <cstdint>
#include "../c/lua.hpp"
#include "lua2exp.hpp"

/**
 * LUA2 is simply a dummy C interface that internally calls Lua's C API.
 * The purpose of this is to simplify and reduce interop-languages calls
 * between C# and C-world, by exposing supposed function-like Lua C macros
 * as C-visible functions. For convention, we use lua2_<name> when the macro
 * name is lua_<name>, or luaL_<name>.
 */

#include <iostream>

extern "C" {
  LUA2 void lua2_debug_string(const char*);

  LUA2 int lua2_dostring(lua_State* L, const char* code) {
    return luaL_dostring(L, code);
  }

  LUA2 int lua2_dofile(lua_State* L, const char* path) {
    return luaL_dofile(L, path);
  }

  LUA2 void lua2_newtable(lua_State* L) {
    lua_newtable(L);
  }

  LUA2 lua_State* lua2_newstate() {
    return luaL_newstate();
  }

  LUA2 void lua2_docall(lua_State* L, int nargs, int nresults) {
    lua_call(L, nargs, nresults);
  }

  LUA2 int lua2_isboolean(lua_State* L, int idx) {
    return lua_isboolean(L, idx);
  }

  LUA2 void lua2_pushcfunction(lua_State* L, lua_CFunction func) {
    lua_pushcfunction(L, func);
  }

  LUA2 void lua2_openlibs(lua_State* L) {
    luaL_openlibs(L);
  }

  LUA2 int lua2_getmetatable(lua_State* L, const char* tname) {
    return luaL_getmetatable(L, tname);
  }

  LUA2 void lua2_newlib(lua_State* L, const luaL_Reg* funcs, int count) {
    luaL_checkversion(L);
    lua_createtable(L, 0, count);
    luaL_setfuncs(L, funcs, 0);
  }

  LUA2 void* lua2_newuserdata(lua_State* L, uint64_t len) {
    return lua_newuserdata(L, len);
  }

  LUA2 void lua2_pop(lua_State* L, int n) {
    lua_pop(L, n);
  }

  LUA2 void lua2_remove(lua_State* L, int idx) {
    lua_remove(L, idx);
  }

  LUA2 lua_Number lua2_tonumber(lua_State* L, int idx) {
    return lua_tonumber(L, idx);
  }

  LUA2 const char* lua2_tostring(lua_State* L, int idx) {
    return lua_tostring(L, idx);
  }

  LUA2 void lua2_debug_string(const char* p) {
    std::cout << "p (start):  \e[0;35m" << (uintptr_t)p << "\e[0m" << std::endl;
    std::cout << "p (string): \e[0;36m" << p << "\e[0m" << std::endl;
  }
}