#nullable enable

using System.Runtime.InteropServices;

namespace luja.lua;

public unsafe class Userdatum
{
  public static void Set<T>(nint memory, T value)
  {
    var handle = GCHandle.Alloc(value);
    *(nint*)memory = GCHandle.ToIntPtr(handle);
  }

  public static T? Get<T>(nint memory)
  {
    var token = *(nint*)memory;
    var handle = GCHandle.FromIntPtr(token);

    return (T?)handle.Target;
  }

  public static void Free(nint memory)
  {
    var token = *(nint*)memory;
    var handle = GCHandle.FromIntPtr(token);

    handle.Free();
  }
}