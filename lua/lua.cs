using System.Runtime.InteropServices;

namespace luja.lua;

public class C
{
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
	protected delegate int lua_CFunction(lua_State L);

	public const string libname = "lua";

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushinteger(lua_State state, lua_Integer num);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushboolean(lua_State state, lua_Boolean v);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushnumber(lua_State state, lua_Number v);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushnil(lua_State state);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushstring(lua_State state, lua_String bytes);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Integer luaL_checkinteger(lua_State state, int arg);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Number luaL_checknumber(lua_State state, int arg);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	private static extern lua_String luaL_checklstring(lua_State state, int arg, nint psize);

	protected static lua_String luaL_checkstring(lua_State state, int arg)
	{
		return luaL_checklstring(state, arg, 0);
	}

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_dostring(lua_State L, lua_String str);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_dofile(lua_State L, lua_String str);
	
	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_docall(lua_State L, lua_String name);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_isboolean(lua_State L, int idx);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_ismeta(lua_State L, lua_String name, int idx);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isinteger(lua_State L, int idx);
	
	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isnumber(lua_State L, int idx);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isstring(lua_State L, int idx);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_type(lua_State L, int idx);
}
