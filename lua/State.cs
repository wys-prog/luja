#nullable enable

using System.Collections.Generic;
using Godot;

namespace luja.lua;

public class State: C
{
  protected lua_State state;

  public State(bool stdlua = true)
  {
    state = C.lua_newstate();
    if (stdlua) C.lua2_openlibs(state);
  }

  public void LoadLibrary(string libname, Dictionary<string, lua_CFunction> funcs)
  {
    List<luaL_Reg> regs = [];
    foreach (var item in funcs)
    {
      var reg = new luaL_Reg
      {
        func = item.Value,
        name = item.Key.ToUtf8Buffer()
      };
    }

    LoadLibrary(libname.ToUtf8Buffer(), [..regs]);
  }

  public void LoadLibrary(byte[] libname, luaL_Reg[] regs)
  {
    C.lua2_newtable(state);
    _ = C.lua2_newlib(state, regs, regs.Length);
    C.lua_setfield(state, -1, libname);
  }
}