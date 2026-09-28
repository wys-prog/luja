#nullable enable

using System.Collections.Generic;
using Godot;

namespace luja.lua;

public class State: C
{
  protected lua_State state;

  public State(bool allowExit = false, bool stdlua = true)
  {
    state = C.lua2_newstate();
    if (stdlua) C.lua2_openlibs(state);
    if (! allowExit)
    {
      DoString("os = os or {} ; os.exit = function(...) print('>>> exit rejected!') end");
    }
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

  public bool DoString(string code) => C.lua2_dostring(state, code.ToUtf8Buffer()) != 0;
  public bool DoString(byte[] code) => C.lua2_dostring(state, code) != 0;

  public bool DoFile(string code) => C.lua2_dofile(state, code.ToUtf8Buffer()) != 0;
  public bool DoFile(byte[] code) => C.lua2_dofile(state, code) != 0;
}