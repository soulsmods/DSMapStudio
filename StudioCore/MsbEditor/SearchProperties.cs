using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace StudioCore.MsbEditor;

public class SearchProperties
{
    private readonly Dictionary<string, List<WeakReference<Entity>>> FoundObjects = new();
    private readonly Universe Universe;
    public string PropertyName = "";
    private Type PropertyType;

    private dynamic PropertyValue;
    private bool ValidType;

    public SearchProperties(Universe universe)
    {
        Universe = universe;
    }

    public bool InitializeSearchValue(string initialValue = null)
    {
        if (PropertyType != null)
        {
            if (PropertyType == typeof(bool) || PropertyType == typeof(bool[]))
            {
                PropertyValue = bool.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(byte) || PropertyType == typeof(byte[]))
            {
                PropertyValue = byte.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(char) || PropertyType == typeof(char[]))
            {
                PropertyValue = char.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(short) || PropertyType == typeof(short[]))
            {
                PropertyValue = short.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(ushort) || PropertyType == typeof(ushort[]))
            {
                PropertyValue = ushort.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(int) || PropertyType == typeof(int[]))
            {
                PropertyValue = int.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(uint) || PropertyType == typeof(uint[]))
            {
                PropertyValue = uint.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(long) || PropertyType == typeof(long[]))
            {
                PropertyValue = long.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(ulong) || PropertyType == typeof(ulong[]))
            {
                PropertyValue = ulong.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(float) || PropertyType == typeof(float[]))
            {
                PropertyValue = float.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(double) || PropertyType == typeof(double[]))
            {
                PropertyValue = double.TryParse(initialValue, out var val) ? val : default;
                return true;
            }

            if (PropertyType == typeof(string) || PropertyType == typeof(string[]))
            {
                PropertyValue = initialValue ?? "";
                return true;
            }
        }

        return false;
    }

