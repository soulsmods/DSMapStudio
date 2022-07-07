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
using StudioCore;
using StudioCore.Editor;

namespace StudioCore.ParamEditor
{
    public class PropertyEditor
    {
        public ActionManager ContextActionManager;
        private ParamEditorScreen _paramEditor;

        private Dictionary<string, PropertyInfo[]> _propCache = new Dictionary<string, PropertyInfo[]>();

        public PropertyEditor(ActionManager manager, ParamEditorScreen paramEditorScreen)
        {
            ContextActionManager = manager;
            _paramEditor = paramEditorScreen;
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
            bool changed, bool committed, int arrayindex = -1)
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

        public void PropEditorParamRow(PARAM.Row row, PARAM.Row vrow, ref string propSearchString, string activeParam)
        {
            ParamMetaData meta = ParamMetaData.Get(row.Def);
            ImGui.Columns(ParamEditorScreen.ShowVanillaParamsPreference?3:2);
            ImGui.Separator();
            int id = 0;

            if (propSearchString != null)
                ImGui.InputText("Search...", ref propSearchString, 255);
            Regex propSearchRx = null;
            try
            {
                propSearchRx = new Regex(propSearchString.ToLower());
            }
            catch
            {
            }
            ImGui.NextColumn();
            ImGui.NextColumn();
            if (ParamEditorScreen.ShowVanillaParamsPreference)
                ImGui.NextColumn();

            // This should be rewritten somehow it's super ugly
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
            var nameProp = row.GetType().GetProperty("Name");
            var idProp = row.GetType().GetProperty("ID");
            PropEditorPropInfoRow(row, nameProp, "Name", ref id, propSearchRx);
            PropEditorPropInfoRow(row, idProp, "ID", ref id, propSearchRx);
            ImGui.PopStyleColor();
            ImGui.Separator();

            List<string> pinnedFields = new List<string>(_paramEditor._projectSettings.PinnedFields.GetValueOrDefault(activeParam, new List<string>()));
            foreach (var field in pinnedFields)
            {
                List<PARAM.Cell> matches = row.Cells.Where(cell => cell.Def.InternalName == field).ToList();
                List<PARAM.Cell> vmatches = vrow==null ? null : vrow.Cells.Where(cell => cell.Def.InternalName == field).ToList();
                for (int i=0; i<matches.Count; i++)
                    PropEditorPropCellRow(matches[i], vrow==null ? null : vmatches[i], ref id, propSearchRx, activeParam, true);
            }
            if (pinnedFields.Count != 0)
                ImGui.Separator();
            List<string> fieldOrder = meta != null && meta.AlternateOrder != null && ParamEditorScreen.AllowFieldReorderPreference ? meta.AlternateOrder : new List<string>();
            foreach (PARAMDEF.Field field in row.Def.Fields)
            {
                if (!fieldOrder.Contains(field.InternalName))
                    fieldOrder.Add(field.InternalName);
            }
            foreach (var field in fieldOrder)
            {
                if (field.Equals("-"))
                {
                    ImGui.Separator();
                    continue;
                }
                if (row[field] == null)
                    continue;
                List<PARAM.Cell> matches = row.Cells.Where(cell => cell.Def.InternalName == field).ToList();
                List<PARAM.Cell> vmatches = vrow==null ? null : vrow.Cells.Where(cell => cell.Def.InternalName == field).ToList();
                for (int i=0; i<matches.Count; i++)
                    PropEditorPropCellRow(matches[i], vrow==null ? null : vmatches[i], ref id, propSearchRx, activeParam, false);
            }
            ImGui.Columns(1);
        }
        
