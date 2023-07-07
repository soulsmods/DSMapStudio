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
using Microsoft.Extensions.Logging;

namespace StudioCore
{
    /// <summary>
    /// Used to log and display information for the user.
    /// </summary>
    public static class TaskLogs
    {

        private static readonly MapStudioLoggerProvider _provider = new();
        private static readonly List<(LogLevel, string)> _log = new();

        // Multiply text color values. Mult transitions from 0 to 1 during transition timer. 
        private static float _timerColorMult = 1.0f;
        private static LogLevel _currentEntryType = LogLevel.Information;
        private static bool _loggerWindowOpen = false;
        private static bool _scrollToEnd = false;

        private class MapStudioLogger : ILogger
        {
            private readonly string _name;

            public MapStudioLogger(string name) => _name = name;

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                _log.Add((logLevel, state.ToString()));
            }
        }

        private class MapStudioLoggerProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string name)
            {
                var logger = new MapStudioLogger(name);
                return logger;
            }

            public void Dispose() { }
        }

        /// <summary>
        /// Adds a new entry to task logger.
        /// </summary>
        /// <param name="text">Text to add to log.</param>
        /// <param name="type">Type of entry. Affects text color.</param>
        public static void AddLog(string text, LogLevel type = LogLevel.Information)
        {
            var logger = _provider.CreateLogger("");
            logger.Log(type, text);
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

        public static void Display()
        {
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            var dir = ImGuiDir.Right;
            if (_loggerWindowOpen)
                dir = ImGuiDir.Down;
            if (ImGui.ArrowButton("##ShowLogsBtn", dir))
            {
                _loggerWindowOpen = !_loggerWindowOpen;
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
                        AddLog("Log cleared", LogLevel.Information);
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
            
            var color = PickColor(null);
            var message = _log.LastOrDefault();
            if (message != default)
            {
                ImGui.TextColored(color, message.Item2);
            }
        }

        private static Vector4 PickColor(LogLevel? type)
        {
            var mult = 0.0f;
            if (type == null)
            {
                type = _currentEntryType;
                mult = _timerColorMult;
            }

            float alpha = 1.0f - 0.3f * mult;
            if (type is LogLevel.Information)
            {
                return new Vector4(
                    0.8f + 0.1f * mult,
                    1.0f - 0.1f * mult,
                    0.4f + 0.5f * mult,
                    alpha);
            }
            else if (type is LogLevel.Warning or LogLevel.Error or LogLevel.Critical)
            {
                return new Vector4(
                    1.0f - 0.1f * mult,
                    1.0f - 0.1f * mult,
                    0.3f + 0.6f * mult,
                    alpha);
            }
            else if (type is LogLevel.Error or LogLevel.Critical)
            {
                return new Vector4(
                    1.0f - 0.1f * mult,
                    0.3f + 0.6f * mult,
                    0.3f + 0.6f * mult,
                    alpha);
            }
            else
            {
                return new Vector4(
                    1.0f - 0.1f * mult,
                    1.0f - 0.1f * mult,
                    1.0f - 0.1f * mult,
                    alpha);
            }
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
