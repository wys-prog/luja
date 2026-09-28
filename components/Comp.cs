using System;
using System.Collections.Generic;
using luja.lua;

namespace luja.Components;

public class Component
{
  protected struct ManagedLuaBind(C.lua_CFunction function, Delegate deleg)
  {
    public C.lua_CFunction function = function;
    public Delegate deleg = deleg;

    public ManagedLuaBind() : this(null, null) {}
  }

  protected readonly Dictionary<string, ManagedLuaBind> bindings = [];

  public virtual void Init() {}
  public virtual void Update(double delta) {}
  public virtual void Leave() {}

  protected void Bind(Delegate deleg)
  {
    var name = deleg.Method.Name;
    bindings[name] = new(Function.From(deleg), deleg);
  }

  public C.lua_CFunction Index(string key)
  {
    return bindings[key].function;
  }
}
