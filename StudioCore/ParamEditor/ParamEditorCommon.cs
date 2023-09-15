using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Numerics;
using SoulsFormats;
using ImGuiNET;
using Veldrid;
using System.Net.Http.Headers;
using System.Security;
using System.Text.RegularExpressions;
using FSParam;
using StudioCore;
using StudioCore.Editor;

namespace StudioCore.ParamEditor
{
    public class ParamEditorCommon
    {
        private static object _editedPropCache;
        private static object _editedTypeCache;
        private static object _editedObjCache;
        private static bool _changedCache;
        private static bool _committedCache;

        public static unsafe void PropertyField(Type typ, object oldval, ref object newval, bool isBool)
        {
            _changedCache = false;
            _committedCache = false;
            ImGui.SetNextItemWidth(-1);
            try
            {
                if (isBool)
                {
                    dynamic val = oldval;
                    bool checkVal = val > 0;
                    if (ImGui.Checkbox("##valueBool", ref checkVal))
                    {
                        newval = Convert.ChangeType(checkVal ? 1 : 0, oldval.GetType());
                        _editedPropCache = newval;
                        _changedCache = true;
                    }
                    _committedCache = ImGui.IsItemDeactivatedAfterEdit();
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1);
                }
            }
            catch
            {

            }

            if (typ == typeof(long))
            {
                long val = (long)oldval;
                string strval = $@"{val}";
                if (ImGui.InputText("##value", ref strval, 128))
                {
                    var res = long.TryParse(strval, out val);
                    if (res)
                    {
                        newval = val;
                        _editedPropCache = newval;
                        _changedCache = true;
                    }
                }
            }
            else if (typ == typeof(int))
            {
                int val = (int)oldval;
                if (ImGui.InputInt("##value", ref val))
                {
                    newval = val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(uint))
            {
                uint val = (uint)oldval;
                string strval = $@"{val}";
                if (ImGui.InputText("##value", ref strval, 16))
                {
                    var res = uint.TryParse(strval, out val);
                    if (res)
                    {
                        newval = val;
                        _editedPropCache = newval;
                        _changedCache = true;
                    }
                }
            }
            else if (typ == typeof(short))
            {
                int val = (short)oldval;
                if (ImGui.InputInt("##value", ref val))
                {
                    newval = (short)val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(ushort))
            {
                ushort val = (ushort)oldval;
                string strval = $@"{val}";
                if (ImGui.InputText("##value", ref strval, 5))
                {
                    var res = ushort.TryParse(strval, out val);
                    if (res)
                    {
                        newval = val;
                        _editedPropCache = newval;
                        _changedCache = true;
                    }
                }
            }
            else if (typ == typeof(sbyte))
            {
                int val = (sbyte)oldval;
                if (ImGui.InputInt("##value", ref val))
                {
                    newval = (sbyte)val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(byte))
            {
                byte val = (byte)oldval;
                string strval = $@"{val}";
                if (ImGui.InputText("##value", ref strval, 3))
                {
                    var res = byte.TryParse(strval, out val);
                    if (res)
                    {
                        newval = val;
                        _editedPropCache = newval;
                        _changedCache = true;
                    }
                }
            }
            else if (typ == typeof(bool))
            {
                bool val = (bool)oldval;
                if (ImGui.Checkbox("##value", ref val))
                {
                    newval = val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(float))
            {
                float val = (float)oldval;
                if (ImGui.InputFloat("##value", ref val, 0.1f))
                {
                    newval = val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(double))
            {
                double val = (double)oldval;
                if (ImGui.InputScalar("##value", ImGuiDataType.Double, new IntPtr(&val)))
                {
                    newval = val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(string))
            {
                string val = (string)oldval;
                if (val == null)
                {
                    val = "";
                }
                if (ImGui.InputText("##value", ref val, 128))
                {
                    newval = val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(Vector2))
            {
                Vector2 val = (Vector2)oldval;
                if (ImGui.InputFloat2("##value", ref val))
                {
                    newval = val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(Vector3))
            {
                Vector3 val = (Vector3)oldval;
                if (ImGui.InputFloat3("##value", ref val))
                {
                    newval = val;
                    _editedPropCache = newval;
                    _changedCache = true;
                }
            }
            else if (typ == typeof(byte[]))
            {
                byte[] bval = (byte[])oldval;
                string val = ParamUtils.Dummy8Write(bval);
                if (ImGui.InputText("##value", ref val, 9999))
                {
                    byte[] nval = ParamUtils.Dummy8Read(val, bval.Length);
                    if (nval!=null)
                    {
                        newval = nval;
                        _editedPropCache = newval;
                        _changedCache = true;
                    }
                }
            }
            else
            {
                // Using InputText means IsItemDeactivatedAfterEdit doesn't pick up random previous item
                string implMe = "ImplementMe";
                ImGui.InputText(null, ref implMe, 256, ImGuiInputTextFlags.ReadOnly);
            }
            _committedCache |= ImGui.IsItemDeactivatedAfterEdit();
        }

        public static void SetLastPropertyManual(object newval)
        {
            _editedPropCache = newval;
            _changedCache = true;
            _committedCache = true;
        }
        
        public static bool UpdateProperty(ActionManager executor, object obj, PropertyInfo prop, object oldval, int arrayindex = -1)
        {
            if (_changedCache)
            {
                _editedObjCache = obj;
                _editedTypeCache = prop;
            }
            else if (_editedPropCache != null && _editedPropCache != oldval)
            {
                _changedCache = true;
            }
            if (_changedCache)
            {
                ChangeProperty(executor, _editedTypeCache, _editedObjCache, _editedPropCache, ref _committedCache, arrayindex);
            }
            return _changedCache && _committedCache;
        }
        private static void ChangeProperty(ActionManager executor, object prop, object obj, object newval,
            ref bool committed, int arrayindex = -1)
        {
            if (committed)
            {
                if (newval == null)
                {
                    // Safety check warned to user, should have proper crash handler instead
                    TaskLogs.AddLog("ParamEditorCommon: Property changed was null",
                        Microsoft.Extensions.Logging.LogLevel.Warning);
                    return;
                }
                PropertiesChangedAction action;
                if (arrayindex != -1)
                {
                    action = new PropertiesChangedAction((PropertyInfo)prop, arrayindex, obj, newval);
                }
                else
                {
                    action = new PropertiesChangedAction((PropertyInfo)prop, obj, newval);
                }
                executor.ExecuteAction(action);
            }
        }
    }
}