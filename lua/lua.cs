///
///                              Produced by Wys.
///
///   You may not use the components of this class unless you appreciate
///   suffering for hours, or, seek for a very fine and precise control over
///   Lua.
///   If that is the case, you may extend the class C in your component.
///   Good luck!
///   This work uses the Lua project: https://www.lua.org,
///   and my own bindings (Lua2, for more information, search in source files!)
///   For any documentations, its website stays the best source.
/// 

using System;
using System.Runtime.InteropServices;

namespace luja.lua;

public unsafe class C
{
	[StructLayout(LayoutKind.Sequential)]
	public struct luaL_Reg
	{
		public byte* name;
		public lua_CFunction func;
	}

	protected enum Tenum
	{
		LUA_TNIL = 0,
		LUA_TBOOLEAN,
		LUA_TLIGHTUSERDATA,
		LUA_TNUMBER,
		LUA_TSTRING,
		LUA_TTABLE,
		LUA_TFUNCTION,
		LUA_TUSERDATA,
		LUA_TTHREAD,
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int lua_CFunction(lua_State L);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]

	public delegate Int32 lua2_StreamWrite(byte* s, Int32 len);

	public const string libname = "./bin/liblua2";

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushinteger(lua_State state, lua_Integer num);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushboolean(lua_State state, lua_Boolean v);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushnumber(lua_State state, lua_Number v);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushnil(lua_State state);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushstring(lua_State state, lua_String bytes);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Integer luaL_checkinteger(lua_State state, int arg);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Number luaL_checknumber(lua_State state, int arg);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_String luaL_checklstring(lua_State state, int arg, nint psize);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_getglobal(lua_State L, byte* name);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_getfield(lua_State L, int idx, byte* k);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua2_pop(lua_State L, int n);

	protected static lua_String luaL_checkstring(lua_State state, int arg)
	{
		return luaL_checklstring(state, arg, 0);
	}

	#region lua2

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_State lua2_newstate();

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_dostring(lua_State L, lua_String str);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_dofile(lua_State L, lua_String str);
	
	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua2_docall(lua_State L, int nargs, int nresults);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_isboolean(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua2_pushcfunction(lua_State L, lua_CFunction func);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua2_newtable(lua_State L);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua2_openlibs(lua_State L);
	
	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_getmetatable(lua_State L, byte* tname);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_newlib(lua_State L, luaL_Reg* funcs, int nfuncs);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern nint lua2_newuserdata(lua_State L, System.UInt64 len);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern byte* lua2_tostring(lua_State L, int idx);

	#endregion

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern byte* lua_tolstring(lua_State L, int idx, nint psize);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int luaL_setmetatable(lua_State L, byte* tname);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int luaL_newmetatable(lua_State L, byte* tname);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_setfield(lua_State L, int idx, byte* k);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isuserdata(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern nint luaL_checkudata(lua_State L, int idx, lua_String name);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isinteger(lua_State L, int idx);
	
	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isnumber(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_iscfunction(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isstring(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_type(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_close(lua_State L);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
  protected static extern void lua2_remove(nint state, int v);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
  protected static extern void lua_pushvalue(nint state, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
  protected static extern void lua_setglobal(nint state, byte* name);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
  public static extern void lua2_debug_string(byte* name);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
  public static extern void lua2_dump(lua_State L, Int32 colors, Int32 chunksz, lua2_StreamWrite writter);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
  public static extern void lua2_dumpstdout(lua_State L, Int32 colors);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Boolean lua_toboolean(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Integer lua_tointeger(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_CFunction lua_tocfunction(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern nint lua_touserdata(lua_State L, int idx);

	[DllImport(libname, CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Number lua2_tonumber(nint l, int idx);

	public static T* PtrOf<T>(T[] array)
	{
		unsafe
		{
			fixed (T* ptr = array)
			{
				return ptr;
			}
		}
	}
}
