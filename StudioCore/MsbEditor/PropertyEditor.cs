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
using FSParam;

namespace StudioCore.MsbEditor
{
    public class PropertyEditor
    {
        public ActionManager ContextActionManager;

        private Dictionary<string, PropertyInfo[]> _propCache = new Dictionary<string, PropertyInfo[]>();

        private object _changingObject = null;
        private object _changingPropery = null;
        private Action _lastUncommittedAction = null;

        private string _refContextCurrentAutoComplete = "";

        public PropertyEditor(ActionManager manager)
        {
            ContextActionManager = manager;
        }

        private bool PropertyRow(Type typ, object oldval, out object newval, Entity obj = null, string propname = null)
        {
            if (typ == typeof(long))
            {
                long val = (long)oldval;
                string strval = $@"{val}";
                if (ImGui.InputText("##value", ref strval, 99))
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
                if (ImGui.InputText("##value", ref val, 99))
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

        private void UpdateProperty(object prop, Entity selection, object obj, object newval,
            bool changed, bool committed, bool shouldUpdateVisual, bool destroyRenderModel, int arrayindex = -1)
        {
            if (changed)
            {
                ChangeProperty(prop, selection, obj, newval, ref committed, shouldUpdateVisual, destroyRenderModel, arrayindex);
            }
            if (committed)
            {
                CommitProperty(selection, destroyRenderModel);
            }
        }

        private void ChangeProperty(object prop, Entity selection, object obj, object newval,
            ref bool committed, bool shouldUpdateVisual, bool destroyRenderModel, int arrayindex = -1)
        {
            if (prop == _changingPropery && _lastUncommittedAction != null && ContextActionManager.PeekUndoAction() == _lastUncommittedAction)
            {
                ContextActionManager.UndoAction();
            }
            else
            {
                _lastUncommittedAction = null;
            }

            if (_changingObject != null && selection != null && selection.WrappedObject != _changingObject)
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
                        if (destroyRenderModel)
                        {
                            if (selection.RenderSceneMesh != null)
                            {
                                selection.RenderSceneMesh.Dispose();
                                selection.RenderSceneMesh = null;
                            }
                        }
                        selection.UpdateRenderModel();
                    });
                }
                ContextActionManager.ExecuteAction(action);

