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
        private object PropertyValue = null;
        private Type PropertyType = null;
        private bool ValidType = false;

        private Dictionary<string, List<WeakReference<Entity>>> FoundObjects = new Dictionary<string, List<WeakReference<Entity>>>();

        public SearchProperties(Universe universe)
        {
            Universe = universe;
        }

        public bool InitializeSearchValue()
        {
            if (PropertyType == typeof(byte))
            {
                PropertyValue = (byte)0;
                return true;
            }
            else if (PropertyType == typeof(char))
            {
                PropertyValue = (char)0;
                return true;
            }
            else if (PropertyType == typeof(short))
            {
                PropertyValue = (short)0;
                return true;
            }
            else if (PropertyType == typeof(ushort))
            {
                PropertyValue = (ushort)0;
                return true;
            }
            else if (PropertyType == typeof(int))
            {
                PropertyValue = (int)0;
                return true;
            }
            else if (PropertyType == typeof(uint))
            {
                PropertyValue = (uint)0;
                return true;
            }
            else if (PropertyType == typeof(long))
            {
                PropertyValue = (long)0;
                return true;
            }
            else if (PropertyType == typeof(ulong))
            {
                PropertyValue = (ulong)0;
                return true;
            }
            else if (PropertyType == typeof(float))
            {
                PropertyValue = 0.0f;
                return true;
            }
            else if (PropertyType == typeof(string))
            {
                PropertyValue = "";
                return true;
            }
            return false;
        }

        public bool SearchValue()
        {
            ImGui.Text("Value");
            ImGui.NextColumn();
            bool ret = false;
            if (PropertyType == typeof(int))
            {
                int ival = (int)PropertyValue;
                if (ImGui.InputInt("##value2", ref ival))
                {
                    PropertyValue = ival;
                    ret = true;
                }
            }
            else if (PropertyType == typeof(string))
            {
                string val = (string)PropertyValue;
                if (val == null)
                {
                    val = "";
                }
                if (ImGui.InputText("##value2", ref val, 40))
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
            if (ImGui.Begin("Search Properties"))
            {
                ImGui.Columns(2);
                ImGui.Text("Property Name");
                ImGui.NextColumn();
                if (ImGui.InputText("##value", ref PropertyName, 40))
                {
                    PropertyType = Universe.GetPropertyType(PropertyName);
                    ValidType = InitializeSearchValue();
                }
                ImGui.NextColumn();
                if (PropertyType != null && ValidType)
                {
                    ImGui.Text("Type");
                    ImGui.NextColumn();
                    ImGui.Text(PropertyType.Name);
                    ImGui.NextColumn();
                    if (SearchValue())
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
                ImGui.Columns(1);
                if (FoundObjects.Count > 0)
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
                    ImGui.End();
                }
            }
        }
    }
}
