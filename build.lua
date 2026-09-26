local CC = os.getenv("CC") or "cc"
local CXX = os.getenv("CXX") or "c++"

local CCFLAGS = {}
local CXXFLAGS = {}
local LDFLAGS = {}

local is_windows = package.config:sub(1, 1) == "\\"
local is_macos = not is_windows and io.popen("uname -s 2>/dev/null"):read("*l") == "Darwin"

local RED = "\27[31m"
local GREEN = "\27[32m"
local YELLOW = "\27[33m"
local CYAN = "\27[36m"
local RESET = "\27[0m"

local function shellquote(s)
  s = tostring(s)

  if is_windows then
    return '"' .. s:gsub('"', '\\"') .. '"'
  end

  return "'" .. s:gsub("'", "'\\''") .. "'"
end

local function command_exists(cmd)
  local check

  if is_windows then
    check = 'where "' .. cmd .. '" >nul 2>nul'
  else
    check = 'command -v "' .. cmd .. '" >/dev/null 2>&1'
  end

  return os.execute(check) == true or os.execute(check) == 0
end

local function mkdir(path)
  local cmd

  if is_windows then
    cmd = 'if not exist "' .. path .. '" mkdir "' .. path .. '"'
  else
    cmd = 'mkdir -p ' .. shellquote(path)
  end

  return os.execute(cmd) == true or os.execute(cmd) == 0
end

local function run(cmd)
  print(CYAN .. "  " .. cmd .. RESET)

  local ok = os.execute(cmd)

  if ok ~= true and ok ~= 0 then
    io.stderr:write(RED .. "build failed" .. RESET .. "\n")
    os.exit(1)
  end
end

local function add(list, value)
  list[#list + 1] = value
end

local function parse_args()
  local i = 1

  while i <= #arg do
    local value = arg[i]

    if value == "-cc" then
      i = i + 1
      CC = arg[i] or CC

    elseif value == "-cxx" then
      i = i + 1
      CXX = arg[i] or CXX

    elseif value == "-ccflag" then
      i = i + 1
      if arg[i] then
        add(CCFLAGS, arg[i])
      end

    elseif value == "-cxxflag" then
      i = i + 1
      if arg[i] then
        add(CXXFLAGS, arg[i])
      end

    elseif value == "-ldflag" then
      i = i + 1
      if arg[i] then
        add(LDFLAGS, arg[i])
      end

    elseif value == "-h" or value == "--help" then
      print([[
usage:
  lua build.lua [options]

options:
  -cc <compiler>       C compiler
  -cxx <compiler>      C++ compiler
  -ccflag <flag>       C compiler flag
  -cxxflag <flag>      C++ compiler flag
  -ldflag <flag>       linker flag

environment:
  CC                     C compiler
  CXX                    C++ compiler

examples:
  lua build.lua
  lua build.lua -cc clang -cxx clang++
  lua build.lua -ccflag -DLUA_USE_C89
  lua build.lua -cxxflag -std=c++20
]])
      os.exit(0)

    else
      io.stderr:write(YELLOW .. "warning: unknown argument: " .. value .. RESET .. "\n")
    end

    i = i + 1
  end
end

local function join(list)
  local result = {}

  for _, value in ipairs(list) do
    result[#result + 1] = shellquote(value)
  end

  return table.concat(result, " ")
end

local function object_name(source)
  local name = source:gsub("[/\\]", "_")
  name = name:gsub("%.[^.]+$", "")
  return "bin/" .. name .. ".o"
end

local lua_source = {
  "lua/c/lapi.c",
  "lua/c/lauxlib.c",
  "lua/c/lbaselib.c",
  "lua/c/lcode.c",
  "lua/c/lcorolib.c",
  "lua/c/lctype.c",
  "lua/c/ldblib.c",
  "lua/c/ldebug.c",
  "lua/c/ldo.c",
  "lua/c/ldump.c",
  "lua/c/lfunc.c",
  "lua/c/lgc.c",
  "lua/c/linit.c",
  "lua/c/liolib.c",
  "lua/c/llex.c",
  "lua/c/lmathlib.c",
  "lua/c/lmem.c",
  "lua/c/loadlib.c",
  "lua/c/lobject.c",
  "lua/c/lopcodes.c",
  "lua/c/loslib.c",
  "lua/c/lparser.c",
  "lua/c/lstate.c",
  "lua/c/lstring.c",
  "lua/c/lstrlib.c",
  "lua/c/ltable.c",
  "lua/c/ltablib.c",
  "lua/c/ltm.c",
  "lua/c/lundump.c",
  "lua/c/lutf8lib.c",
  "lua/c/lvm.c",
  "lua/c/lzio.c",
}

local lua2_source = {
  "lua/lua2.cpp",
}

parse_args()

if not command_exists(CC) then
  io.stderr:write(RED .. "error: C compiler not found: " .. CC .. RESET .. "\n")
  os.exit(1)
end

if not command_exists(CXX) then
  io.stderr:write(RED .. "error: C++ compiler not found: " .. CXX .. RESET .. "\n")
  os.exit(1)
end

if not mkdir("bin") then
  io.stderr:write(RED .. "error: could not create bin/" .. RESET .. "\n")
  os.exit(1)
end

print(CYAN .. "CC  = " .. CC .. RESET)
print(CYAN .. "CXX = " .. CXX .. RESET)

local objects = {}

for _, source in ipairs(lua_source) do
  local object = object_name(source)

  local cmd = table.concat({
    shellquote(CC),
    join(CCFLAGS),
    "-Wall",
    "-Wextra",
    "-fPIC",
    "-c",
    shellquote(source),
    "-o",
    shellquote(object),
  }, " ")

  run(cmd)
  objects[#objects + 1] = object
end

for _, source in ipairs(lua2_source) do
  local object = object_name(source)

  local cmd = table.concat({
    shellquote(CXX),
    join(CXXFLAGS),
    "-Wall",
    "-Wextra",
    "-fPIC",
    "-c",
    shellquote(source),
    "-o",
    shellquote(object),
  }, " ")

  run(cmd)
  objects[#objects + 1] = object
end

local library

if is_windows then
  library = "bin/lua2.dll"

  local cmd = table.concat({
    shellquote(CXX),
    "-shared",
    join(LDFLAGS),
    "-o",
    shellquote(library),
    table.concat(objects, " "),
  }, " ")

  run(cmd)

elseif is_macos then
  library = "bin/liblua2.dylib"

  local cmd = table.concat({
    shellquote(CXX),
    "-dynamiclib",
    join(LDFLAGS),
    "-o",
    shellquote(library),
    table.concat(objects, " "),
  }, " ")

  run(cmd)

else
  library = "bin/liblua2.so"

  local cmd = table.concat({
    shellquote(CXX),
    "-shared",
    join(LDFLAGS),
    "-o",
    shellquote(library),
    table.concat(objects, " "),
  }, " ")

  run(cmd)
end

print(GREEN .. "built " .. library .. RESET)