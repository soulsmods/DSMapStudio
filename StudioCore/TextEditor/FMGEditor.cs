using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Numerics;
using SoulsFormats;
using ImGuiNET;
using System.Net.Http.Headers;
using System.Security;
using System.Text.RegularExpressions;
using StudioCore.Editor;

namespace StudioCore.TextEditor
{
    public class PropertyEditor
    {
        public ActionManager ContextActionManager;

        private Dictionary<string, PropertyInfo[]> _propCache = new Dictionary<string, PropertyInfo[]>();

        public PropertyEditor(ActionManager manager)
        {
            ContextActionManager = manager;
        }

        private bool PropertyRow(Type typ, object oldval, out object newval, bool isBool)
        {
            try
            {
                if (isBool)
                {
                    dynamic val = oldval;
                    bool checkVal = val > 0;
                    if (ImGui.Checkbox("##valueBool", ref checkVal))
                    {
                        newval = Convert.ChangeType(checkVal ? 1 : 0, oldval.GetType());
                        return true;
                    }
                    ImGui.SameLine();
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
                        return true;
                    }
                }
            }
            else if (typ == typeof(int))
            {
                int val = (int)oldval;
                if (ImGui.InputInt("##value", ref val))
                {
                    newval = val;
                    return true;
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
                        return true;
                    }
                }
            }
            else if (typ == typeof(short))
            {
                int val = (short)oldval;
                if (ImGui.InputInt("##value", ref val))
                {
                    newval = (short)val;
                    return true;
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
                        return true;
                    }
                }
            }
            else if (typ == typeof(sbyte))
            {
                int val = (sbyte)oldval;
                if (ImGui.InputInt("##value", ref val))
                {
                    newval = (sbyte)val;
                    return true;
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
                        return true;
                    }
                }
            }
            else if (typ == typeof(bool))
            {
                bool val = (bool)oldval;
                if (ImGui.Checkbox("##value", ref val))
                {
                    newval = val;
                    return true;
                }
            }
            else if (typ == typeof(float))
            {
                float val = (float)oldval;
                if (ImGui.DragFloat("##value", ref val, 0.1f))
                {
                    newval = val;
                    return true;
                    // shouldUpdateVisual = true;
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
                    return true;
                }
            }
            else if (typ == typeof(Vector2))
            {
                Vector2 val = (Vector2)oldval;
                if (ImGui.DragFloat2("##value", ref val, 0.1f))
                {
                    newval = val;
                    return true;
                    // shouldUpdateVisual = true;
                }
            }
            else if (typ == typeof(Vector3))
            {
                Vector3 val = (Vector3)oldval;
                if (ImGui.DragFloat3("##value", ref val, 0.1f))
                {
                    newval = val;
                    return true;
                    // shouldUpdateVisual = true;
                }
            }
            else
            {
                ImGui.Text("ImplementMe");
            }

            newval = null;
            return false;
        }

        private void UpdateProperty(object prop, object obj, object newval,
            bool changed = true, bool committed = true, int arrayindex = -1)
        {
            if (changed)
            {
                ChangeProperty(prop, obj, newval, ref committed, arrayindex);
            }
        }

        private void ChangeProperty(object prop, object obj, object newval,
            ref bool committed, int arrayindex = -1)
        {
            if (committed)
            {
                PropertiesChangedAction action;
                if (arrayindex != -1)
                {
                    action = new PropertiesChangedAction((PropertyInfo)prop, arrayindex, obj, newval);
                }
                else
                {
                    action = new PropertiesChangedAction((PropertyInfo)prop, obj, newval);
                }
                ContextActionManager.ExecuteAction(action);
            }
        }

        private int _fmgID = 0;
        public void PropEditorFMGBegin()
        {
            _fmgID = 0;
            ImGui.Columns(2);
            ImGui.Separator();
        }

        private string _textCache = "";
        private FMG.Entry _entryCache;

        public void PropEditorFMG(FMG.Entry entry, string name, float boxsize)
        {
            ImGui.PushID(_fmgID);
            ImGui.AlignTextToFramePadding();
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);
            var oldval = entry.Text;

            string val = (string)oldval;
            if (val == null)
            {
                val = "";
            }
            if (boxsize > 0.0f)
            {
                if (ImGui.InputTextMultiline("##value", ref val, 2000, new Vector2(-1, boxsize)))
                {
                    _textCache = val;
                    _entryCache = entry;
                }
            }
            else
            {
                if (ImGui.InputText("##value", ref val, 2000))
                {
                    _textCache = val;
                    _entryCache = entry;
                }
            }

            bool committed = ImGui.IsItemDeactivatedAfterEdit();
            if (committed)
            {
                if (_textCache != _entryCache.Text)
                    UpdateProperty(_entryCache.GetType().GetProperty("Text"), _entryCache, _textCache);
            }

            ImGui.NextColumn();
            ImGui.PopID();
            _fmgID++;
        }

        private int _idCache = 0;
        private FMGBank.EntryGroup _eGroupCache;
        public void PropIDFMG(FMGBank.EntryGroup eGroup, List<FMG.Entry> entryCache)
        {
            var oldID = eGroup.ID;
            var id = oldID;
            if (ImGui.InputInt("##id", ref id))
            {
                _idCache = id;
                _eGroupCache = eGroup;
            }
            bool committed = ImGui.IsItemDeactivatedAfterEdit();
            if (committed)
            {
                if (_idCache != oldID)
                {
                    // Forbid duplicate IDs
                    if (entryCache.Find(e => e.ID == _idCache) == null)
                    {
                        UpdateProperty(_eGroupCache.GetType().GetProperty("ID"), _eGroupCache, _idCache);
                    }
                }
            }
        }

        public void PropEditorFMGEnd()
        {
            ImGui.Columns(1);
        }
    }
}
