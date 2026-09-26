using System;
using System.Text;
using Godot;

namespace luja.lua;

public class Stack: C
{
  private static void CreateMetatable(nint L, Type info)
  {
    C.luaL_newmetatable(L, info.FullName.ToUtf8Buffer());

    /// Retreives the 1st argument K as a string,
    /// - (Maybe?) checks for overloads and a given set of argument
    /// -> Resolve and call (OR, maybe C# has this internally)
  }

  public static void Push<T>(nint L, T v)
  {
    var type = v.GetType();
    if (type.IsAssignableTo(typeof(lua_Boolean))) 
      C.lua_pushboolean(L, (lua_Boolean)Activator.CreateInstance(typeof(lua_Boolean), v));
    else if (type.IsAssignableTo(typeof(lua_Integer))) 
      C.lua_pushinteger(L, (lua_Integer)Activator.CreateInstance(typeof(lua_Integer), v));
    else if (type.IsAssignableTo(typeof(lua_Number))) 
      C.lua_pushnumber(L, (lua_Number)Activator.CreateInstance(typeof(lua_Number), v));
    else if (type.GetType() == typeof(string))
      // Lua stack copies strings internally, and technically C# cannot move a string ... During a call. So ima abuse this.
      C.lua_pushstring(L, v.ToString().ToUtf8Buffer());
    else
    {
      unsafe
      {
        nint datum = C.lua2_newuserdata(L, (ulong)sizeof(nint));
        Userdatum.Set(datum, v);
      }
    }
  }

  public static T Get<T>(nint L, int idx = -1) => (T)Get(L, idx, typeof(T)); 

  public static object Get(nint L, int idx, Type info)
  {
    if (info.IsAssignableFrom(typeof(lua_Integer))) return C.luaL_checkinteger(L, idx);
    else if (info.IsAssignableFrom(typeof(lua_Number))) return C.luaL_checknumber(L, idx);
    else if (info.IsAssignableFrom(typeof(bool))) return C.luaL_checkinteger(L, idx) != 0;
    else if (info.IsAssignableFrom(typeof(string))) return Encoding.UTF8.GetString(C.luaL_checkstring(L, idx));
    else if (info.IsAssignableFrom(typeof(char[]))) return Encoding.UTF8.GetChars(C.luaL_checkstring(L, idx));
    else if (info.IsAssignableFrom(typeof(byte[]))) return C.luaL_checkstring(L, idx);
    else
    {
      unsafe
      {
        nint datum = C.luaL_checkudata(L, idx, info.FullName.ToUtf8Buffer());
        return Userdatum.Get<object>(datum);
      }
    }
  }
}