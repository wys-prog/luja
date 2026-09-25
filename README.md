# Luja

Luja is a game project of mine.

## (C#) luja.lua namespace

This namespace uses the Lua C source code, and Lua2 (my own implementation) C++ project. Just so you know, Lua2 is simply a set of convinience bindings to Lua, exported in a C public interface. In order to build luja.lua, you simply need a C++ and C compiler, and compile all .c and .cpp files to .o files, and then link them against each-other into a shared library named "lua". However, feel free to compile it however you wish... as long as it works!