        // Many parameter options, which may be simplified.
        private void PropEditorPropInfoRow(PARAM.Row row, PropertyInfo prop, string visualName, ref int id, Regex propSearchRx)
        {
            PropEditorPropRow(prop.GetValue(row), null, ref id, visualName, null, prop.PropertyType, prop, null, row, propSearchRx, null, false);
        }
        private void PropEditorPropCellRow(PARAM.Cell cell, PARAM.Cell vcell, ref int id, Regex propSearchRx, string activeParam, bool isPinned)
        {
            PropEditorPropRow(cell.Value, vcell==null?null:vcell.Value, ref id, cell.Def.InternalName, FieldMetaData.Get(cell.Def), cell.Value.GetType(), cell.GetType().GetProperty("Value"), cell, null, propSearchRx, activeParam, isPinned);
        }
        private void PropEditorPropRow(object oldval, object vanillaval, ref int id, string internalName, FieldMetaData cellMeta, Type propType, PropertyInfo proprow, PARAM.Cell nullableCell, PARAM.Row nullableRow, Regex propSearchRx, string activeParam, bool isPinned)
        {
            List<string> RefTypes = cellMeta == null ? null : cellMeta.RefTypes;
            string VirtualRef = cellMeta == null ? null : cellMeta.VirtualRef;
            ParamEnum Enum = cellMeta == null ? null : cellMeta.EnumType;
            string Wiki = cellMeta == null ? null : cellMeta.Wiki;
            bool IsBool = cellMeta == null ? false : cellMeta.IsBool;
            string AltName = cellMeta == null ? null : cellMeta.AltName;

            if (propSearchRx != null)
            {
                if (!propSearchRx.IsMatch(internalName.ToLower()) && !(AltName != null && propSearchRx.IsMatch(AltName.ToLower())))
                {
                    return;
                }
            }

            object newval = null;
            ImGui.PushID(id);
            ImGui.AlignTextToFramePadding();
            PropertyRowName(ref internalName, cellMeta);
            PropertyRowNameContextMenu(internalName, cellMeta, activeParam, activeParam!=null, isPinned);
            if (Wiki != null)
            {
                if (EditorDecorations.HelpIcon(internalName, ref Wiki, true))
                    cellMeta.Wiki = Wiki;
            }

            EditorDecorations.ParamRefText(RefTypes);
            EditorDecorations.EnumNameText(Enum == null ? null : Enum.name);

            //PropertyRowMetaDefContextMenu();
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);
            bool changed = false;

