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

        private Dictionary<string, PropertyInfo[]> PropCache = new Dictionary<string, PropertyInfo[]>();

        private object ChangingObject = null;
        private object ChangingPropery = null;
        private object ChangedValue = null;
        private Action LastUncommittedAction = null;

        private string currentAutoComplete = "";
        private int searchStartPos = 0;

        public PropertyEditor(ActionManager manager)
        {
            ContextActionManager = manager;
        }

        private bool PropertyRow(Type typ, object oldval, out object newval, Entity obj=null, string propname=null)
        {
            if (typ == typeof(long))
            {
                long val = (long)oldval;
                string strval = $@"{val}";
                if (ImGui.InputText("##value", ref strval, 40))
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
                if (obj != null && ImGui.BeginPopupContextItem(propname))
                {
                    bool r = false;
                    if (ImGui.Selectable("Set Next Unique Value"))
                    {
                        newval = obj.Container.GetNextUnique(propname, val);
                        ImGui.EndPopup();
                        return true;
                    }
                    ImGui.EndPopup();
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
                    //shouldUpdateVisual = true;
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
                    //shouldUpdateVisual = true;
                }
            }
            else if (typ == typeof(Vector3))
            {
                Vector3 val = (Vector3)oldval;
                if (ImGui.DragFloat3("##value", ref val, 0.1f))
                {
                    newval = val;
                    return true;
                    //shouldUpdateVisual = true;
                }
            }
            else
            {
                ImGui.Text("ImplementMe");
            }

            newval = null;
            return false;
        }

        private void UpdateProperty(object prop, Entity selection, object obj, object newval,
            bool changed, bool committed, bool shouldUpdateVisual, int arrayindex = -1)
        {
            if (changed)
            {
                ChangeProperty(prop, selection, obj, newval, ref committed, shouldUpdateVisual, arrayindex);
            }
            if (committed)
            {
                CommitProperty(selection);
            }
        }
        private void ChangeProperty(object prop, Entity selection, object obj, object newval,
            ref bool committed, bool shouldUpdateVisual, int arrayindex = -1)
        {
            if (prop == ChangingPropery && LastUncommittedAction != null && ContextActionManager.PeekUndoAction() == LastUncommittedAction)
            {
                ContextActionManager.UndoAction();
            }
            else
            {
                LastUncommittedAction = null;
            }

            if (ChangingObject != null && selection != null && selection.WrappedObject != ChangingObject)
            {
                committed = true;
            }
            else
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
                if (shouldUpdateVisual && selection != null)
                {
                    action.SetPostExecutionAction((undo) =>
                    {
                        selection.UpdateRenderModel();
                    });
                }
                ContextActionManager.ExecuteAction(action);

                LastUncommittedAction = action;
                ChangingPropery = prop;
                //ChangingObject = selection.MsbObject;
                ChangingObject = selection != null ? selection.WrappedObject : obj;
            }
        }
        private void CommitProperty(Entity selection)
        {
            // Invalidate name cache
            if (selection != null)
            {
                selection.Name = null;
            }

            // Undo and redo the last action with a rendering update
            if (LastUncommittedAction != null && ContextActionManager.PeekUndoAction() == LastUncommittedAction)
            {
                if (LastUncommittedAction is PropertiesChangedAction a)
                {
                    // Kinda a hack to prevent a jumping glitch
                    a.SetPostExecutionAction(null);
                    ContextActionManager.UndoAction();
                    if (selection != null)
                    {
                        a.SetPostExecutionAction((undo) =>
                        {
                            selection.UpdateRenderModel();
                        });
                    }
                    ContextActionManager.ExecuteAction(a);
                }
            }

            LastUncommittedAction = null;
            ChangingPropery = null;
            ChangingObject = null;
        }

        private void PropEditorParamRow(Entity selection)
        {
            IReadOnlyList<PARAM.Cell> cells = new List<PARAM.Cell>();
            if (selection.WrappedObject is PARAM.Row row)
            {
                cells = row.Cells;

            }
            else if (selection.WrappedObject is MergedParamRow mrow)
            {
                cells = mrow.Cells;
            }
            ImGui.Columns(2);
            ImGui.Separator();
            int id = 0;

            // This should be rewritten somehow it's super ugly
            var nameProp = selection.WrappedObject.GetType().GetProperty("Name");
            var idProp = selection.WrappedObject.GetType().GetProperty("ID");
            PropEditorPropInfoRow(selection.WrappedObject, nameProp, "Name", ref id, selection);
            PropEditorPropInfoRow(selection.WrappedObject, idProp, "ID", ref id, selection);

            foreach (var cell in cells)
            {
                PropEditorPropCellRow(cell, ref id, selection);
            }
            ImGui.Columns(1);
        }

        public void PropEditorParamRow(PARAM.Row row)
        {
            IReadOnlyList<PARAM.Cell> cells = new List<PARAM.Cell>();
            cells = row.Cells;
            ImGui.Columns(2);
            ImGui.Separator();
            int id = 0;

            // This should be rewritten somehow it's super ugly
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
            var nameProp = row.GetType().GetProperty("Name");
            var idProp = row.GetType().GetProperty("ID");
            PropEditorPropInfoRow(row, nameProp, "Name", ref id, null);
            PropEditorPropInfoRow(row, idProp, "ID", ref id, null);
            ImGui.PopStyleColor();

            foreach (var cell in cells)
            {
                PropEditorPropCellRow(cell, ref id, null);
            }
            ImGui.Columns(1);
        }
        //Absolute parameter spam because everything is some minor variation
        //Should really determine which options are necessary and which can be safely removed
        private void PropEditorPropInfoRow(object rowOrWrappedObject, PropertyInfo prop, string visualName, ref int id, Entity nullableSelection)
        {
            PropEditorPropRow(prop.GetValue(rowOrWrappedObject), ref id, visualName, null, prop.PropertyType, null, null, prop, rowOrWrappedObject, nullableSelection);
        }
        private void PropEditorPropCellRow(PARAM.Cell cell, ref int id, Entity nullableSelection)
        {
            PropEditorPropRow(cell.Value, ref id, cell.Def.InternalName, cell.Def.Meta.RefTypes, cell.Value.GetType(), null, cell.Def.InternalName, cell.GetType().GetProperty("Value"), cell, nullableSelection);
        }
        private void PropEditorPropRow(object oldval, ref int id, string visualName, List<string> reftypes, Type propType, Entity nullableEntity, string nullableName, PropertyInfo proprow, object paramRowOrCell, Entity nullableSelection)
        {
            object newval = null;
            ImGui.PushID(id);
            ImGui.AlignTextToFramePadding();
            ImGui.Text(visualName);
            if(reftypes!=null)
            {
                    ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), " <"+String.Join(',', reftypes)+">");
            }
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);
            bool changed = false;
            if(reftypes!=null)
            {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 1.0f, 1.0f));
            }
            changed = PropertyRow(propType, oldval, out newval, nullableEntity, nullableName);
            bool committed = ImGui.IsItemDeactivatedAfterEdit();
            if(reftypes!=null)
            {
                ImGui.PopStyleColor();
                if(propType==typeof(int))
                {
                    changed |= PropertyRowRefs(reftypes, oldval, ref newval);
                }
            }
            UpdateProperty(proprow, nullableSelection, paramRowOrCell, newval, changed, committed, false);
            ImGui.NextColumn();
            ImGui.PopID();
            id++;
        }
        private bool PropertyRowRefs(List<string> reftypes, object oldval ,ref object newval)
        {
            //Add named row and context menu
            //This should probably be moved to PropertyRow stuff, but the whole drawing system is all over the place anyway
            //List located params
            ImGui.NewLine();
            foreach(string rt in reftypes)
            {
                if(ParamBank.Params.ContainsKey(rt))
                {
                    PARAM.Row r = ParamBank.Params[rt][(int)oldval];
                    if(r!=null && r.Name!=null)
                    {
                        ImGui.SameLine();
                        ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.5f, 1.0f), r.Name);
                    }
                }
                //Attach context menu
                return PropertyRowRefsContextMenu(reftypes, oldval, ref newval);
            }
            return false;
        }
        private bool PropertyRowRefsContextMenu(List<string> reftypes, object oldval, ref object newval)
        {
            if (ImGui.BeginPopupContextItem(String.Join(',', reftypes)))
            {   
                //Add Goto statements
                foreach(string rt in reftypes)
                {
                    if(!ParamBank.Params.ContainsKey(rt))
                    {
                        continue;
                    }
                    if (ParamBank.Params[rt][(int)oldval]!=null && ImGui.Selectable($@"Go to {rt}"))
                    {   
                        EditorCommandQueue.AddCommand($@"param/select/{rt}/{oldval}");
                    }
                }
                //Add searchbar for named editing
                ImGui.InputText("##value", ref currentAutoComplete, 128);
                //Unordered scanthrough search for matching param entries.
                //This needs to be replaced by a proper search box with a scroll and everything
                foreach(string rt in reftypes)
                {
                    if(currentAutoComplete!="")
                    {
                        int max = 15/reftypes.Count;//Magic numbers
                        foreach(PARAM.Row r in ParamBank.Params[rt].Rows)
                        {
                            if(max<=0)
                                break;
                            if(r.Name==null)
                                continue;
                            if(RefContextSearchMatch(r.Name, currentAutoComplete))
                            {
                                if(ImGui.Selectable(r.Name))
                                {
                                    newval = (int)r.ID;
                                    currentAutoComplete = "";
                                    ImGui.EndPopup();
                                    return true;
                                }
                                max--;
                            }
                        }
                    }
                }
                ImGui.EndPopup();
            }
            return false;
        }
        //Return true if a param name should be found by searching term. Ordering is not provided.
        private bool RefContextSearchMatch(string name, string term)
        {
            return name.ToLower().Contains(term.ToLower());//simple, could be expanded. Not regex cus that is clunky for a quick menu.
        }

        private int _fmgID = 0;
        public void PropEditorFMGBegin()
        {
            _fmgID = 0;
            ImGui.Columns(2);
            ImGui.Separator();
        }

        public void PropEditorFMG(FMG.Entry entry, string name, float boxsize)
        {
            ImGui.PushID(_fmgID);
            ImGui.AlignTextToFramePadding();
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);
            //ImGui.AlignTextToFramePadding();
            var typ = typeof(string);
            var oldval = entry.Text;
            bool shouldUpdateVisual = false;
            bool changed = false;
            object newval = null;

            string val = (string)oldval;
            if (val == null)
            {
                val = "";
            }
            if (boxsize > 0.0f)
            {
                if (ImGui.InputTextMultiline("##value", ref val, 2000, new Vector2(-1, boxsize)))
                {
                    newval = val;
                    changed = true;
                }
            }
            else
            {
                if (ImGui.InputText("##value", ref val, 2000))
                {
                    newval = val;
                    changed = true;
                }
            }

            bool committed = ImGui.IsItemDeactivatedAfterEdit();
            UpdateProperty(entry.GetType().GetProperty("Text"), null, entry, newval, changed, committed, shouldUpdateVisual);

            ImGui.NextColumn();
            ImGui.PopID();
            _fmgID++;
        }

        public void PropEditorFMGEnd()
        {
            ImGui.Columns(1);
        }

        private void PropertyContextMenu(object obj, PropertyInfo propinfo)
        {
            if (ImGui.BeginPopupContextItem(propinfo.Name))
            {
                var att = propinfo.GetCustomAttribute<MSBParamReference>();
                if (att != null)
                {
                    if (ImGui.Selectable($@"Goto {att.ParamName}"))
                    {
                        var id = (int)propinfo.GetValue(obj);
                        EditorCommandQueue.AddCommand($@"param/select/{att.ParamName}/{id}");
                    }
                }
                if (ImGui.Selectable($@"Search"))
                {
                    EditorCommandQueue.AddCommand($@"map/propsearch/{propinfo.Name}");
                }
                ImGui.EndPopup();
            }
        }

        private void PropEditorFlverLayout(Entity selection, FLVER2.BufferLayout layout)
        {
            foreach (var l in layout)
            {
                ImGui.Text(l.Semantic.ToString());
                ImGui.NextColumn();
                ImGui.Text(l.Type.ToString());
                ImGui.NextColumn();
            }
        }

        private void PropEditorGeneric(Entity selection, object target=null, bool decorate=true)
        {
            var obj = (target == null) ? selection.WrappedObject : target;
            var type = obj.GetType();
            if (!PropCache.ContainsKey(type.FullName))
            {
                PropCache.Add(type.FullName, type.GetProperties(BindingFlags.Instance | BindingFlags.Public));
            }
            var properties = PropCache[type.FullName];
            if (decorate)
            {
                ImGui.Columns(2);
                ImGui.Separator();
                ImGui.Text("Object Type");
                ImGui.NextColumn();
                ImGui.Text(type.Name);
                ImGui.NextColumn();
            }

            // Custom editors
            if (type == typeof(FLVER2.BufferLayout))
            {
                PropEditorFlverLayout(selection, (FLVER2.BufferLayout)obj);
            }
            else
            {
                int id = 0;
                foreach (var prop in properties)
                {
                    if (!prop.CanWrite && !prop.PropertyType.IsArray)
                    {
                        continue;
                    }

                    if (prop.GetCustomAttribute<HideProperty>() != null)
                    {
                        continue;
                    }

                    ImGui.PushID(id);
                    ImGui.AlignTextToFramePadding();
                    //ImGui.AlignTextToFramePadding();
                    var typ = prop.PropertyType;

                    if (typ.IsArray)
                    {
                        Array a = (Array)prop.GetValue(obj);
                        for (int i = 0; i < a.Length; i++)
                        {
                            ImGui.PushID(i);

                            var arrtyp = typ.GetElementType();
                            if (arrtyp.IsClass && arrtyp != typeof(string) && !arrtyp.IsArray)
                            {
                                bool open = ImGui.TreeNodeEx($@"{prop.Name}[{i}]", ImGuiTreeNodeFlags.DefaultOpen);
                                ImGui.NextColumn();
                                ImGui.SetNextItemWidth(-1);
                                var o = a.GetValue(i);
                                ImGui.Text(o.GetType().Name);
                                ImGui.NextColumn();
                                if (open)
                                {
                                    PropEditorGeneric(selection, o, false);
                                    ImGui.TreePop();
                                }
                                ImGui.PopID();
                            }
                            else
                            {
                                ImGui.Text($@"{prop.Name}[{i}]");
                                ImGui.NextColumn();
                                ImGui.SetNextItemWidth(-1);
                                var oldval = a.GetValue(i);
                                bool shouldUpdateVisual = false;
                                bool changed = false;
                                object newval = null;

                                changed = PropertyRow(typ.GetElementType(), oldval, out newval);
                                //PropertyContextMenu(prop);
                                if (ImGui.IsItemActive() && !ImGui.IsWindowFocused())
                                {
                                    ImGui.SetItemDefaultFocus();
                                }
                                bool committed = ImGui.IsItemDeactivatedAfterEdit();
                                UpdateProperty(prop, selection, obj, newval, changed, committed, shouldUpdateVisual, i);

                                ImGui.NextColumn();
                                ImGui.PopID();
                            }
                        }
                        ImGui.PopID();
                    }
                    else if (typ.IsGenericType && typ.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        object l = prop.GetValue(obj);
                        PropertyInfo itemprop = l.GetType().GetProperty("Item");
                        int count = (int)l.GetType().GetProperty("Count").GetValue(l);
                        for (int i = 0; i < count; i++)
                        {
                            ImGui.PushID(i);

                            var arrtyp = typ.GetGenericArguments()[0];
                            if (arrtyp.IsClass && arrtyp != typeof(string) && !arrtyp.IsArray)
                            {
                                bool open = ImGui.TreeNodeEx($@"{prop.Name}[{i}]", ImGuiTreeNodeFlags.DefaultOpen);
                                ImGui.NextColumn();
                                ImGui.SetNextItemWidth(-1);
                                var o = itemprop.GetValue(l, new object[] { i });
                                ImGui.Text(o.GetType().Name);
                                ImGui.NextColumn();
                                if (open)
                                {
                                    PropEditorGeneric(selection, o, false);
                                    ImGui.TreePop();
                                }
                                ImGui.PopID();
                            }
                            else
                            {
                                ImGui.Text($@"{prop.Name}[{i}]");
                                ImGui.NextColumn();
                                ImGui.SetNextItemWidth(-1);
                                var oldval = itemprop.GetValue(l, new object[] { i });
                                bool shouldUpdateVisual = false;
                                bool changed = false;
                                object newval = null;

                                changed = PropertyRow(arrtyp, oldval, out newval);
                                PropertyContextMenu(obj, prop);
                                if (ImGui.IsItemActive() && !ImGui.IsWindowFocused())
                                {
                                    ImGui.SetItemDefaultFocus();
                                }
                                bool committed = ImGui.IsItemDeactivatedAfterEdit();
                                UpdateProperty(prop, selection, obj, newval, changed, committed, shouldUpdateVisual, i);

                                ImGui.NextColumn();
                                ImGui.PopID();
                            }
                        }
                        ImGui.PopID();
                    }
                    else if (typ.IsClass && typ != typeof(string) && !typ.IsArray)
                    {
                        bool open = ImGui.TreeNodeEx(prop.Name, ImGuiTreeNodeFlags.DefaultOpen);
                        ImGui.NextColumn();
                        ImGui.SetNextItemWidth(-1);
                        var o = prop.GetValue(obj);
                        ImGui.Text(o.GetType().Name);
                        ImGui.NextColumn();
                        if (open)
                        {
                            PropEditorGeneric(selection, o, false);
                            ImGui.TreePop();
                        }
                        ImGui.PopID();
                    }
                    else
                    {
                        ImGui.Text(prop.Name);
                        ImGui.NextColumn();
                        ImGui.SetNextItemWidth(-1);
                        var oldval = prop.GetValue(obj);
                        bool shouldUpdateVisual = false;
                        bool changed = false;
                        object newval = null;

                        changed = PropertyRow(typ, oldval, out newval, selection, prop.Name);
                        PropertyContextMenu(obj, prop);
                        if (ImGui.IsItemActive() && !ImGui.IsWindowFocused())
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                        bool committed = ImGui.IsItemDeactivatedAfterEdit();
                        UpdateProperty(prop, selection, obj, newval, changed, committed, shouldUpdateVisual);

                        ImGui.NextColumn();
                        ImGui.PopID();
                    }
                    id++;
                }
            }
            if (decorate)
            {
                ImGui.Columns(1);
                if (selection.References != null)
                {
                    ImGui.NewLine();
                    ImGui.Text("References: ");
                    foreach (var m in selection.References)
                    {
                        foreach (var n in m.Value)
                        {
                            ImGui.Text(n.PrettyName);
                        }
                    }
                }
                ImGui.NewLine();
                ImGui.Text("Objects referencing this object:");
                foreach (var m in selection.GetReferencingObjects())
                {
                    ImGui.Text(m.PrettyName);
                }
            }
        }

        public void OnGui(Entity selection, string id, float w, float h)
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.145f, 0.145f, 0.149f, 1.0f));
            ImGui.SetNextWindowSize(new Vector2(350, h - 80), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(w - 370, 20), ImGuiCond.FirstUseEver);
            ImGui.Begin($@"Properties##{id}");
            ImGui.BeginChild("propedit");
            if (selection == null || selection.WrappedObject == null)
            {
                ImGui.Text("Select a single object to edit properties.");
                ImGui.EndChild();
                ImGui.End();
                ImGui.PopStyleColor();
                return;
            }
            if (selection.WrappedObject is PARAM.Row prow || selection.WrappedObject is MergedParamRow)
            {
                PropEditorParamRow(selection);
            }
            else
            {
                PropEditorGeneric(selection);
            }
            ImGui.EndChild();
            ImGui.End();
            ImGui.PopStyleColor();
        }
    }
}
