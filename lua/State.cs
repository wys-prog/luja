#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Godot;

namespace luja.lua;

public class State : C, IDisposable
{
  protected lua_State state;
  private const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

  private void PushPath(string path, bool createMissing = false)
  {
    string[] parts = path.Split('.', splitOptions);

    if (parts.Length == 0)
      throw new ArgumentException("Empty Lua path.", nameof(path));

    C.lua_getglobal(state, parts[0].ToAsciiBuffer());

    // Create the global root if it doesn't exist.
    if (createMissing && (C.Tenum)C.lua_type(state, -1) == C.Tenum.LUA_TNIL)
    {
      C.lua2_pop(state, 1);
      C.lua2_newtable(state);
      C.lua_setglobal(state, parts[0].ToAsciiBuffer());

      C.lua_getglobal(state, parts[0].ToAsciiBuffer());
    }

    for (int i = 1; i < parts.Length; i++)
    {
      if ((C.Tenum)C.lua_type(state, -1) != C.Tenum.LUA_TTABLE)
      {
        C.lua2_pop(state, 1);
        throw new InvalidOperationException(
          $"Lua value '{parts[i - 1]}' is not a table.");
      }

      C.lua_getfield(state, -1, parts[i].ToAsciiBuffer());

      if (createMissing && (C.Tenum)C.lua_type(state, -1) == C.Tenum.LUA_TNIL)
      {
        // Remove the nil.
        C.lua2_pop(state, 1);

        // Create the missing table.
        C.lua2_newtable(state);

        // Duplicate it so one copy can be assigned to the parent
        // while the other remains as the current path value.
        C.lua_pushvalue(state, -1);

        C.lua_setfield(state, -3, parts[i].ToAsciiBuffer());
      }

      // Remove the parent table, leaving the current value.
      C.lua2_remove(state, -2);
    }
  }

  public void Push<T>(string path, T value)
  {
    var lastPoint = path.LastIndexOf('.');
    var way = path[..lastPoint];
    var name = path[(lastPoint + 1)..];
    PushPath(way, true);
    Stack.Push<T>(state, value);
    C.lua_setfield(state, -2, name.ToAsciiBuffer());
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
    List<C.luaL_Reg> regs = [];
    List<nint> allocatedNames = [];

    foreach (var item in funcs)
    {
      byte[] bytes = item.Key.ToAsciiBuffer();

      nint namePtr = Marshal.AllocHGlobal(bytes.Length);
      Marshal.Copy(bytes, 0, namePtr, bytes.Length);
      allocatedNames.Add(namePtr);

      var reg = new C.luaL_Reg
      {
        name = namePtr,
        func = item.Value
      };

      regs.Add(reg);
      GD.Print($"name: {reg.name}\nfunc: {reg.func}");
    }

    PushPath(libname);

    C.lua2_newlib(state, [.. regs], regs.Count);

    foreach (nint ptr in allocatedNames) Marshal.FreeHGlobal(ptr);

    C.lua_setfield(state, -1, libname.ToAsciiBuffer());
  }

  // Unlike what people who doesn't know how to use Lua's C API, we DO NOT need to temporalize or pin the given managed buffer
  // in order to prevent it to be GC'ed, as the GC won't be able to move the string anywhere-else DURING the call.
  // Even if you call a C# function, this would move internal references, and so, invalidate the callee ... Which would not make
  // sense.

  public bool DoString(string code) => C.lua2_dostring(state, code.ToAsciiBuffer()) == 0;
  public bool DoString(byte[] code) => C.lua2_dostring(state, code) == 0;

  public bool DoFile(string code) => C.lua2_dofile(state, code.ToAsciiBuffer()) == 0;
  public bool DoFile(byte[] code) => C.lua2_dofile(state, code) == 0;

  public string ToString(int idx)
  {
    unsafe
    {
      return Marshal.PtrToStringUTF8((nint)C.lua2_tostring(state, idx)) ?? "<null>";
    }
  }

  public void Dispose()
  {
    if (state != 0)
    {
      C.lua_close(state);
      state = 0;
    }
  }
}