using ImGuiNET;
using SoulsFormats;
using StudioCore.ParamEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StudioCore.Tests;

public static class ParamUniqueRowFinder
{
    private static int _searchID = 0;
    private static int _searchIndex = -1;

    public static void Display()
    {
        ImGui.InputInt("id", ref _searchID);
        ImGui.InputInt("index", ref _searchIndex);
        if (ImGui.Button("Search##ParamUniqueInserter"))
        {
            Find(_searchID, _searchIndex);
        }
        if (ImGui.Button("Insert unique row ID into every param"))
        {
            var baseID = ParamBank.PrimaryBank.Params.Values.Max(p => p.Rows.Max(r => r.ID)) + 1;
            var i = baseID;
            foreach (var p in ParamBank.PrimaryBank.Params.Values)
            {
                Andre.Formats.Param.Row row = new(p.Rows.First());
                row.ID = i;
                i++;
                p.AddRow(row);
            }
            TaskLogs.AddLog($"Added rows to all params with IDs {baseID}-{i-1} ",
                Microsoft.Extensions.Logging.LogLevel.Debug, TaskLogs.LogPriority.High);
        }
    }


    public static bool Find(int id, int index)
    {
        List<string> output = new();
        foreach (var p in ParamBank.PrimaryBank.Params)
        {
            for (var i = 0; i < p.Value.Rows.Count; i++)
            {
                var r = p.Value.Rows[i];
                if (r.ID == id
                    && (index == -1 || index == i))
                {
                    output.Add(p.Key);
                }
            }
        }
        if (output.Count > 0)
        {
            string message = "";
            foreach (var line in output)
            {
                message += $"{line}\n";
            }
            TaskLogs.AddLog(message,
                Microsoft.Extensions.Logging.LogLevel.Debug, TaskLogs.LogPriority.High);
        }
        else
        {
            TaskLogs.AddLog("No row IDs found",
                Microsoft.Extensions.Logging.LogLevel.Debug, TaskLogs.LogPriority.High);
        }


        return true;
    }
}
