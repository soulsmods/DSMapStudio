using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private object _editedPropCache;
        private object _editedTypeCache;
        private object _editedObjCache;

        private unsafe (bool, bool) PropertyRow(Type typ, object oldval, ref object newval, bool isBool)
        {
            bool isChanged = false;
            bool isDeactivatedAfterEdit = false;
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
                        isChanged = true;
                    }
                    isDeactivatedAfterEdit = ImGui.IsItemDeactivatedAfterEdit();
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
                        _editedPropCache = newval;
                        isChanged = true;
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
                    isChanged = true;
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
                        isChanged = true;
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
                    isChanged = true;
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
                        isChanged = true;
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
                    isChanged = true;
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
                        isChanged = true;
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
                    isChanged = true;
                }
            }
            else if (typ == typeof(float))
            {
                float val = (float)oldval;
                if (ImGui.InputFloat("##value", ref val, 0.1f))
                {
                    newval = val;
                    _editedPropCache = newval;
                    isChanged = true;
                }
            }
            else if (typ == typeof(double))
            {
                double val = (double)oldval;
                if (ImGui.InputScalar("##value", ImGuiDataType.Double, new IntPtr(&val)))
                {
                    newval = val;
                    _editedPropCache = newval;
                    isChanged = true;
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
                    isChanged = true;
                }
            }
            else if (typ == typeof(Vector2))
            {
                Vector2 val = (Vector2)oldval;
                if (ImGui.InputFloat2("##value", ref val))
                {
                    newval = val;
                    _editedPropCache = newval;
                    isChanged = true;
                }
            }
            else if (typ == typeof(Vector3))
            {
                Vector3 val = (Vector3)oldval;
                if (ImGui.InputFloat3("##value", ref val))
                {
                    newval = val;
                    _editedPropCache = newval;
                    isChanged = true;
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
                        isChanged = true;
                    }
                }
            }
            else
            {
                // Using InputText means IsItemDeactivatedAfterEdit doesn't pick up random previous item
                string implMe = "ImplementMe";
                ImGui.InputText(null, ref implMe, 256, ImGuiInputTextFlags.ReadOnly);
            }
            isDeactivatedAfterEdit |= ImGui.IsItemDeactivatedAfterEdit();

            return (isChanged, isDeactivatedAfterEdit);
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
                if (newval == null)
                {
                    // Safety check warned to user, should have proper crash handler instead
                    TaskManager.warningList["ParamRowEditorPropertyChangeError"] = "ParamRowEditor: Property changed was null";
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
                ContextActionManager.ExecuteAction(action);
            }
        }

        public void PropEditorParamRow(ParamBank bank, Param.Row row, Param.Row vrow, List<(string, Param.Row)> auxRows, Param.Row crow, ref string propSearchString, string activeParam, bool isActiveView)
        {
            ParamMetaData meta = ParamMetaData.Get(row.Def);
            int id = 0;
            bool showParamCompare = auxRows.Count > 0;
            bool showRowCompare = crow != null;

            if (propSearchString != null)
            {
                if (isActiveView && InputTracker.GetKeyDown(KeyBindings.Current.Param_SearchField))
                    ImGui.SetKeyboardFocusHere();
                ImGui.InputText($"Search <{KeyBindings.Current.Param_SearchField.HintText}>", ref propSearchString, 255);
                if (ImGui.IsItemEdited())
                    CacheBank.ClearCaches();
                string resAutoCol = AutoFill.ColumnSearchBarAutoFill();
                if (resAutoCol != null)
                {
                    propSearchString = resAutoCol;
                    CacheBank.ClearCaches();
                }
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
            }

            ImGui.BeginChild("Param Fields");
            int columnCount = 2;
            if (CFG.Current.Param_ShowVanillaParams)
                columnCount++;
            if (showRowCompare)
                columnCount++;
            if (showParamCompare)
                columnCount += auxRows.Count;
            ImGui.Columns(columnCount);
            ImGui.NextColumn();
            if (showParamCompare)
                ImGui.Text("Current");
            ImGui.NextColumn();
            if (CFG.Current.Param_ShowVanillaParams)
            {
                if (showParamCompare)
                    ImGui.Text("Vanilla");
                ImGui.NextColumn();
            }
            foreach ((string name, Param.Row r) in auxRows)
            {
                if (showParamCompare)
                    ImGui.Text(name);
                ImGui.NextColumn();
            }
            if (showRowCompare)
                ImGui.NextColumn();

            // This should be rewritten somehow it's super ugly
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 1.0f, 1.0f));
            var nameProp = row.GetType().GetProperty("Name");
            var idProp = row.GetType().GetProperty("ID");
            PropEditorPropInfoRow(bank, row, vrow, auxRows, crow, nameProp, "Name", ref id, activeParam);
            PropEditorPropInfoRow(bank, row, vrow, auxRows, crow, idProp, "ID", ref id, activeParam);
            ImGui.PopStyleColor();
            ImGui.Spacing();
            ImGui.Separator();

            string search = propSearchString;
            List<(PseudoColumn, Param.Column)> cols = CacheBank.GetCached(_paramEditor, row, "fieldFilter", () => CellSearchEngine.cse.Search((activeParam, row), search, true, true));
            List<(PseudoColumn, Param.Column)> vcols = CacheBank.GetCached(_paramEditor, vrow, "vFieldFilter", () => cols.Select((x, i) => x.GetAs(ParamBank.VanillaBank.GetParamFromName(activeParam))).ToList());
            List<List<(PseudoColumn, Param.Column)>> auxCols = CacheBank.GetCached(_paramEditor, auxRows, "auxFieldFilter", () => auxRows.Select((r, i) => cols.Select((c, j) => c.GetAs(ParamBank.AuxBanks[r.Item1].GetParamFromName(activeParam))).ToList()).ToList());

            List<string> pinnedFields = new List<string>(_paramEditor._projectSettings.PinnedFields.GetValueOrDefault(activeParam, new List<string>()));
            if (pinnedFields.Count > 0)
            {
                foreach (var field in pinnedFields)
                {
                    List<(PseudoColumn, Param.Column)> matches = cols.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList();
                    List<(PseudoColumn, Param.Column)> vmatches = vcols.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList();
                    List<List<(PseudoColumn, Param.Column)>> auxMatches = auxCols.Select((aux, i) => aux.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList()).ToList();
                    for (int i = 0; i < matches.Count; i++)
                    {
                        PropEditorPropCellRow(bank,
                        row,
                        crow,
                        matches[i],
                        vrow,
                        vmatches.Count > i ? vmatches[i] : (PseudoColumn.None, null),
                        auxRows,
                        auxMatches.Select((x, j) => x.Count > i ? x[i] : (PseudoColumn.None, null)).ToList(),
                        matches[i].Item2?.GetByteOffset().ToString("x"),
                        ref id, activeParam, true);
                    }
                }
                ImGui.Separator();
            }
            List<string> fieldOrder = meta != null && meta.AlternateOrder != null && CFG.Current.Param_AllowFieldReorder ? meta.AlternateOrder : new List<string>();
            foreach (PARAMDEF.Field field in row.Def.Fields)
            {
                if (!fieldOrder.Contains(field.InternalName))
                    fieldOrder.Add(field.InternalName);
            }
            bool lastRowExists = false;
            foreach (var field in fieldOrder)
            {
                if (field.Equals("-") && lastRowExists)
                {
                    ImGui.Separator();
                    lastRowExists = false;
                    continue;
                }
                if (row[field] == null)
                    continue;
                List<(PseudoColumn, Param.Column)> matches = cols.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList();
                List<(PseudoColumn, Param.Column)> vmatches = vcols.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList();
                List<List<(PseudoColumn, Param.Column)>> auxMatches = auxCols.Select((aux, i) => aux.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList()).ToList();
                for (int i = 0; i < matches.Count; i++)
                {
                    lastRowExists |= PropEditorPropCellRow(bank,
                    row,
                    crow,
                    matches[i],
                    vrow,
                    vmatches.Count > i ? vmatches[i] : (PseudoColumn.None, null),
                    auxRows,
                    auxMatches.Select((x, j) => x.Count > i ? x[i] : (PseudoColumn.None, null)).ToList(),
                    matches[i].Item2?.GetByteOffset().ToString("x"),
                    ref id, activeParam, false);
                }
            }
            ImGui.Columns(1);
            if (meta.CalcCorrectDef != null)
            {
                DrawCalcCorrectGraph(meta, row);
            }
            ImGui.EndChild();
        }

        private void DrawCalcCorrectGraph(ParamMetaData meta, Param.Row row)
        {
            try
            {
                ImGui.Separator();
                ImGui.NewLine();
                var ccd = meta.CalcCorrectDef;
                (float[] values, int xOffset, float minY, float maxY) = CacheBank.GetCached(_paramEditor, row, "calcCorrectData", () => getCalcCorrectedData(ccd, row));
                ImGui.PlotLines("##graph", ref values[0], values.Length, 0, xOffset == 0 ? "" : $@"Note: add {xOffset} to x coordinate", minY, maxY, new Vector2(ImGui.GetColumnWidth(-1), ImGui.GetColumnWidth(-1)*0.5625f));
            }
            catch (Exception e)
            {
                ImGui.TextUnformatted("Unable to draw graph");
            }
        }
        private (float[], int, float, float) getCalcCorrectedData(CalcCorrectDefinition ccd, Param.Row row)
        {
            float[] stageMaxVal = ccd.stageMaxVal.Select((x, i) => (float)row[x].Value.Value).ToArray();
            float[] stageMaxGrowVal = ccd.stageMaxGrowVal.Select((x, i) => (float)row[x].Value.Value).ToArray();
            float[] adjPoint_maxGrowVal = ccd.adjPoint_maxGrowVal.Select((x, i) => (float)row[x].Value.Value).ToArray();

            int length = (int)(stageMaxVal[stageMaxVal.Length-1] - stageMaxVal[0] + 1);
            if (length <= 0 || length > 1000)
                return (new float[0], 0, 0, 0);
            float[] values = new float[length];
            for (int i=0; i<values.Length; i++)
            {
                float baseVal = i + stageMaxVal[0];
                int band = 0;
                while (band + 1 < stageMaxVal.Length && stageMaxVal[band + 1] < baseVal)
                    band++;
                if (band + 1 >= stageMaxVal.Length)
                    values[i] = stageMaxGrowVal[stageMaxGrowVal.Length-1];
                else
                {
                    float adjValRate = stageMaxVal[band] == stageMaxVal[band+1] ? 0 : (baseVal - stageMaxVal[band]) / (stageMaxVal[band+1] - stageMaxVal[band]);
                    float adjGrowValRate = adjPoint_maxGrowVal[band] >= 0 ? (float)Math.Pow(adjValRate, adjPoint_maxGrowVal[band]) : 1 - (float)Math.Pow(1 - adjValRate, -adjPoint_maxGrowVal[band]);
                    values[i] = adjGrowValRate * (stageMaxGrowVal[band+1] - stageMaxGrowVal[band]) + stageMaxGrowVal[band];
                }
            }
            return (values, (int)stageMaxVal[0], stageMaxGrowVal[0], stageMaxGrowVal[stageMaxGrowVal.Length-1]);
        }

        // Many parameter options, which may be simplified.
        private void PropEditorPropInfoRow(ParamBank bank, Param.Row row, Param.Row vrow, List<(string, Param.Row)> auxRows, Param.Row crow, PropertyInfo prop, string visualName, ref int id, string activeParam)
        {
            PropEditorPropRow(
                bank,
                prop.GetValue(row),
                crow != null ? prop.GetValue(crow) : null,
                vrow != null ? prop.GetValue(vrow) : null,
                auxRows.Select((r, i) => r.Item2 != null ? prop.GetValue(r.Item2) : null).ToList(),
                ref id,
                "header",
                visualName,
                null,
                prop.PropertyType,
                prop,
                null,
                row,
                null,
                false);
        }
        private bool PropEditorPropCellRow(ParamBank bank, Param.Row row, Param.Row crow, (PseudoColumn, Param.Column) col, Param.Row vrow, (PseudoColumn, Param.Column) vcol, List<(string, Param.Row)> auxRows, List<(PseudoColumn, Param.Column)> auxCols, string fieldOffset, ref int id, string activeParam, bool isPinned)
        {
            return PropEditorPropRow(
                bank,
                row.Get(col),
                crow?.Get(col),
                vcol.IsColumnValid() ? vrow?.Get(vcol) : null,
                auxRows.Select((r, i) => auxCols[i].IsColumnValid() ? r.Item2?.Get(auxCols[i]) : null).ToList(),
                ref id,
                fieldOffset != null ? "0x" + fieldOffset : null, col.Item2.Def.InternalName,
                FieldMetaData.Get(col.Item2.Def),
                col.GetColumnType(),
                typeof(Param.Cell).GetProperty("Value"),
                row[col.Item2],
                row,
                activeParam,
                isPinned);
        }
        private bool PropEditorPropRow(ParamBank bank, object oldval, object compareval, object vanillaval, List<object> auxVals, ref int id, string fieldOffset, string internalName, FieldMetaData cellMeta, Type propType, PropertyInfo proprow, Param.Cell? nullableCell, Param.Row row, string activeParam, bool isPinned)
        {
            List<ParamRef> RefTypes = cellMeta?.RefTypes;
            string VirtualRef = cellMeta?.VirtualRef;
            string FmgRef = cellMeta?.FmgRef;
            ParamEnum Enum = cellMeta?.EnumType;
            string Wiki = cellMeta?.Wiki;
            bool IsBool = cellMeta?.IsBool ?? false;

            object newval = null;

            ImGui.PushID(id);
            ImGui.AlignTextToFramePadding();
            if (Wiki != null)
            {
                if (EditorDecorations.HelpIcon(internalName, ref Wiki, true))
                {
                    cellMeta.Wiki = Wiki;
                }
                ImGui.SameLine();
            }
            else
            {
                ImGui.Text(" ");
                ImGui.SameLine();
            }
            PropertyRowName(fieldOffset, ref internalName, cellMeta);
            PropertyRowNameContextMenu(bank, internalName, cellMeta, activeParam, activeParam != null, isPinned);

            EditorDecorations.ParamRefText(RefTypes, row);
            EditorDecorations.FmgRefText(FmgRef);
            EditorDecorations.EnumNameText(Enum == null ? null : Enum.name);

            //PropertyRowMetaDefContextMenu();
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);
            bool changed = false;
            bool committed = false;

            bool diffVanilla = ParamUtils.IsValueDiff(ref oldval, ref vanillaval, propType);
            bool diffCompare = ParamUtils.IsValueDiff(ref oldval, ref compareval, propType);
            List<bool> diffAuxVanilla = auxVals.Select((o, i) => ParamUtils.IsValueDiff(ref o, ref vanillaval, propType)).ToList();
            List<bool> diffAuxPrimaryAndVanilla = auxVals.Select((o, i) => ParamUtils.IsValueDiff(ref o, ref oldval, propType) && ParamUtils.IsValueDiff(ref o, ref vanillaval, propType)).ToList();
            bool conflict = diffVanilla && diffAuxPrimaryAndVanilla.Contains(true);

            bool matchDefault = nullableCell?.Def.Default != null && nullableCell.Value.Def.Default.Equals(oldval);
            bool isRef = (CFG.Current.Param_HideReferenceRows == false && (RefTypes != null || FmgRef != null)) || (CFG.Current.Param_HideEnums == false && Enum != null) || VirtualRef != null;
            if (conflict)
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.25f, 0.2f, 0.2f, 1.0f));
            else if (diffVanilla)
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.22f, 0.2f, 1.0f));
            if (isRef)
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 1.0f, 1.0f));
            else if (matchDefault)
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 1.0f));
            (changed, committed) = PropertyRow(propType, oldval, ref newval, IsBool);

            if (isRef || matchDefault) //if diffVanilla, remove styling later
                ImGui.PopStyleColor();

            PropertyRowValueContextMenu(bank, internalName, VirtualRef, oldval);

            if (CFG.Current.Param_HideReferenceRows == false && RefTypes != null)
                EditorDecorations.ParamRefsSelectables(bank, RefTypes, row, oldval);
            if (CFG.Current.Param_HideReferenceRows == false && FmgRef != null)
                EditorDecorations.FmgRefSelectable(FmgRef, oldval);
            if (CFG.Current.Param_HideEnums == false && Enum != null)
                EditorDecorations.EnumValueText(Enum.values, oldval.ToString());

            if (CFG.Current.Param_HideReferenceRows == false || CFG.Current.Param_HideEnums == false)
            {
                var fmgInfo = TextEditor.FMGBank.FmgInfoBank.Find((x) => x.Name == FmgRef);
                if (EditorDecorations.ParamRefEnumContextMenu(bank, oldval, ref newval, RefTypes, row, fmgInfo, Enum))
                {
                    changed = true;
                    committed = true;
                    _editedPropCache = newval;
                }
            }
            if (conflict || diffVanilla)
                ImGui.PopStyleColor();
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.180f, 0.180f, 0.196f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            if (conflict)
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.25f, 0.2f, 0.2f, 1.0f));
            if (CFG.Current.Param_ShowVanillaParams)
            {
                ImGui.NextColumn();
                AdditionalColumnValue(vanillaval, propType, bank, RefTypes, FmgRef, row, Enum, "vanilla");
            }
            if (auxVals.Count > 0)
            {
                for (int i=0; i<auxVals.Count; i++)
                {
                    if (!conflict && diffAuxVanilla[i])
                        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.2f, 0.236f, 1.0f));
                    ImGui.NextColumn();
                    AdditionalColumnValue(auxVals[i], propType, bank, RefTypes, FmgRef, row, Enum, i.ToString());
                    if (!conflict && diffAuxVanilla[i])
                        ImGui.PopStyleColor();
                }
                if (conflict)
                    ImGui.PopStyleColor();
            }
            if (compareval != null)
            {
                if(diffCompare)
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.2f, 0.236f, 1.0f));
                ImGui.NextColumn();
                AdditionalColumnValue(compareval, propType, bank, RefTypes, FmgRef, row, Enum, "compRow");
                if (diffCompare)
                    ImGui.PopStyleColor();
            }
            ImGui.PopStyleColor(2);


            if (changed)
            {
                _editedObjCache = nullableCell != null ? (object)nullableCell : row;
                _editedTypeCache = proprow;
            }
            else if (_editedPropCache != null && _editedPropCache != oldval)
            {
                changed = true;
            }

            UpdateProperty(_editedTypeCache, _editedObjCache, _editedPropCache, changed, committed);
            if (changed && committed && !ParamBank.VanillaBank.IsLoadingParams)
                ParamBank.PrimaryBank.RefreshParamRowVanillaDiff(row, activeParam);

            ImGui.NextColumn();
            ImGui.PopID();
            id++;
            return true;
        }

        private static void AdditionalColumnValue(object colVal, Type propType, ParamBank bank, List<ParamRef> RefTypes, string FmgRef, Param.Row context, ParamEnum Enum, string imguiSuffix)
        {
            if (colVal == null)
                ImGui.TextUnformatted("");
            else
            {
                string value;
                if (propType == typeof(byte[]))
                    value = ParamUtils.Dummy8Write((byte[])colVal);
                else
                    value = colVal.ToString();
                ImGui.InputText("##colval"+imguiSuffix, ref value, 256, ImGuiInputTextFlags.ReadOnly);
                if (CFG.Current.Param_HideReferenceRows == false && RefTypes != null)
                    EditorDecorations.ParamRefsSelectables(bank, RefTypes, context, colVal);
                if (CFG.Current.Param_HideReferenceRows == false && FmgRef != null)
                    EditorDecorations.FmgRefSelectable(FmgRef, colVal);
                if (CFG.Current.Param_HideEnums == false && Enum != null)
                    EditorDecorations.EnumValueText(Enum.values, colVal.ToString());
            }
        }

        private static void PropertyRowName(string fieldOffset, ref string internalName, FieldMetaData cellMeta)
        {
            string altName = cellMeta?.AltName;
            if (cellMeta != null && ParamEditorScreen.EditorMode)
            {
                string editName = !string.IsNullOrWhiteSpace(altName) ? altName : internalName;
                ImGui.InputText("##editName", ref editName, 128);
                if (editName.Equals(internalName) || editName.Equals(""))
                    cellMeta.AltName = null;
                else
                    cellMeta.AltName = editName;
            }
            else
            {
                string printedName = internalName;
                if (!string.IsNullOrWhiteSpace(altName))
                {
                    if (CFG.Current.Param_MakeMetaNamesPrimary)
                    {
                        printedName = altName;
                        if (CFG.Current.Param_ShowSecondaryNames)
                            printedName = $"{printedName} ({internalName})";
                    }
                    else if (CFG.Current.Param_ShowSecondaryNames) {
                            printedName = $"{printedName} ({altName})";
                    }
                }
                if (fieldOffset != null && CFG.Current.Param_ShowFieldOffsets)
                    printedName = $"{fieldOffset} {printedName}";

                ImGui.TextUnformatted(printedName);
            }
        }

        private void PropertyRowNameContextMenu(ParamBank bank, string internalName, FieldMetaData cellMeta, string activeParam, bool showPinOptions, bool isPinned)
        {
            float scale = ImGuiRenderer.GetUIScale();
            string altName = cellMeta?.AltName;
            string shownName = internalName;

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 10f) * scale);
            if (ImGui.BeginPopupContextItem("rowName"))
            {
                if (!string.IsNullOrWhiteSpace(altName))
                {
                    if (CFG.Current.Param_MakeMetaNamesPrimary)
                    {
                        shownName = altName;
                        ImGui.TextColored(new Vector4(1f, .7f, .4f, 1f), internalName);
                    }
                    else
                        ImGui.TextColored(new Vector4(1f, .7f, .4f, 1f), altName);
                    ImGui.Separator();
                }
                if (ImGui.MenuItem("Add to Searchbar"))
                {
                    EditorCommandQueue.AddCommand($@"param/search/prop {internalName.Replace(" ", "\\s")} ");
                }
                if (showPinOptions && ImGui.MenuItem((isPinned ? "Unpin " : "Pin " + shownName)))
                {
                    if (!_paramEditor._projectSettings.PinnedFields.ContainsKey(activeParam))
                        _paramEditor._projectSettings.PinnedFields.Add(activeParam, new List<string>());
                    List<string> pinned = _paramEditor._projectSettings.PinnedFields[activeParam];
                    if (isPinned)
                        pinned.Remove(internalName);
                    else if (!pinned.Contains(internalName))
                        pinned.Add(internalName);
                }
                if (ParamEditorScreen.EditorMode && cellMeta != null)
                {
                    if (ImGui.BeginMenu("Add Reference"))
                    {
                        foreach (string p in bank.Params.Keys)
                        {
                            if (ImGui.MenuItem(p+"##add"+p))
                            {
                                if (cellMeta.RefTypes == null)
                                    cellMeta.RefTypes = new List<ParamRef>();
                                cellMeta.RefTypes.Add(new ParamRef(p));
                            }
                        }
                        ImGui.EndMenu();
                    }
                    if (cellMeta.RefTypes != null && ImGui.BeginMenu("Remove Reference"))
                    {
                        foreach (ParamRef p in cellMeta.RefTypes)
                        {
                            if (ImGui.MenuItem(p.param+"##remove"+p.param))
                            {
                                cellMeta.RefTypes.Remove(p);
                                if (cellMeta.RefTypes.Count == 0)
                                    cellMeta.RefTypes = null;
                                break;
                            }
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.MenuItem(cellMeta.IsBool ? "Remove bool toggle" : "Add bool toggle"))
                        cellMeta.IsBool = !cellMeta.IsBool;
                    if (cellMeta.Wiki == null && ImGui.MenuItem("Add wiki..."))
                        cellMeta.Wiki = "Empty wiki...";
                    if (cellMeta.Wiki != null && ImGui.MenuItem("Remove wiki"))
                        cellMeta.Wiki = null;
                }
                ImGui.EndPopup();
            }
            ImGui.PopStyleVar();
        }
        private void PropertyRowValueContextMenu(ParamBank bank, string internalName, string VirtualRef, dynamic oldval)
        {
            bool onlyEditOptions = (VirtualRef == null && !ParamEditorScreen.EditorMode);
            if (ImGui.BeginPopupContextItem("quickMEdit"))
            {
                ImGui.TextColored(new Vector4(1.0f, 0.7f, 0.8f, 1.0f), "Param Field Context Menu");
                ImGui.SameLine(600);
                ImGui.Text("");
                if (ImGui.CollapsingHeader("Mass edit", ImGuiTreeNodeFlags.SpanFullWidth))
                {
                    ImGui.Separator();
                    if (ImGui.Selectable("Manually..."))
                    {
                        EditorCommandQueue.AddCommand($@"param/menu/massEditRegex/selection: {Regex.Escape(internalName)}: ");
                    }
                    if (ImGui.Selectable("Reset to vanilla..."))
                    {
                        EditorCommandQueue.AddCommand($@"param/menu/massEditRegex/selection: {Regex.Escape(internalName)}: = vanilla;");
                    }
                    ImGui.Separator();
                    string res = AutoFill.MassEditOpAutoFill();
                    if (res != null)
                    {
                        EditorCommandQueue.AddCommand($@"param/menu/massEditRegex/selection: {Regex.Escape(internalName)}: " + res);
                    }
                }
                if (VirtualRef != null)
                    EditorDecorations.VirtualParamRefSelectables(bank, VirtualRef, oldval);
                if (ImGui.Selectable("View distribution..."))
                {
                    EditorCommandQueue.AddCommand($@"param/menu/distributionPopup/{internalName}");
                }
                if (ParamEditorScreen.EditorMode && ImGui.BeginMenu("Find rows with this value..."))
                {
                    foreach (KeyValuePair<string, Param> p in bank.Params)
                    {
                        int v = (int)oldval;
                        Param.Row r = p.Value[v];
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