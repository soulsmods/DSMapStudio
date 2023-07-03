using ImGuiNET;
using SoapstoneLib;
using StudioCore.Editor;
using StudioCore.Scene;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Veldrid;
using System.Threading;
using System.Threading.Tasks;

namespace StudioCore
{
    /// <summary>
    /// Used to log and display information for the user.
    /// </summary>
    public static class TaskLogger
    {
        // Multiply text color values. Mult transitions from 0 to 1 during transition timer. 
        private static float _timerColorMult = 1.0f;

        private static readonly List<(EntryType, string)> _log = new();
        private static readonly object _lockObj = new();
        private static EntryType _currentEntryType = EntryType.Success;
        private static bool _loggerWindowOpen = false;
        private static bool _scrollToEnd = false;

        /// <summary>
        /// Entry types for log.
        /// </summary>
        public enum EntryType
        { 
            Transitional = -1,
            Error = 0,
            Success = 1,
            Neutral = 2,
        }

        /// <summary>
        /// Adds a new entry to task logger.
        /// </summary>
        /// <param name="text">Text to add to log.</param>
        /// <param name="type">Type of entry. Affects text color.</param>
        public static void AddLog(string text, EntryType type = EntryType.Success)
        {
            lock (_lockObj)
            {
                _log.Add((type, text));
                _currentEntryType = type;
                _scrollToEnd = true;

                // Run color timer or reset mult if it's already running.
                if (_timerColorMult == 1.0f)
                {
                    Task.Run(ColorTimer);
                }
                else
                {
                    _timerColorMult = 0.0f;
                }
            }
        }

        public static void Display()
        {
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.ArrowButton("##ShowLogsBtn", ImGuiDir.Down))
            {
                _loggerWindowOpen = true;
            }

            if (_loggerWindowOpen)
            {
                ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0f, 0f, 0f, 0.98f));
                ImGui.PushStyleColor(ImGuiCol.TitleBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new Vector4(0.25f, 0.25f, 0.25f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.1f, 0.1f, 0.1f, 0.98f));
                if (ImGui.Begin("Logger##TaskLogger", ref _loggerWindowOpen, ImGuiWindowFlags.NoDocking))
                {
                    if (ImGui.Button("Clear##TaskLogger"))
                    {
                        _log.Clear();
                    }

                    ImGui.BeginChild("##LogItems");
                    ImGui.Spacing();
                    for (var i = 0; i < _log.Count; i++)
                    {
                        ImGui.TextColored(PickColor(_log[i].Item1), " " + _log[i].Item2);
                    }
                    if (_scrollToEnd)
                    {
                        ImGui.SetScrollHereY();
                        _scrollToEnd = false;
                    }
                    ImGui.Spacing();
                    ImGui.EndChild();
                }
                ImGui.End();
                ImGui.PopStyleColor(4);
            }

            if (!_log.Any())
            {
                _log.Add((EntryType.Neutral, "Log cleared"));
            }

            var color = PickColor(EntryType.Transitional);
            ImGui.TextColored(color, _log.Last().Item2);
        }

        private static Vector4 PickColor(EntryType type)
        {
            var mult = 0.0f;
            if (type == EntryType.Transitional)
            {
                type = _currentEntryType;
                mult = _timerColorMult;
            }

            float alpha = 1.0f - 0.3f * mult;
            if (type == EntryType.Success)
            {
                return new Vector4(
                    0.8f + 0.1f * mult,
                    1.0f - 0.1f * mult,
                    0.4f + 0.5f * mult,
                    alpha);
            }
            else if (type == EntryType.Error)
            {
                return new Vector4(
                    1.0f - 0.1f * mult,
                    0.3f + 0.6f * mult,
                    0.3f + 0.6f * mult,
                    alpha);
            }
            else if (type == EntryType.Neutral)
            {
                return new Vector4(
                    1.0f - 0.1f * mult,
                    1.0f - 0.1f * mult,
                    1.0f - 0.1f * mult,
                    alpha);
            }
            throw new NotImplementedException("Unsupported task logger LogType");
        }

        private static void ColorTimer()
        {
            // Time for task text color to transition completely (in miliseconds)
            const float transitionTime = 1000.0f;
            // Time for task text color to start transitioning (in miliseconds)
            const int transitionDelay = 4000;

            _timerColorMult = 0.0f;
            float prevMult = -1.0f;
            while (_timerColorMult < 1.0f)
            {
                if (_timerColorMult != prevMult)
                {
                    // Mult was just changed, sleep for initial delay.
                    Thread.Sleep(transitionDelay);
                }
                _timerColorMult += 1.0f / transitionTime;
                prevMult = _timerColorMult;
                Thread.Sleep(1);
            }
            _timerColorMult = 1.0f;
        }
    }
}
