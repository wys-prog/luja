#nullable enable

using System;
using System.Collections.Generic;
using Godot;

namespace luja.lua;

public class Function: C
{
  private static readonly Dictionary<lua_CFunction, Delegate> delegations = [];

  public static lua_CFunction From(Delegate deleg)
  {
    var args = deleg.Method.GetParameters();

    int cfun(nint L)
    {

      int pos = 1;
      var callargs = new object[args.Length];
      foreach (var item in args)
      {
        callargs[pos-1] = Stack.Get(L, pos++, item.ParameterType);
      }
      
      var ret = deleg.DynamicInvoke([.. callargs]);
      if (ret != null)
      {
        Stack.Push(L, ret);
        return 1;
      }

      return 0;
    }

    delegations[cfun] = deleg;

    return cfun;
  }

  public static object[] SimulateCall(lua_CFunction cfun, params object?[]? args)
  {
    unsafe
    {
      lua_State L = C.lua2_newstate();
      if (args != null) foreach (var arg in args) Stack.Push(L, arg);
      
      int nresult = cfun(L);
      object[] result = new object[nresult];
      GD.Print(nresult);
      for (int i = 0; i < nresult; i++) result[i] = Stack.Get(L, -nresult + i);
      C.lua_close(L);

      return result;
    }
  }

  public static object[] SimulateCall(Delegate fun, params object?[]? args) => SimulateCall(From(fun), args);
  public static object[] SimulateCall(Delegate fun, Action<Exception> SEH, params object?[]? args) 
   => SimulateCall(From(fun), SEH, args);
  
  public static object[] SimulateCall(lua_CFunction cfun, Action<Exception> SEH, params object?[]? args)
  {
    try
    {
      return SimulateCall(cfun, args);
    }
    catch (Exception e)
    {
      SEH(e);
      return [];
    }    
  }

  public static Delegate? Retreive(lua_CFunction cfun)
  {
    if (delegations.TryGetValue(cfun, out Delegate? value)) return value;
    return null;
  }
}