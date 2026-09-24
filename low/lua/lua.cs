using System.Runtime.InteropServices;

namespace luja.low.lua;

using lua_State = nint;
using lua_Number = System.Double;
using lua_Integer = System.Int64;
using lua_Boolean = System.Int32;

public class C
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	protected delegate int lua_CFunction(lua_State L);

	public const string libname = "lua";

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushinteger(lua_State state, lua_Integer num);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushbooleans(lua_State state, lua_Boolean v);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushnumber(lua_State state, lua_Number v);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushnil(lua_State state);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern void lua_pushstring(lua_State state, byte[] bytes);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Integer luaL_checkinteger(lua_State state, int arg);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern lua_Number luaL_checknumber(lua_State state, int arg);

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	private static extern byte[] luaL_checklstring(lua_State state, int arg, nint psize);

	protected static byte[] luaL_checkstring(lua_State state, int arg)
	{
		return luaL_checklstring(state, arg, 0);
	}

	[DllImport(libname,  CallingConvention = CallingConvention.Cdecl)]
	protected static extern int luaL_loadstring(lua_State L, byte[] str);
}
