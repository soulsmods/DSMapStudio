using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using FSParam;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;
using SoulsFormats;
using StudioCore;
using StudioCore.TextEditor;
using StudioCore.Editor;
using StudioCore.MsbEditor;
using ActionManager = StudioCore.Editor.ActionManager;
using AddParamsAction = StudioCore.Editor.AddParamsAction;
using CompoundAction = StudioCore.Editor.CompoundAction;
using DeleteParamsAction = StudioCore.Editor.DeleteParamsAction;
using EditorScreen = StudioCore.Editor.EditorScreen;

namespace StudioCore.ParamEditor
{
    public class MassEditScript
    {
        static List<MassEditScript> scriptList;

        string name;
        List<string> preamble;
        string[] text;
        List<string[]> args;

        MassEditScript(string path, string name)
        {
            List<string> preamble = new List<string>();
            string[] text = File.ReadAllLines(path);
            List<string[]> args = new List<string[]>();
            foreach (string line in text)
            {
                if(line.StartsWith("##") && args.Count == 0)
                    preamble.Add(line);
                else if(line.StartsWith("newvar "))
                {
                    string[] arg = line.Substring(7).Split(':', 2);
                    if (arg[1].EndsWith(';'))
                    {
                        arg[1] = arg[1].Substring(0, arg[1].Length-1);
                    }
                    args.Add(arg);
                }
                else
                    break;
            }
            this.name = name;
            this.preamble = preamble;
            this.text = text;
            this.args = args;
        }

        public static void ReloadScripts()
        {
            var dir = ParamBank.PrimaryBank.AssetLocator.GetScriptAssetsDir();
            try
            {
                if (Directory.Exists(dir))
                {
                    scriptList = Directory.GetFiles(dir).Select((x) =>
                    {
                        string name = x;
                        try
                        {
                            name = Path.GetFileNameWithoutExtension(x);
                            return new MassEditScript(x, name);
                        }
                        catch (Exception e)
                        {
                            TaskManager.warningList["MassEditScriptLoad"] = "Error loading mass edit script " + name;
                            return null;
                        }
                    }).ToList();
                }
                else
                {
                    scriptList = new List<MassEditScript>();
                }
            }
            catch
            {
                TaskManager.warningList["MassEditScriptsLoad"] = "Error loading mass edit scripts in " + dir;
                scriptList = new List<MassEditScript>();
            }
        }

        public static void EditorScreenMenuItems(ref string _currentMEditRegexInput)
        {
            foreach (MassEditScript script in scriptList)
            {
                if (script == null)
                    continue;
                if (ImGui.BeginMenu(script.name))
                {
                    script.MenuItems();
                    if (ImGui.Selectable("Load"))
                    {
                        _currentMEditRegexInput = script.GenerateMassedit();
                        EditorCommandQueue.AddCommand($@"param/menu/massEditRegex");
                    }
                    ImGui.EndMenu();
                }
            }
        }
        public void MenuItems()
        {
            foreach (string s in preamble)
                ImGui.TextUnformatted(s.Substring(2));
            ImGui.Separator();
            foreach (string[] arg in args)
            {
                ImGui.InputText(arg[0], ref arg[1], 128);
            }
        }
        public string GenerateMassedit()
        {
            string addedCommands = preamble.Count == 0 ? "" : "\n" + "clear;\nclearvars;\n";
            return string.Join('\n', preamble) + addedCommands + string.Join('\n', args.Select((x) => $@"newvar {x[0]}:{x[1]};")) + '\n' + string.Join('\n', text.Skip(args.Count + preamble.Count));         
        }
    }
}
