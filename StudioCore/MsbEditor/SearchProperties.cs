using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veldrid;
using ImGuiNET;
using System.Reflection;
using System.Diagnostics;

namespace StudioCore.MsbEditor
{
    public class SearchProperties
    {
        private Universe Universe = null;
        public PropertyInfo Property
        {
            get => _property;
            set
            {
                if (value != null)
                {
                    _property = value;
                    PropertyType = value.PropertyType;
                }
            }
        }

        private PropertyInfo _property = null;
        private Type PropertyType = null;
        private dynamic PropertyValue = null;
        private bool ValidType = false;

        private Dictionary<string, List<WeakReference<Entity>>> FoundObjects = new Dictionary<string, List<WeakReference<Entity>>>();

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
                    PropertyValue = bool.TryParse(initialValue, out bool val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(byte) || PropertyType == typeof(byte[]))
                {
                    PropertyValue = byte.TryParse(initialValue, out byte val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(sbyte) || PropertyType == typeof(sbyte[]))
                {
                    PropertyValue = sbyte.TryParse(initialValue, out sbyte val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(char) || PropertyType == typeof(char[]))
                {
                    PropertyValue = char.TryParse(initialValue, out char val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(short) || PropertyType == typeof(short[]))
                {
                    PropertyValue = short.TryParse(initialValue, out short val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(ushort) || PropertyType == typeof(ushort[]))
                {
                    PropertyValue = ushort.TryParse(initialValue, out ushort val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(int) || PropertyType == typeof(int[]))
                {
                    PropertyValue = int.TryParse(initialValue, out int val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(uint) || PropertyType == typeof(uint[]))
                {
                    PropertyValue = uint.TryParse(initialValue, out uint val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(long) || PropertyType == typeof(long[]))
                {
                    PropertyValue = long.TryParse(initialValue, out long val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(ulong) || PropertyType == typeof(ulong[]))
                {
                    PropertyValue = ulong.TryParse(initialValue, out ulong val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(float) || PropertyType == typeof(float[]))
                {
                    PropertyValue = float.TryParse(initialValue, out float val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(double) || PropertyType == typeof(double[]))
                {
                    PropertyValue = double.TryParse(initialValue, out double val) ? val : default;
                    return true;
                }
                else if (PropertyType == typeof(string) || PropertyType == typeof(string[]))
                {
                    PropertyValue = initialValue ?? "";
                    return true;
                }
                else if (PropertyType.IsEnum)
                {
                    PropertyValue = PropertyType.GetEnumValues().GetValue(0);
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
            else if (PropertyType == typeof(sbyte) || PropertyType == typeof(sbyte[]))
            {
                int val = (int)PropertyValue;
                if (ImGui.InputInt("##valsbyte", ref val) || searchFieldchanged == true)
                {
                    PropertyValue = (sbyte)val;
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
            else if (PropertyType.IsEnum)
            {
                var enumVals = PropertyType.GetEnumValues();
                var enumNames = PropertyType.GetEnumNames();
                int[] intVals = new int[enumVals.Length];

                if (searchFieldchanged == true)
                {
                    ret = true;
                }

                if (PropertyType.GetEnumUnderlyingType() == typeof(byte))
                {
                    for (var i = 0; i < enumVals.Length; i++)
                        intVals[i] = (byte)enumVals.GetValue(i);

                    if (Utils.EnumEditor(enumVals, enumNames, PropertyValue, out object val, intVals))
                    {
                        PropertyValue = val;
                        ret = true;
                    }
                }
                else if (PropertyType.GetEnumUnderlyingType() == typeof(int))
                {
                    for (var i = 0; i < enumVals.Length; i++)
                        intVals[i] = (int)enumVals.GetValue(i);

                    if (Utils.EnumEditor(enumVals, enumNames, PropertyValue, out object val, intVals))
                    {
                        PropertyValue = val;
                        ret = true;
                    }
                }
                else if (PropertyType.GetEnumUnderlyingType() == typeof(uint))
                {
                    for (var i = 0; i < enumVals.Length; i++)
                        intVals[i] = (int)(uint)enumVals.GetValue(i);

                    if (Utils.EnumEditor(enumVals, enumNames, PropertyValue, out object val, intVals))
                    {
                        PropertyValue = val;
                        ret = true;
                    }
                }
                else
                {
                    ImGui.Text("Enum underlying type not implemented");
                }
            }
            else
            {
                ImGui.Text("Value type not implemented");
            }

            ImGui.NextColumn();
            return ret;
        }

        public void OnGui(string[] propSearchCmd = null)
        {
            bool newSearch = false;
            bool selectFirstResult = false;
            if (propSearchCmd != null)
            {
                ImGui.SetNextWindowFocus();
                ValidType = InitializeSearchValue();
                newSearch = true;
                selectFirstResult = propSearchCmd.Contains("selectFirstResult");
            }

            if (ImGui.Begin("Search Properties"))
            {
                ImGui.Text($"To search properties, right click them in the property editor.");
                ImGui.Separator();
                ImGui.Columns(2);

                if (Property != null && ValidType)
                {
                    ImGui.Text("Property Name");
                    ImGui.NextColumn();
                    ImGui.Text(Property.Name);
                    ImGui.NextColumn();

                    ImGui.Text("Type");
                    ImGui.NextColumn();
                    ImGui.Text(PropertyType.Name);
                    ImGui.NextColumn();
                    if (SearchValue(newSearch))
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
                                        var value = Utils.FindPropertyValue(Property, ob.WrappedObject);

                                        if (value == null)
                                        {
                                            // Object does not contain target property.
                                            continue;
                                        }

                                        if (PropertyType.IsArray)
                                        {
                                            // Property is an array, scan through each index for value matches.
                                            foreach (var p in (Array)value)
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
                                        else
                                        {
                                            if (value.Equals(PropertyValue))
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
                                    if (selectFirstResult)
                                    {
                                        // TODO: We may also want to frame this result when requested via selectFirstResult.
                                        Universe.Selection.ClearSelection();
                                        Universe.Selection.AddSelection(obj);
                                        selectFirstResult = false;
                                    }
                                    // TODO: We may want to frame the result on double-click.
                                    // Is there a good way to use dependency inversion to handle selection/frame/goto together?
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
