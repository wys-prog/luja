using System;
using System.Runtime.InteropServices;
using Godot;

namespace luja.lua;

public class Stack(State __state) : C
{
  protected readonly State state = __state;

  public void Push(object v) => Push(state.Native(), v);
  public object Get(int idx = -1) => Get(state.Native(), idx);
  public T Get<T>(int idx = -1) => Get<T>(state.Native(), idx);

  public static void Push(nint L, int value) => C.lua_pushinteger(L, value);
  public static void Push(nint L, long value) => C.lua_pushinteger(L, value);
  public static void Push(nint L, uint value) => C.lua_pushinteger(L, value);
  public static void Push(nint L, ulong value) => C.lua_pushinteger(L, checked((long)value));
  public static void Push(nint L, float value) => C.lua_pushnumber(L, value);
  public static void Push(nint L, double value) => C.lua_pushnumber(L, value);
  public static void Push(nint L, bool value) => C.lua_pushboolean(L, value ? 1 : 0);

  public unsafe static void Push(nint L, string value)
  {
    byte[] utf8 = value.ToUtf8Buffer();
    C.lua_pushstring(L, (nint)C.PtrOf(utf8));
  }

  public static void Push(nint L, Delegate del)
  {
    C.lua2_pushcfunction(L, Function.From(del));
  }

  public static void Push(nint L, lua_CFunction del)
  {
    C.lua2_pushcfunction(L, del);
  }

  public unsafe static void Push(nint L, object value)
  {
    switch (value)
    {
      case null: C.lua_pushnil(L); break;
      case bool x: Push(L, x); break;
      case int x: Push(L, x); break;
      case uint x: Push(L, x); break;
      case long x: Push(L, x); break;
      case ulong x: Push(L, x); break;
      case float x: Push(L, x); break;
      case double x: Push(L, x); break;
      case string x: Push(L, x); break;
      case lua_CFunction x: Push(L, x); break;
      case Delegate x: Push(L, x); break;
      default:
        nint datum = C.lua2_newuserdata(L, (ulong)sizeof(nint));
        Userdatum.Set(datum, value);
        break;
    }
  }

  public static object Get(nint L, int idx = -1)
  {
    switch ((Tenum)lua_type(L, idx))
    {
      case Tenum.LUA_TBOOLEAN: return luaL_checkinteger(L, idx) == 0;
      case Tenum.LUA_TFUNCTION: return Function.Retreive(lua_tocfunction(L, idx));
      case Tenum.LUA_TNIL: return null;
      case Tenum.LUA_TUSERDATA:
      case Tenum.LUA_TLIGHTUSERDATA:
        return Userdatum.Get<object>(lua_touserdata(L, idx));
      case Tenum.LUA_TNUMBER:
        {
          if (lua_isinteger(L, idx) != 0) return luaL_checkinteger(L, idx);
          return luaL_checknumber(L, idx);
        }
      case Tenum.LUA_TSTRING: return Marshal.PtrToStringUTF8(luaL_checkstring(L, idx));
      default: return null;
    }
  }

  public static T Get<T>(nint L, int idx = -1)
  {
    var type = typeof(T);
    if (type == typeof(bool))
      return (T)(object)(C.lua_toboolean(L, idx) != 0);

    if (type == typeof(sbyte))
      return (T)(object)checked((sbyte)C.luaL_checkinteger(L, idx));

    if (type == typeof(byte))
      return (T)(object)checked((byte)C.luaL_checkinteger(L, idx));

    if (type == typeof(short))
      return (T)(object)checked((short)C.luaL_checkinteger(L, idx));

    if (type == typeof(ushort))
      return (T)(object)checked((ushort)C.luaL_checkinteger(L, idx));

    if (type == typeof(int))
      return (T)(object)checked((int)C.luaL_checkinteger(L, idx));

    if (type == typeof(uint))
      return (T)(object)checked((uint)C.luaL_checkinteger(L, idx));

    if (type == typeof(long))
      return (T)(object)checked(C.luaL_checkinteger(L, idx));

    if (type == typeof(ulong))
      return (T)(object)checked((ulong)C.luaL_checkinteger(L, idx));

    if (type == typeof(float))
      return (T)(object)(float)C.luaL_checknumber(L, idx);

    if (type == typeof(double))
      return (T)(object)C.luaL_checknumber(L, idx);

    if (type == typeof(string))
      return (T)(object)Marshal.PtrToStringUTF8(
          C.luaL_checkstring(L, idx))!;

    if (type == typeof(char[]))
      return (T)(object)Marshal.PtrToStringUTF8(
          C.luaL_checkstring(L, idx))!.ToCharArray();

    if (type == typeof(byte[]))
      return (T)(object)C.luaL_checkstring(L, idx);

    return GetUserdata<T>(L, idx);
  }

  public static object Get(nint L, int idx, Type type)
  {
    if (type == typeof(object)) return Get(L, idx);

    if (type == typeof(bool))
      return C.lua_toboolean(L, idx) != 0;

    if (type == typeof(sbyte))
      return checked((sbyte)C.luaL_checkinteger(L, idx));

    if (type == typeof(byte))
      return checked((byte)C.luaL_checkinteger(L, idx));

    if (type == typeof(short))
      return checked((short)C.luaL_checkinteger(L, idx));

    if (type == typeof(ushort))
      return checked((ushort)C.luaL_checkinteger(L, idx));

    if (type == typeof(int))
      return checked((int)C.luaL_checkinteger(L, idx));

    if (type == typeof(uint))
      return checked((uint)C.luaL_checkinteger(L, idx));

    if (type == typeof(long))
      return C.luaL_checkinteger(L, idx);

    if (type == typeof(ulong))
      return checked((ulong)C.luaL_checkinteger(L, idx));

    if (type == typeof(float))
      return (float)C.luaL_checknumber(L, idx);

    if (type == typeof(double))
      return C.luaL_checknumber(L, idx);

    if (type == typeof(string))
      return Marshal.PtrToStringUTF8(
          C.luaL_checkstring(L, idx))!;

    if (type == typeof(char[]))
      return Marshal.PtrToStringUTF8(
          C.luaL_checkstring(L, idx))!.ToCharArray();

    if (type == typeof(byte[]))
      return C.luaL_checkstring(L, idx);

    return GetUserdata<object>(L, idx);
  }

  private static T GetUserdata<T>(nint L, int idx)
  {
    unsafe
    {
      nint datum = C.luaL_checkudata(
          L,
          idx,
          (nint)C.PtrOf(typeof(T).FullName!.ToAsciiBuffer()));

      return Userdatum.Get<T>(datum);
    }
  }


}