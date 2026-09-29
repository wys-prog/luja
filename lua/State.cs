#nullable enable

using System;
using System.Collections.Generic;
using Godot;

namespace luja.lua;

public class State: C, IDisposable
{
  protected lua_State state;
  private const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

  public class TableRef
  {
    private readonly State state;
    private readonly lua_State L;

    public string[] GetKeys()
    {
      throw new NotImplementedException();
    }

    public void Push<T>(string rel, T val)
    {
      
    }

    public object Get<Hint>(string rel)
    {
      throw new NotImplementedException();
    }

    public TableRef(State s)
    {
      state = s;
      L = state.state;
    }
  }

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
      regs.Add(new luaL_Reg
      {
        func = item.Value,
        name = item.Key.ToAsciiBuffer()
      });
    }

    LoadLibrary(libname.ToAsciiBuffer(), [.. regs]);
  }

  public void LoadLibrary(byte[] libname, luaL_Reg[] regs)
  {
    C.lua2_newtable(state);
    _ = C.lua2_newlib(state, regs, regs.Length);
    C.lua_setfield(state, -1, libname);
  }

  public void Push<T>(string path, T elem)
  {
    // TableRef
  }

  // Unlike what people who doesn't know how to use Lua's C API, we DO NOT need to temporalize or pin the given managed buffer
  // in order to prevent it to be GC'ed, as the GC won't be able to move the string anywhere-else DURING the call.
  // Even if you call a C# function, this would move internal references, and so, invalidate the callee ... Which would not make
  // sense.

  public bool DoString(string code) => C.lua2_dostring(state, code.ToAsciiBuffer()) != 0;
  public bool DoString(byte[] code) => C.lua2_dostring(state, code) != 0;

  public bool DoFile(string code) => C.lua2_dofile(state, code.ToAsciiBuffer()) != 0;
  public bool DoFile(byte[] code) => C.lua2_dofile(state, code) != 0;

  public void Dispose()
  {
    C.lua_close(state);
  }
}