using System;
using System.Numerics;
using System.Text;
using Godot;

namespace luja.lua;

public class Stack : C
{
  public static void Push(nint L, object v)
  {
    var type = v.GetType();
    if (type.IsAssignableTo(typeof(lua_Boolean))) 
      C.lua_pushboolean(L, (lua_Boolean)Activator.CreateInstance(typeof(lua_Boolean), v));
    if (type.IsAssignableTo(typeof(lua_Integer))) 
      C.lua_pushinteger(L, (lua_Integer)Activator.CreateInstance(typeof(lua_Integer), v));
    else if (type.IsAssignableTo(typeof(lua_Number))) 
      C.lua_pushnumber(L, (lua_Number)Activator.CreateInstance(typeof(lua_Number), v));
    else if (type.IsAssignableTo(typeof(string)))
    {
      // Lua stack copies strings internally, and technically C# cannot move a string ... During a call. So ima abuse this.
      C.lua_pushstring(L, ((string)v).ToUtf8Buffer());
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  public static object GetRaw(nint L, int idx = -1)
  {
    switch ((Tenum)C.lua_type(L, idx))
    {
      case Tenum.LUA_TNIL: return null;
      case Tenum.LUA_TBOOLEAN: return C.luaL_checkinteger(L, idx) != 1;
      case Tenum.LUA_TSTRING: return new string(Encoding.UTF8.GetChars(C.luaL_checkstring(L, idx)));
      case Tenum.LUA_TNUMBER:
      {
        if (C.lua_isinteger(L, idx) != 0) return C.luaL_checkinteger(L, idx);
        else return C.luaL_checknumber(L, idx);
      }
      default: throw new NotSupportedException($"type {(Tenum)C.lua_type(L, idx)} at index {idx} is not supported!");
    }
  }

  public static T Get<T>(nint L, int idx = -1)
  {
    return (T)Activator.CreateInstance(typeof(T), GetRaw(L, idx));
  }
}