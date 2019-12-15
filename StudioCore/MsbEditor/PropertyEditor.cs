using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Numerics;
using SoulsFormats;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public class PropertyEditor
    {
        public ActionManager ContextActionManager;

        public PropertyEditor(ActionManager manager)
        {
            ContextActionManager = manager;
        }

        public void OnGui(MapObject selection, float w, float h)
        {
            ImGui.SetNextWindowSize(new Vector2(350, h - 80), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(w - 370, 20), ImGuiCond.FirstUseEver);
            ImGui.Begin("Properties");
            if (selection == null)
            {
                ImGui.Text("Select a single object to edit properties.");
                ImGui.End();
                return;
            }
            var obj = selection.MsbObject;
            var type = obj.GetType();
            //var properties = type.GetProperties();
            var properties = from property in type.GetProperties()
                             where Attribute.IsDefined(property, typeof(OrderAttribute))
                             orderby ((OrderAttribute)property
                                       .GetCustomAttributes(typeof(OrderAttribute), false)
                                       .Single()).Order
                             select property;
            ImGui.Columns(2);
            ImGui.Separator();
            int id = 0;
            bool dirty = false;
            foreach (var prop in properties)
            {
                ImGui.PushID(id);
                ImGui.AlignTextToFramePadding();
                ImGui.Text(prop.Name);
                ImGui.NextColumn();
                ImGui.AlignTextToFramePadding();
                var typ = prop.PropertyType;
                var oldval = prop.GetValue(obj);
                var changed = false;
                object newval = null;
                if (typ == typeof(int))
                {
                    int val = (int)oldval;
                    if (ImGui.InputInt("##value", ref val))
                    {
                        newval = val;
                        changed = true;
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
                            changed = true;
                        }
                    }
                }
                else if (typ == typeof(short))
                {
                    int val = (short)oldval;
                    if (ImGui.InputInt("##value", ref val))
                    {
                        newval = (short)val;
                        changed = true;
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
                            changed = true;
                        }
                    }
                }
                else if (typ == typeof(sbyte))
                {
                    int val = (sbyte)oldval;
                    if (ImGui.InputInt("##value", ref val))
                    {
                        newval = (sbyte)val;
                        changed = true;
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
                            changed = true;
                        }
                    }
                }
                else if (typ == typeof(bool))
                {
                    bool val = (bool)oldval;
                    if (ImGui.Checkbox("##value", ref val))
                    {
                        newval = val;
                        changed = true;
                    }
                }
                else if (typ == typeof(string))
                {
                    string val = (string)oldval;
                    if (val == null)
                    {
                        val = "";
                    }
                    if (ImGui.InputText("##value", ref val, 40))
                    {
                        newval = val;
                        changed = true;
                    }
                }
                else if (typ == typeof(Vector3))
                {
                    Vector3 val = (Vector3)oldval;
                    if (ImGui.InputFloat3("##value", ref val))
                    {
                        newval = val;
                        changed = true;
                    }
                }
                else
                {
                    ImGui.Text("ImplementMe");
                }
                if (changed)
                {
                    var action = new PropertiesChangedAction(prop, obj, newval);
                    action.SetPostExecutionAction((undo) =>
                    {
                        selection.UpdateRenderTransform();
                    });
                    ContextActionManager.ExecuteAction(action);
                    dirty = true;
                }
                ImGui.NextColumn();
                ImGui.PopID();
                id++;
            }
            ImGui.Columns(1);
            ImGui.End();

            if (dirty)
            {
                //selection.UpdateRenderTransform();
            }
        }
    }
}
