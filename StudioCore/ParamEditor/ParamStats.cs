using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Numerics;
using ImGuiNET;
using SoulsFormats;

namespace StudioCore.ParamEditor
{
    public class ParamStats
    {
        public static void ShowValueDistribution(List<PARAM.Row> rows, PARAMDEF.Field field)
        {
            List<(object, int)> distribution = GetValueDistribution(rows, field);
            //sort
            //imgui print
        }

        public static object GetAverageValue(List<PARAM.Row> rows, PARAMDEF.Field field)
        {
            return null;
        }
        
        public static List<(object, int)> GetValueDistribution(List<PARAM.Row> rows, PARAMDEF.Field field)
        {
            return null;
        }
    }
}