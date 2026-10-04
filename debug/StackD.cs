using System;
using System.Runtime.InteropServices;
using luja.lua;

namespace luja.Debug;

public class StackD: C
{
  public static string Dump(lua_State L, bool ansiColors)
  {
    string buff = "";

    unsafe
    {
      C.lua2_StreamWritter writter = (p, chunksz) => 
      {
        Marshal.PtrToStringUTF8((nint)p, (int)chunksz);
        return 1;
      };

      C.lua2_dump(L, ansiColors ? 1 : 0, 4096, writter);
    }

    return buff;
  }

  public static void PrintStack(lua_State L)
  {
    C.lua2_dumpstdout(L, 1);
  }
}