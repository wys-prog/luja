using System;
using System.Runtime.InteropServices;
using System.Text;
using Godot;

namespace luja.lua;

public class Stack: C
{
  public static unsafe void Push<T>(nint L, T v)
  {
    var type = v.GetType();
    if (type == typeof(bool))
      C.lua_pushboolean(L, (lua_Boolean)Activator.CreateInstance(typeof(lua_Boolean), v));
    else if (type.IsAssignableTo(typeof(lua_Integer))) 
      C.lua_pushinteger(L, (lua_Integer)Activator.CreateInstance(typeof(lua_Integer), v));
    else if (type.IsAssignableTo(typeof(lua_Number))) 
      C.lua_pushnumber(L, (lua_Number)Activator.CreateInstance(typeof(lua_Number), v));
    else if (type == typeof(string))
      // Lua stack copies strings internally, and technically C# cannot move a string ... During a call. So ima abuse this.
      C.lua_pushstring(L, (nint)C.PtrOf(v.ToString().ToUtf8Buffer()));
    else if (type == typeof(lua_CFunction))
    {
      object v2 = v;
      C.lua2_pushcfunction(L, (lua_CFunction)v2);
    }
    else
    {
      unsafe
      {
        // TODO: add __index, __newindex and __gc.
        nint datum = C.lua2_newuserdata(L, (ulong)sizeof(nint));
        Userdatum.Set(datum, v);
      }
    }
  }

  public static T Get<T>(nint L, int idx = -1) => (T)Get(L, idx, typeof(T)); 
  public static object Get(nint L, int idx = -1)
  {
    switch ((C.Tenum)C.lua_type(L, idx))
    {
      case Tenum.LUA_TBOOLEAN: return C.lua_toboolean(L, idx) != 0;
      case Tenum.LUA_TNIL: return null;
      case Tenum.LUA_TNUMBER: return C.lua2_tonumber(L, idx);
      case Tenum.LUA_TSTRING: unsafe { return Marshal.PtrToStringUTF8((nint)C.lua2_tostring(L, idx)); }
      case Tenum.LUA_TLIGHTUSERDATA: /* falls */
      case Tenum.LUA_TUSERDATA: return Userdatum.Get<object>(C.lua_touserdata(L, idx));
      case Tenum.LUA_TFUNCTION:
        {
          if (C.lua_iscfunction(L, idx) != 0)
            return Function.Retreive(C.lua_tocfunction(L, idx));
          
          return null;
        }
      default: return null;
    }
  }

  public static object Get(nint L, int idx, Type info)
  {
    if (info == typeof(object)) return Get(L, idx); /* If it is a generic argument, then pass it whatever it is on the stack! */

    if (info.IsAssignableFrom(typeof(lua_Integer))) return C.luaL_checkinteger(L, idx);
    else if (info.IsAssignableFrom(typeof(lua_Number))) return C.luaL_checknumber(L, idx);
    else if (info.IsAssignableFrom(typeof(bool))) return C.luaL_checkinteger(L, idx) != 0;
    else if (info.IsAssignableFrom(typeof(string))) return Marshal.PtrToStringUTF8(C.luaL_checkstring(L, idx));
    else if (info.IsAssignableFrom(typeof(char[]))) return Marshal.PtrToStringUTF8(C.luaL_checkstring(L, idx)).ToCharArray();
    else if (info.IsAssignableFrom(typeof(byte[]))) return C.luaL_checkstring(L, idx);
    else
    {
      unsafe
      {
        nint datum = C.luaL_checkudata(L, idx, (nint)C.PtrOf(info.FullName.ToAsciiBuffer()));
        return Userdatum.Get<object>(datum);
      }
    }
  }
}