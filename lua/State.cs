#nullable enable

using System;
using System.Collections.Generic;
using Godot;

namespace luja.lua;

public class State : C, IDisposable
{
  protected lua_State state;
  private const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

  private int PushPath(string path)
  {
    string[] parts = path.Split('.', splitOptions);

    if (parts.Length == 0) throw new ArgumentException("Empty Lua path.", nameof(path));

    C.lua_getglobal(state, parts[0].ToAsciiBuffer());

    for (int i = 1; i < parts.Length; i++)
    {
      if ((C.Tenum)C.lua_type(state, -1) != C.Tenum.LUA_TTABLE)
      {
        C.lua_pop(state, 1);
        throw new InvalidOperationException($"Lua value '{parts[i - 1]}' is not a table.");
      }

      C.lua_getfield(state, -1, parts[i].ToAsciiBuffer());
      C.lua2_remove(state, -2);
    }

    return parts.Length;
  }

  public State(bool allowExit = false, bool stdlua = true)
  {
    state = C.lua2_newstate();
    if (stdlua) C.lua2_openlibs(state);
    if (!allowExit)
    {
      DoString("os = os or {} ; os.exit = function(...) print('>>> exit rejected!') end");
    }
  }

  public void LoadLibrary(string libname, Dictionary<string, lua_CFunction> funcs)
  {
    List<luaL_Reg> regs = [];

    foreach (var item in funcs)
    {
      regs.Add(new luaL_Reg
      {
        func = item.Value,
        name = item.Key.ToAsciiBuffer()
      });
    }

    var toPop = PushPath(libname);
    

    C.lua2_newlib(state, [.. regs], regs.Count);
    C.lua_setfield(state, -1, libname.ToAsciiBuffer());
    toPop += 1;
  }

  // Unlike what people who doesn't know how to use Lua's C API, we DO NOT need to temporalize or pin the given managed buffer
  // in order to prevent it to be GC'ed, as the GC won't be able to move the string anywhere-else DURING the call.
  // Even if you call a C# function, this would move internal references, and so, invalidate the callee ... Which would not make
  // sense.

  public bool DoString(string code) => C.lua2_dostring(state, code.ToAsciiBuffer()) == 0;
  public bool DoString(byte[] code) => C.lua2_dostring(state, code) == 0;

  public bool DoFile(string code) => C.lua2_dofile(state, code.ToAsciiBuffer()) == 0;
  public bool DoFile(byte[] code) => C.lua2_dofile(state, code) == 0;

  public void Dispose()
  {
    if (state != 0)
    {
      C.lua_close(state);
      state = 0;
    }
  }
}