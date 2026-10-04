#pragma once

#include <string>
#include <vector>
#include <iostream>

struct lua_State;

namespace lua2::debug {
  struct frameinfo {
    int index;
    std::string type;
    std::string value;
  };

  std::vector<frameinfo> dump(lua_State* L);
}