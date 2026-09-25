/// 						Produced by Wys.
///
/// 	You may not use the components of this class unless you appreciate
/// 	suffering for hours, or, seek for a very fine and precise control over
/// 	Lua.
/// 	If that is the case, you may extend the class C in your component.
/// 	Good luck!
/// 	This work uses the Lua project: https://www.lua.org,
/// 	and my own bindings (Lua2, for more information, search in source files!)
/// 	For any documentations, its website stays the best source.
/// 

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

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	private static extern int lua_getglobal(lua_State L, byte[] name);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	private static extern int lua_getfield(lua_State L, int idx, byte[] k);

	protected static lua_String luaL_checkstring(lua_State state, int arg)
	{
		return luaL_checklstring(state, arg, 0);
	}

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_dostring(lua_State L, lua_String str);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_dofile(lua_State L, lua_String str);
	
	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_docall(lua_State L, int argc);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua2_isboolean(lua_State L, int idx);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua2_pushcfunction(lua_State L, lua_CFunction func);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isuserdata(lua_State L, int idx);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern nint lua_checkmetatable(lua_State L, int idx, lua_String name);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isinteger(lua_State L, int idx);
	
	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isnumber(lua_State L, int idx);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_isstring(lua_State L, int idx);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int lua_type(lua_State L, int idx);
}
