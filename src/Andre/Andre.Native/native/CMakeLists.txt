cmake_minimum_required(VERSION 3.26)
project(AndreNative)

set(CMAKE_CXX_STANDARD 17)

add_library(AndreNative SHARED windows/platform_windows.cpp)

# Third party modules
add_subdirectory("third_party")

set_target_properties(AndreNative PROPERTIES PREFIX "")
target_link_libraries(AndreNative)