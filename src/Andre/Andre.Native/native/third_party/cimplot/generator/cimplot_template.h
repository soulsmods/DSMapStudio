#ifndef CIMGUIPLOT_INCLUDED
#define CIMGUIPLOT_INCLUDED

#include "cimgui.h"

#ifdef CIMGUI_DEFINE_ENUMS_AND_STRUCTS
#include <time.h>
typedef struct tm tm;

#include "imgui_structs.h"
#else

#endif // CIMGUI_DEFINE_ENUMS_AND_STRUCTS

//ImPlotPoint getters manually wrapped use this
typedef void *(*ImPlotPoint_getter)(void* data, int idx, ImPlotPoint *point);

#include "auto_funcs.h"


#endif //CIMGUIPLOT_INCLUDED




