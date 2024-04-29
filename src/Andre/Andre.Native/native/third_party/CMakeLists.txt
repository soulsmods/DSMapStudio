# ImGUI + ImPlot
file(GLOB IMGUI_SOURCES
        cimgui/cimgui.cpp
        cimgui/imgui/imgui.cpp
        cimgui/imgui/imgui_draw.cpp
        cimgui/imgui/imgui_demo.cpp
        cimgui/imgui/imgui_widgets.cpp
        cimgui/imgui/imgui_tables.cpp
        cimplot/cimplot.cpp
        cimplot/implot/implot.cpp
        cimplot/implot/implot_demo.cpp
        cimplot/implot/implot_items.cpp
)

target_sources(AndreNative PRIVATE ${IMGUI_SOURCES})
target_compile_definitions(AndreNative PUBLIC IMGUI_DISABLE_OBSOLETE_FUNCTIONS=1)
target_include_directories(AndreNative PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}/cimplot/implot)
target_include_directories(AndreNative PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}/cimgui/imgui)
target_include_directories(AndreNative PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}/cimgui)

# Tracy client
target_sources(AndreNative PUBLIC tracy/public/TracyClient.cpp)
target_compile_definitions(AndreNative PUBLIC TRACY_ONLY_LOCALHOST TRACY_IMPORTS TRACY_DELAYED_INIT TRACY_MANUAL_LIFETIME)
target_compile_definitions(AndreNative PRIVATE TRACY_EXPORTS)
target_include_directories(AndreNative PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}/tracy/public/tracy)

# Recast
file(GLOB RECAST_SOURCES recastnavigation/Recast/Source/*.cpp)
target_sources(AndreNative PRIVATE ${RECAST_SOURCES})
target_include_directories(AndreNative PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}/recastnavigation/Recast/Include)