    public bool SearchValue(bool searchFieldchanged)
    {
        ImGui.Text("Value (Exact)");
        ImGui.NextColumn();
        var ret = false;
        if (PropertyType == typeof(bool) || PropertyType == typeof(bool[]))
        {
            var val = (bool)PropertyValue;
            if (ImGui.Checkbox("##valBool", ref val) || searchFieldchanged)
            {
                PropertyValue = val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(byte) || PropertyType == typeof(byte[]))
        {
            var val = (int)PropertyValue;
            if (ImGui.InputInt("##valbyte", ref val) || searchFieldchanged)
            {
                PropertyValue = (byte)val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(char) || PropertyType == typeof(char[]))
        {
            var val = (int)PropertyValue;
            if (ImGui.InputInt("##valchar", ref val) || searchFieldchanged)
            {
                PropertyValue = (char)val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(short) || PropertyType == typeof(short[]))
        {
            var val = (int)PropertyValue;
            if (ImGui.InputInt("##valshort", ref val) || searchFieldchanged)
            {
                PropertyValue = (short)val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(ushort) || PropertyType == typeof(ushort[]))
        {
            var val = (int)PropertyValue;
            if (ImGui.InputInt("##valushort", ref val) || searchFieldchanged)
            {
                PropertyValue = (ushort)val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(int) || PropertyType == typeof(int[]))
        {
            var val = (int)PropertyValue;
            if (ImGui.InputInt("##valbyte", ref val) || searchFieldchanged)
            {
                PropertyValue = val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(uint) || PropertyType == typeof(uint[]))
        {
            var val = (int)PropertyValue;
            if (ImGui.InputInt("##valuint", ref val) || searchFieldchanged)
            {
                PropertyValue = (uint)val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(long) || PropertyType == typeof(long[]))
        {
            var val = (int)PropertyValue;
            if (ImGui.InputInt("##vallong", ref val) || searchFieldchanged)
            {
                PropertyValue = (long)val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(ulong) || PropertyType == typeof(ulong[]))
        {
            var val = (int)PropertyValue;
            if (ImGui.InputInt("##valulong", ref val) || searchFieldchanged)
            {
                PropertyValue = (ulong)val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(float) || PropertyType == typeof(float[]))
        {
            var val = (float)PropertyValue;
            if (ImGui.InputFloat("##valFloat", ref val) || searchFieldchanged)
            {
                PropertyValue = val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(double) || PropertyType == typeof(double[]))
        {
            var val = (double)PropertyValue;
            if (ImGui.InputDouble("##valDouble", ref val) || searchFieldchanged)
            {
                PropertyValue = val;
                ret = true;
            }
        }
        else if (PropertyType == typeof(string) || PropertyType == typeof(string[]))
        {
            string val = PropertyValue;
            if (ImGui.InputText("##valString", ref val, 99) || searchFieldchanged)
            {
                PropertyValue = val;
                ret = true;
            }
        }

        ImGui.NextColumn();
        return ret;
    }

    public void OnGui(string[] propSearchCmd = null)
    {
        var searchFieldChanged = false;
        var selectFirstResult = false;
        if (propSearchCmd != null && propSearchCmd.Length > 0)
        {
            ImGui.SetNextWindowFocus();
            PropertyName = propSearchCmd[0];
            PropertyType = Universe.GetPropertyType(PropertyName);
            ValidType = InitializeSearchValue(propSearchCmd.Length > 1 ? propSearchCmd[1] : null);
            searchFieldChanged = true;
            selectFirstResult = propSearchCmd.Contains("selectFirstResult");
        }

        if (InputTracker.GetKeyDown(KeyBindings.Current.Map_PropSearch))
        {
            ImGui.SetNextWindowFocus();
        }

        if (ImGui.Begin("Search Properties"))
        {
            ImGui.Text($"Search Properties By Name <{KeyBindings.Current.Map_PropSearch.HintText}>");
            ImGui.Separator();
            ImGui.Columns(2);
            ImGui.Text("Property Name");
            ImGui.NextColumn();

            if (InputTracker.GetKeyDown(KeyBindings.Current.Map_PropSearch))
            {
                ImGui.SetKeyboardFocusHere();
            }

            if (ImGui.InputText("##value", ref PropertyName, 64))
            {
                PropertyType = Universe.GetPropertyType(PropertyName);
                ValidType = InitializeSearchValue();
                searchFieldChanged = true;
            }

            ImGui.NextColumn();
            if (PropertyType != null && ValidType)
            {
                ImGui.Text("Type");
                ImGui.NextColumn();
                ImGui.Text(PropertyType.Name);
                ImGui.NextColumn();
                if (SearchValue(searchFieldChanged))
                {
                    FoundObjects.Clear();
                    foreach (ObjectContainer o in Universe.LoadedObjectContainers.Values)
                    {
                        if (o == null)
                        {
                            continue;
                        }

                        if (o is Map m)
                        {
                            foreach (Entity ob in m.Objects)
                            {
                                if (ob is MapEntity e)
                                {
                                    if (PropertyType.IsArray)
                                    {
                                        //search through objects to find field matches (field is an array)
                                        dynamic pArray = ob.GetPropertyValue(PropertyName);
                                        if (pArray != null)
                                        {
                                            foreach (var p in pArray)
                                            {
                                                if (p != null && p.Equals(PropertyValue))
                                                {
                                                    if (!FoundObjects.ContainsKey(e.ContainingMap.Name))
                                                    {
                                                        FoundObjects.Add(e.ContainingMap.Name,
                                                            new List<WeakReference<Entity>>());
                                                    }

                                                    FoundObjects[e.ContainingMap.Name]
                                                        .Add(new WeakReference<Entity>(e));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //search through objects to find field matches
                                        var p = ob.GetPropertyValue(PropertyName);
                                        if (p != null && p.Equals(PropertyValue))
                                        {
                                            if (!FoundObjects.ContainsKey(e.ContainingMap.Name))
                                            {
                                                FoundObjects.Add(e.ContainingMap.Name,
                                                    new List<WeakReference<Entity>>());
                                            }

                                            FoundObjects[e.ContainingMap.Name].Add(new WeakReference<Entity>(e));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ImGui.Columns(1);
            if (FoundObjects.Count > 0 && ValidType)
            {
                ImGui.Text("Search Results");
                ImGui.Separator();
                ImGui.BeginChild("Search Results");
                foreach (KeyValuePair<string, List<WeakReference<Entity>>> f in FoundObjects)
                {
                    if (ImGui.TreeNodeEx(f.Key, ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        foreach (WeakReference<Entity> o in f.Value)
                        {
                            Entity obj;
                            if (o.TryGetTarget(out obj))
                            {
                                if (selectFirstResult)
                                {
                                    // TODO: We may also want to frame this result when requested via selectFirstResult.
                                    Universe.Selection.ClearSelection();
                                    Universe.Selection.AddSelection(obj);
                                    selectFirstResult = false;
                                }

                                // TODO: We may want to frame the result on double-click.
                                // Is there a good way to use dependency inversion to handle selection/frame/goto together?
                                if (ImGui.Selectable(obj.Name, Universe.Selection.GetSelection().Contains(obj),
                                        ImGuiSelectableFlags.AllowDoubleClick))
                                {
                                    if (InputTracker.GetKey(Key.ControlLeft) ||
                                        InputTracker.GetKey(Key.ControlRight))
                                    {
                                        Universe.Selection.AddSelection(obj);
                                    }
                                    else
                                    {
                                        Universe.Selection.ClearSelection();
                                        Universe.Selection.AddSelection(obj);
                                    }
                                }
                            }
                        }

                        ImGui.TreePop();
                    }
                }

                ImGui.EndChild();
            }
        }

        ImGui.End();
    }
}
