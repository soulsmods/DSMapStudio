using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public class SearchProperties
    {
        private Universe Universe = null;
        public string PropertyName = "";

        private dynamic PropertyValue = null;
        private Type PropertyType = null;
        private bool ValidType = false;

        private Dictionary<string, List<WeakReference<Entity>>> FoundObjects = new Dictionary<string, List<WeakReference<Entity>>>();

        public SearchProperties(Universe universe)
        {
            Universe = universe;
        }

        public bool InitializeSearchValue()
        {
            if (PropertyType != null)
            {
                if (PropertyType == typeof(bool) || PropertyType == typeof(bool[]))
                {
                    PropertyValue = false;
                    return true;
                }
                else if (PropertyType == typeof(byte) || PropertyType == typeof(byte[]))
                {
                    PropertyValue = (byte)0;
                    return true;
                }
                else if (PropertyType == typeof(char) || PropertyType == typeof(char[]))
                {
                    PropertyValue = (char)0;
                    return true;
                }
                else if (PropertyType == typeof(short) || PropertyType == typeof(short[]))
                {
                    PropertyValue = (short)0;
                    return true;
                }
                else if (PropertyType == typeof(ushort) || PropertyType == typeof(ushort[]))
                {
                    PropertyValue = (ushort)0;
                    return true;
                }
                else if (PropertyType == typeof(int) || PropertyType == typeof(int[]))
                {
                    PropertyValue = (int)0;
                    return true;
                }
                else if (PropertyType == typeof(uint) || PropertyType == typeof(uint[]))
                {
                    PropertyValue = (uint)0;
                    return true;
                }
                else if (PropertyType == typeof(long) || PropertyType == typeof(long[]))
                {
                    PropertyValue = (long)0;
                    return true;
                }
                else if (PropertyType == typeof(ulong) || PropertyType == typeof(ulong[]))
                {
                    PropertyValue = (ulong)0;
                    return true;
                }
                else if (PropertyType == typeof(float) || PropertyType == typeof(float[]))
                {
                    PropertyValue = 0.0f;
                    return true;
                }
                else if (PropertyType == typeof(double) || PropertyType == typeof(double[]))
                {
                    PropertyValue = 0.0d;
                    return true;
                }
                else if (PropertyType == typeof(string) || PropertyType == typeof(string[]))
                {
                    PropertyValue = "";
                    return true;
                }
            }
            return false;
        }

        public bool SearchValue(bool searchFieldchanged)
        {
            ImGui.Text("Value (Exact)");
            ImGui.NextColumn();
            bool ret = false;
            if (PropertyType == typeof(bool) || PropertyType == typeof(bool[]))
            {
                var val = (bool)PropertyValue;
                if (ImGui.Checkbox("##valBool", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(byte) || PropertyType == typeof(byte[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##valbyte", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (byte)val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(char) || PropertyType == typeof(char[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##valchar", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (char)val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(short) || PropertyType == typeof(short[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##valshort", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (short)val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(ushort) || PropertyType == typeof(ushort[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##valushort", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (ushort)val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(int) || PropertyType == typeof(int[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##valbyte", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (int)val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(uint) || PropertyType == typeof(uint[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##valuint", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (uint)val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(long) || PropertyType == typeof(long[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##vallong", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (long)val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(ulong) || PropertyType == typeof(ulong[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##valulong", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (ulong)val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(float) || PropertyType == typeof(float[]))
            {
                var val = (float)PropertyValue;
                if (ImGui.InputFloat("##valFloat", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(double) || PropertyType == typeof(double[]))
            {
                var val = (double)PropertyValue;
                if (ImGui.InputDouble("##valDouble", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = val;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(string) || PropertyType == typeof(string[]))
            {
                string val = PropertyValue;
                if (ImGui.InputText("##valString", ref val, 99) || searchFieldchanged == true)
                {
                    PropertyValue = val;
                    ret = true;
                }
            }

            ImGui.NextColumn();
            return ret;
        }

        public void OnGui(string propname=null)
        {
            if (propname != null)
            {
                ImGui.SetNextWindowFocus();
                PropertyName = propname;
                PropertyType = Universe.GetPropertyType(PropertyName);
                ValidType = InitializeSearchValue();
            }

            if (InputTracker.GetControlShortcut(Key.F))
                ImGui.SetNextWindowFocus();
            if (ImGui.Begin("Search Properties"))
            {
                ImGui.Text("Search Properties By Name <Ctrl+F>");
                ImGui.Separator();
                ImGui.Columns(2);
                ImGui.Text("Property Name");
                ImGui.NextColumn();

                if (InputTracker.GetControlShortcut(Key.F))
                    ImGui.SetKeyboardFocusHere();

                bool searchFieldChanged = false;
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
                        foreach (var o in Universe.LoadedObjectContainers.Values)
                        {
                            if (o == null)
                            {
                                continue;
                            }
                            if (o is Map m)
                            {
                                foreach (var ob in m.Objects)
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
                                                            FoundObjects.Add(e.ContainingMap.Name, new List<WeakReference<Entity>>());
                                                        }
                                                        FoundObjects[e.ContainingMap.Name].Add(new WeakReference<Entity>(e));
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
                                                    FoundObjects.Add(e.ContainingMap.Name, new List<WeakReference<Entity>>());
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
                    foreach (var f in FoundObjects)
                    {
                        if (ImGui.TreeNodeEx(f.Key, ImGuiTreeNodeFlags.DefaultOpen))
                        {
                            foreach (var o in f.Value)
                            {
                                Entity obj;
                                if (o.TryGetTarget(out obj))
                                {
                                    if (ImGui.Selectable(obj.Name, Universe.Selection.GetSelection().Contains(obj), ImGuiSelectableFlags.AllowDoubleClick))
                                    {
                                        if (InputTracker.GetKey(Key.ControlLeft) || InputTracker.GetKey(Key.ControlRight))
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
}
