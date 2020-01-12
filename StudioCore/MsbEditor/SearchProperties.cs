using System;
using System.Collections.Generic;
using System.Text;
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

        private List<MapObject> FoundObjects = new List<MapObject>();

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
            ImGui.NextColumn();
            return ret;
        }

        public void OnGui()
        {
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
                        foreach (var m in Universe.LoadedMaps)
                        {
                            foreach (var o in m.MapObjects)
                            {
                                var p = o.GetPropertyValue(PropertyName);
                                if (p != null && p.Equals(PropertyValue))
                                {
                                    FoundObjects.Add(o);
                                }
                            }
                        }
                    }
                }
                ImGui.Columns(1);
                foreach (var f in FoundObjects)
                {
                    ImGui.Text(f.Name);
                }
                ImGui.End();
            }
        }
    }
}
