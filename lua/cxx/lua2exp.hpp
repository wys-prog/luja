#ifndef LUA2EXP_H_MACRO_IDK_MIAWWWWWWW
#define LUA2EXP_H_MACRO_IDK_MIAWWWWWWW

#if defined(_WIN32) || defined(__CYGWIN__)
#  if defined(__GNUC__) || defined(__clang__)
#    define LUA2 __attribute__((dllexport))
#  else
#    define LUA2 __declspec(dllexport)
#  endif
#elif defined(__GNUC__) || defined(__clang__)
#  define LUA2 __attribute__((visibility("default")))
#else
#  define LUA2
#endif

#endif // LUA2EXP_H_MACRO_IDK_MIAWWWWWWW