                _lastUncommittedAction = action;
                _changingPropery = prop;
                // ChangingObject = selection.MsbObject;
                _changingObject = selection != null ? selection.WrappedObject : obj;
            }
        }
        private void CommitProperty(Entity selection, bool destroyRenderModel)
        {
            // Invalidate name cache
            if (selection != null)
            {
                selection.Name = null;
            }

            // Undo and redo the last action with a rendering update
            if (_lastUncommittedAction != null && ContextActionManager.PeekUndoAction() == _lastUncommittedAction)
            {
                if (_lastUncommittedAction is PropertiesChangedAction a)
                {
                    // Kinda a hack to prevent a jumping glitch
                    a.SetPostExecutionAction(null);
                    ContextActionManager.UndoAction();
                    if (selection != null)
                    {
                        a.SetPostExecutionAction((undo) =>
                        {
                            if (destroyRenderModel)
                            {
                                if (selection.RenderSceneMesh != null)
                                {
                                    selection.RenderSceneMesh.Dispose();
                                    selection.RenderSceneMesh = null;
                                }
                            }
                            selection.UpdateRenderModel();
                        });
                    }
                    ContextActionManager.ExecuteAction(a);
                }
            }

            _lastUncommittedAction = null;
            _changingPropery = null;
            _changingObject = null;
        }

        private void PropEditorParamRow(Entity selection)
        {
            IReadOnlyList<Param.Cell> cells = new List<Param.Cell>();
            if (selection.WrappedObject is Param.Row row)
            {
                cells = row.CellHandles;

            }
            else if (selection.WrappedObject is MergedParamRow mrow)
            {
                cells = mrow.CellHandles;
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

        public void PropEditorParamRow(Param.Row row)
        {
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

            foreach (var cell in row.Cells)
            {
                PropEditorPropCellRow(row[cell], ref id, null);
            }
            ImGui.Columns(1);
        }

        // Many parameter options, which may be simplified.
        private void PropEditorPropInfoRow(object rowOrWrappedObject, PropertyInfo prop, string visualName, ref int id, Entity nullableSelection)
        {
            PropEditorPropRow(prop.GetValue(rowOrWrappedObject), ref id, visualName, prop.PropertyType, null, null, prop, rowOrWrappedObject, nullableSelection);
        }
        private void PropEditorPropCellRow(Param.Cell cell, ref int id, Entity nullableSelection)
        {
            PropEditorPropRow(cell.Value, ref id, cell.Def.InternalName, cell.Value.GetType(), null, cell.Def.InternalName, cell.GetType().GetProperty("Value"), cell, nullableSelection);
        }
        private void PropEditorPropRow(object oldval, ref int id, string visualName, Type propType, Entity nullableEntity, string nullableName, PropertyInfo proprow, object paramRowOrCell, Entity nullableSelection)
        {
            object newval = null;
            ImGui.PushID(id);
            ImGui.AlignTextToFramePadding();
            ImGui.Text(visualName);
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);
            bool changed = false;

            changed = PropertyRow(propType, oldval, out newval, nullableEntity, nullableName);
            bool committed = ImGui.IsItemDeactivatedAfterEdit();
            //bool committed = true;
            UpdateProperty(proprow, nullableSelection, paramRowOrCell, newval, changed, committed, false, false);
            ImGui.NextColumn();
            ImGui.PopID();
            id++;
        }


        private void PropertyContextMenu(object obj, PropertyInfo propinfo)
        {
            if (ImGui.BeginPopupContextItem(propinfo.Name))
            {
                if (ImGui.Selectable($@"Search"))
                {
                    EditorCommandQueue.AddCommand($@"map/propsearch/{propinfo.Name}");
                }
                ImGui.EndPopup();
            }
        }
        private bool ParamRefRow(PropertyInfo propinfo, object oldval, ref object newObj)
        {
            var att = propinfo.GetCustomAttribute<MSBParamReference>();
            if (att != null)
            {
                ImGui.NextColumn();
                List<string> refs = new List<string>();
                refs.Add(att.ParamName);
                Editor.EditorDecorations.ParamRefText(refs);
                ImGui.NextColumn();
                var id = oldval; //oldval cannot always be casted to int
                Editor.EditorDecorations.ParamRefsSelectables(refs, id);
                return Editor.EditorDecorations.ParamRefEnumContextMenu(id, ref newObj, refs, null);
            }
            return false;
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

        internal enum RegionShape
        {
            Point,
            Sphere,
            Cylinder,
            Box,
            Composite,
        }

        private string[] _regionShapes =
        {
            "Point",
            "Sphere",
            "Cylinder",
            "Box",
            "Composite",
        };

        private void PropEditorGeneric(Selection selection, Entity entSelection, object target = null, bool decorate = true)
        {
            var obj = (target == null) ? entSelection.WrappedObject : target;
            var type = obj.GetType();
            if (!_propCache.ContainsKey(type.FullName))
            {
                _propCache.Add(type.FullName, type.GetProperties(BindingFlags.Instance | BindingFlags.Public));
            }
            var properties = _propCache[type.FullName];
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
                PropEditorFlverLayout(entSelection, (FLVER2.BufferLayout)obj);
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
                    // ImGui.AlignTextToFramePadding();
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
                                    PropEditorGeneric(selection, entSelection, o, false);
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
                                PropertyContextMenu(obj, prop);
                                if (ImGui.IsItemActive() && !ImGui.IsWindowFocused())
                                {
                                    ImGui.SetItemDefaultFocus();
                                }
                                bool committed = ImGui.IsItemDeactivatedAfterEdit();
                                //bool committed = true;
                                UpdateProperty(prop, entSelection, obj, newval, changed, committed, shouldUpdateVisual, false, i);

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
                                    PropEditorGeneric(selection, entSelection, o, false);
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
                                if (ParamRefRow(prop, oldval, ref newval))
                                {
                                    changed = true;
                                    committed = true;
                                    //return tuple?
                                }
                                //bool committed = true;
                                UpdateProperty(prop, entSelection, obj, newval, changed, committed, shouldUpdateVisual, false, i);

                                ImGui.NextColumn();
                                ImGui.PopID();
                            }
                        }
                        ImGui.PopID();
                    }
                    // TODO: find a better place to handle this special case (maybe)
                    else if (typ.IsClass && typ == typeof(MSB.Shape))
                    {
                        bool open = ImGui.TreeNodeEx(prop.Name, ImGuiTreeNodeFlags.DefaultOpen);
                        ImGui.NextColumn();
                        ImGui.SetNextItemWidth(-1);
                        var o = prop.GetValue(obj);
                        var shapetype = Enum.Parse<RegionShape>(o.GetType().Name);
                        int shap = (int)shapetype;
                        if (ImGui.Combo("##shapecombo", ref shap, _regionShapes, _regionShapes.Length))
                        {
                            MSB.Shape newshape;
                            switch ((RegionShape)shap)
                            {
                                case RegionShape.Box:
                                    newshape = new MSB.Shape.Box();
                                    break;
                                case RegionShape.Point:
                                    newshape = new MSB.Shape.Point();
                                    break;
                                case RegionShape.Cylinder:
                                    newshape = new MSB.Shape.Cylinder();
                                    break;
                                case RegionShape.Sphere:
                                    newshape = new MSB.Shape.Sphere();
                                    break;
                                case RegionShape.Composite:
                                    newshape = new MSB.Shape.Composite();
                                    break;
                                default:
                                    throw new Exception("Invalid shape");
                            }
                            //UpdateProperty(prop, selection, obj, newshape, true, true, true, true);

                            var action = new PropertiesChangedAction((PropertyInfo)prop, obj, newshape);
                            action.SetPostExecutionAction((undo) =>
                            {
                                bool selected = false;
                                if (entSelection.RenderSceneMesh != null)
                                {
                                    selected = entSelection.RenderSceneMesh.RenderSelectionOutline;
                                    entSelection.RenderSceneMesh.Dispose();
                                    entSelection.RenderSceneMesh = null;
                                }

                                entSelection.UpdateRenderModel();
                                entSelection.RenderSceneMesh.RenderSelectionOutline = selected;
                            });

                            ContextActionManager.ExecuteAction(action);
                        }
                        ImGui.NextColumn();
                        if (open)
                        {
                            PropEditorGeneric(selection, entSelection, o, false);
                            ImGui.TreePop();
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
                            PropEditorGeneric(selection, entSelection, o, false);
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

                        changed = PropertyRow(typ, oldval, out newval, entSelection, prop.Name);
                        PropertyContextMenu(obj, prop);
                        if (ImGui.IsItemActive() && !ImGui.IsWindowFocused())
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                        bool committed = ImGui.IsItemDeactivatedAfterEdit();
                        if (ParamRefRow(prop, oldval, ref newval))
                        {
                            changed = true;
                            committed = true;
                        }
                        //bool committed = true;
                        UpdateProperty(prop, entSelection, obj, newval, changed, committed, shouldUpdateVisual, false);

                        ImGui.NextColumn();
                        ImGui.PopID();
                    }
                    id++;
                }
            }
            if (decorate)
            {
                ImGui.Columns(1);
                if (entSelection.References != null)
                {
                    ImGui.NewLine();
                    ImGui.Text("References: ");
                    ImGui.Indent(10);
                    foreach (var m in entSelection.References)
                    {
                        foreach (var n in m.Value)
                        {
                            if (n is Entity e)
                            {
                                var nameWithType = e.PrettyName.Insert(2, e.WrappedObject.GetType().Name + " - ");
                                if (ImGui.Button(nameWithType))
                                {
                                    selection.ClearSelection();
                                    selection.AddSelection(e);
                                }
                            }
                            else if (n is ObjectContainerReference r)
                            {
                                // Try to select the map's RootObject if it is loaded, and the reference otherwise.
                                // It's not the end of the world if we choose the wrong one, as SceneTree can use either,
                                // but only the RootObject has the TransformNode and Viewport integration.
                                string mapid = r.Name;
                                string prettyName = $"{ForkAwesome.Cube} {mapid}";
                                if (Editor.AliasBank.MapNames != null && Editor.AliasBank.MapNames.TryGetValue(mapid, out string metaName))
                                {
                                    prettyName += $" <{metaName.Replace("--", "")}>";
                                }
                                if (ImGui.Button(prettyName))
                                {
                                    Scene.ISelectable rootTarget = r.GetSelectionTarget();
                                    selection.ClearSelection();
                                    selection.AddSelection(rootTarget);
                                    // For this type of connection, jump to the object in the list to actually load the map
                                    // (is this desirable in other cases?). It could be possible to have a Load context menu
                                    // here, but that should be shared with SceneTree.
                                    selection.GotoTreeTarget = rootTarget;
                                }
                            }
                        }
                    }
                }
                ImGui.Unindent(10);
                ImGui.NewLine();
                ImGui.Text("Objects referencing this object:");
                ImGui.Indent(10);
                foreach (var m in entSelection.GetReferencingObjects())
                {
                    var nameWithType = m.PrettyName.Insert(2, m.WrappedObject.GetType().Name + " - ");
                    if (ImGui.Button(nameWithType))
                    {
                        selection.ClearSelection();
                        selection.AddSelection(m);
                    }
                }
                ImGui.Unindent(10);
            }
        }

        public void OnGui(Selection selection, Entity entSelection, string id, float w, float h)
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.145f, 0.145f, 0.149f, 1.0f));
            ImGui.SetNextWindowSize(new Vector2(350, h - 80), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(w - 370, 20), ImGuiCond.FirstUseEver);
            ImGui.Begin($@"Properties##{id}");
            ImGui.BeginChild("propedit");
            if (entSelection == null || entSelection.WrappedObject == null)
            {
                if (selection.IsMultiSelection())
                {
                    ImGui.Text("Select a single object to edit properties.");
                }
                ImGui.EndChild();
                ImGui.End();
                ImGui.PopStyleColor();
                return;
            }
            if (entSelection.WrappedObject is Param.Row prow || entSelection.WrappedObject is MergedParamRow)
            {
                PropEditorParamRow(entSelection);
            }
            else
            {
                PropEditorGeneric(selection, entSelection);
            }
            ImGui.EndChild();
            ImGui.End();
            ImGui.PopStyleColor();
        }
    }
}