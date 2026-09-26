using System;
using System.Collections.Generic;

namespace luja.lua;

public class Function : C
{
  protected static lua_CFunction From(Delegate deleg)
  {
    var args = deleg.Method.GetParameters();
    
    return L =>
    {
      int pos = 1;
      List<object> callargs = [];
      foreach (var item in args)
      {
        callargs.Add(Stack.Get(L, pos++, item.ParameterType));
      }

      var ret = deleg.DynamicInvoke([..callargs]);
      if (ret != null)
      {
        Stack.Push(L, ret);
        return 1;
      }

      return 0;
    };
  }
}