            bool diffVanilla = vanillaval != null && !oldval.Equals(vanillaval);
            bool matchDefault = nullableCell != null && nullableCell.Def.Default!=null && nullableCell.Def.Default.Equals(oldval);
            bool isRef = (ParamEditorScreen.HideReferenceRowsPreference == false && RefTypes != null) || (ParamEditorScreen.HideEnumsPreference == false && Enum != null) || VirtualRef != null;
            if (isRef)
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 1.0f, 1.0f));
            else if (matchDefault)
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 1.0f));
            else if (diffVanilla)
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.22f, 0.2f, 1f));
            changed = PropertyRow(propType, oldval, out newval, IsBool);
            bool committed = ImGui.IsItemDeactivatedAfterEdit();
            if (isRef || matchDefault) //if diffVanilla, remove styling later
                ImGui.PopStyleColor();

            PropertyRowValueContextMenu(internalName, VirtualRef, oldval);

            if (ParamEditorScreen.HideReferenceRowsPreference == false && RefTypes != null)
                EditorDecorations.ParamRefsSelectables(RefTypes, oldval);
            if (ParamEditorScreen.HideEnumsPreference == false && Enum != null)
                EditorDecorations.EnumValueText(Enum.values, oldval.ToString());

            if (ParamEditorScreen.HideReferenceRowsPreference == false || ParamEditorScreen.HideEnumsPreference == false)
            {
                if (EditorDecorations.ParamRefEnumContextMenu(oldval, ref newval, RefTypes, Enum))
                {
                    changed = true;
                    committed = true;
                }
            }
            if (ParamEditorScreen.ShowVanillaParamsPreference)
            {
                ImGui.NextColumn();
                if (vanillaval==null)
                    ImGui.TextUnformatted("");
                else
                {
                    ImGui.TextUnformatted(vanillaval.ToString());
                    if (ParamEditorScreen.HideReferenceRowsPreference == false && RefTypes != null)
                        EditorDecorations.ParamRefsSelectables(RefTypes, vanillaval);
                    if (ParamEditorScreen.HideEnumsPreference == false && Enum != null)
                        EditorDecorations.EnumValueText(Enum.values, vanillaval.ToString());
                }
            }

            UpdateProperty(proprow, nullableCell != null ? (object)nullableCell : nullableRow, newval, changed, committed);
            if (diffVanilla && !(isRef || matchDefault)) //and not already removed
                ImGui.PopStyleColor();
            ImGui.NextColumn();
            ImGui.PopID();
            id++;
        }

        private void PropertyRowName(ref string internalName, FieldMetaData cellMeta)
        {
            string AltName = cellMeta == null ? null : cellMeta.AltName;
            if (cellMeta != null && ParamEditorScreen.EditorMode)
            {
                string EditName = AltName == null ? internalName : AltName;
                ImGui.InputText("", ref EditName, 128);
                if (EditName.Equals(internalName) || EditName.Equals(""))
                    cellMeta.AltName = null;
                else
                    cellMeta.AltName = EditName;
            }
            else
            {
                string printedName = (AltName != null && ParamEditorScreen.ShowAltNamesPreference) ? (ParamEditorScreen.AlwaysShowOriginalNamePreference ? $"{internalName} ({AltName})" : $"{AltName}*") : internalName;
                ImGui.TextUnformatted(printedName);
            }
        }

        private void PropertyRowNameContextMenu(string originalName, FieldMetaData cellMeta, string activeParam, bool showPinOptions, bool isPinned)
        {
            if (ImGui.BeginPopupContextItem("rowName"))
            {
                if (ParamEditorScreen.ShowAltNamesPreference == true && ParamEditorScreen.AlwaysShowOriginalNamePreference == false)
                    ImGui.Text(originalName);
                if (ImGui.Selectable("Search..."))
                    EditorCommandQueue.AddCommand($@"param/search/prop {originalName.Replace(" ", "\\s")} ");
                if (showPinOptions && ImGui.Selectable((isPinned?"Unpin ":"Pin "+ originalName)))
                {
                    if(!_paramEditor._projectSettings.PinnedFields.ContainsKey(activeParam))
                        _paramEditor._projectSettings.PinnedFields.Add(activeParam, new List<string>());
                    List<string> pinned = _paramEditor._projectSettings.PinnedFields[activeParam];
                    if (isPinned)
                        pinned.Remove(originalName);
                    else if (!pinned.Contains(originalName))
                        pinned.Add(originalName);
                }
                if (ParamEditorScreen.EditorMode && cellMeta != null)
                {
                    if (ImGui.BeginMenu("Add Reference"))
                    {
                        foreach (string p in ParamBank.Params.Keys)
                        {
                            if (ImGui.Selectable(p))
                            {
                                if (cellMeta.RefTypes == null)
                                    cellMeta.RefTypes = new List<string>();
                                cellMeta.RefTypes.Add(p);
                            }
                        }
                        ImGui.EndMenu();
                    }
                    if (cellMeta.RefTypes != null && ImGui.BeginMenu("Remove Reference"))
                    {
                        foreach (string p in cellMeta.RefTypes)
                        {
                            if (ImGui.Selectable(p))
                            {
                                cellMeta.RefTypes.Remove(p);
                                if (cellMeta.RefTypes.Count == 0)
                                    cellMeta.RefTypes = null;
                                break;
                            }
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.Selectable(cellMeta.IsBool ? "Remove bool toggle" : "Add bool toggle"))
                        cellMeta.IsBool = !cellMeta.IsBool;
                    if (cellMeta.Wiki == null && ImGui.Selectable("Add wiki..."))
                        cellMeta.Wiki = "Empty wiki...";
                    if (cellMeta.Wiki != null && ImGui.Selectable("Remove wiki"))
                        cellMeta.Wiki = null;
                }
                ImGui.EndPopup();
            }
        }
        private void PropertyRowValueContextMenu(string internalName, string VirtualRef, dynamic oldval)
        {
            if (ImGui.BeginPopupContextItem("quickMEdit"))
            {
                if (ImGui.Selectable("Edit all selected..."))
                {
                    EditorCommandQueue.AddCommand($@"param/menu/massEditRegex/selection: {Regex.Escape(internalName)}: ");
                }
                if (VirtualRef != null)
                    EditorDecorations.VirtualParamRefSelectables(VirtualRef, oldval);
                if (ParamEditorScreen.EditorMode && ImGui.BeginMenu("Find rows with this value..."))
                {
                    foreach(KeyValuePair<string, PARAM> p in ParamBank.Params)
                    {
                        int v = (int)oldval;
                        PARAM.Row r = p.Value[v];
                        if (r != null && ImGui.Selectable($@"{p.Key}: {Utils.ImGuiEscape(r.Name, "null")}"))
                            EditorCommandQueue.AddCommand($@"param/select/-1/{p.Key}/{v}");
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndPopup();
            }
        }
    }
}
