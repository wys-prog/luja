using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using luja.lua;

namespace luja.Components;

public static class Machinery
{
  private static readonly State state = new();

  public static Assembly[] GetAssemblies()
  {
    return AppDomain.CurrentDomain.GetAssemblies();
  }

  public static Type[] GetTypes()
  {
    List<Type> types = [];
    var assemblies = GetAssemblies();
    foreach (var asm in assemblies)
      foreach (var type in asm.GetTypes()) types.Add(type);

    return [..types];
  }

  public static Type[] GetTypesDerivedOf<T>()
   => [..GetTypes().Where(type => type.IsSubclassOf(typeof(T)))];
  

  public static void Start()
  {
    var types = GetTypesDerivedOf<Component>();
    foreach (var type in types)
    {
      
    }
  }
}