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
    public class ParamRowEditor
    {
        public ActionManager ContextActionManager;
        private ParamEditorScreen _paramEditor;

        private Dictionary<string, PropertyInfo[]> _propCache = new Dictionary<string, PropertyInfo[]>();

        public ParamRowEditor(ActionManager manager, ParamEditorScreen paramEditorScreen)
        {
            ContextActionManager = manager;
            _paramEditor = paramEditorScreen;
        }

        private static void PropEditorParamRow_Header(bool isActiveView, ref string propSearchString)
        {
            if (propSearchString != null)
            {
                if (isActiveView && InputTracker.GetKeyDown(KeyBindings.Current.Param_SearchField))
                    ImGui.SetKeyboardFocusHere();
                ImGui.InputText($"Search <{KeyBindings.Current.Param_SearchField.HintText}>", ref propSearchString, 255);
                if (ImGui.IsItemEdited())
                    UICache.ClearCaches();
                string resAutoCol = AutoFill.ColumnSearchBarAutoFill();
                if (resAutoCol != null)
                {
                    propSearchString = resAutoCol;
                    UICache.ClearCaches();
                }
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
            }
        }

        private void PropEditorParamRow_RowFields(ParamBank bank, Param.Row row, Param.Row vrow, List<(string, Param.Row)> auxRows, Param.Row crow, ref int imguiId, ParamEditorSelectionState selection)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.8f, 1.0f, 1.0f));
            var nameProp = row.GetType().GetProperty("Name");
            var idProp = row.GetType().GetProperty("ID");
            PropEditorPropInfoRow(bank, row, vrow, auxRows, crow, nameProp, "Name", ref imguiId, selection);
            PropEditorPropInfoRow(bank, row, vrow, auxRows, crow, idProp, "ID", ref imguiId, selection);
            ImGui.PopStyleColor();
            ImGui.Spacing();
        }

        private void PropEditorParamRow_PinnedFields(List<string> pinnedFields, ParamBank bank, Param.Row row, Param.Row vrow, List<(string, Param.Row)> auxRows, Param.Row crow, List<(PseudoColumn, Param.Column)> cols, List<(PseudoColumn, Param.Column)> vcols, List<List<(PseudoColumn, Param.Column)>> auxCols, ref int imguiId, string activeParam, ParamEditorSelectionState selection)
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
                    ref imguiId, activeParam, true, selection);
                }
            }
        }
        private void PropEditorParamRow_MainFields(ParamMetaData meta, ParamBank bank, Param.Row row, Param.Row vrow, List<(string, Param.Row)> auxRows, Param.Row crow, List<(PseudoColumn, Param.Column)> cols, List<(PseudoColumn, Param.Column)> vcols, List<List<(PseudoColumn, Param.Column)>> auxCols, ref int imguiId, string activeParam, ParamEditorSelectionState selection)
        {
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
                    EditorDecorations.ImguiTableSeparator();
                    lastRowExists = false;
                    continue;
                }
                List<(PseudoColumn, Param.Column)> matches = cols?.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList();
                List<(PseudoColumn, Param.Column)> vmatches = vcols?.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList();
                List<List<(PseudoColumn, Param.Column)>> auxMatches = auxCols?.Select((aux, i) => aux.Where((x, i) => x.Item2 != null && x.Item2.Def.InternalName == field).ToList()).ToList();
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
                    ref imguiId, activeParam, false, selection);
                    lastRowExists = true;
                }
            }
        }

        public void PropEditorParamRow(ParamBank bank, Param.Row row, Param.Row vrow, List<(string, Param.Row)> auxRows, Param.Row crow, ref string propSearchString, string activeParam, bool isActiveView, ParamEditorSelectionState selection)
        {
            ParamMetaData meta = ParamMetaData.Get(row.Def);
            int imguiId = 0;

            PropEditorParamRow_Header(isActiveView, ref propSearchString);

            //ImGui.BeginChild("Param Fields");
            int columnCount = 2;
            if (CFG.Current.Param_ShowVanillaParams)
                columnCount++;
            if (crow != null)
                columnCount++;
            if (auxRows.Count > 0)
                columnCount += auxRows.Count;
            if (EditorDecorations.ImGuiTableStdColumns("ParamFieldsT", columnCount, false))
            {
                ImGui.TableSetupScrollFreeze(columnCount, auxRows.Count > 0 ? 3 : 2);
                if (auxRows.Count > 0)
                {
                    ImGui.TableNextColumn();
                    if (ImGui.TableNextColumn())
                        ImGui.Text("Current");
                    if (CFG.Current.Param_ShowVanillaParams && ImGui.TableNextColumn())
                        ImGui.Text("Vanilla");
                    foreach ((string name, Param.Row r) in auxRows)
                    {
                        if (ImGui.TableNextColumn())
                            ImGui.Text(name);
                    }
                }
                PropEditorParamRow_RowFields(bank, row, vrow, auxRows, crow, ref imguiId, selection);
                EditorDecorations.ImguiTableSeparator();
                
                string search = propSearchString;
                List<(PseudoColumn, Param.Column)> cols = UICache.GetCached(_paramEditor, row, "fieldFilter", () => CellSearchEngine.cse.Search((activeParam, row), search, true, true));
                List<(PseudoColumn, Param.Column)> vcols = UICache.GetCached(_paramEditor, vrow, "vFieldFilter", () => cols.Select((x, i) => x.GetAs(ParamBank.VanillaBank.GetParamFromName(activeParam))).ToList());
                List<List<(PseudoColumn, Param.Column)>> auxCols = UICache.GetCached(_paramEditor, auxRows, "auxFieldFilter", () => auxRows.Select((r, i) => cols.Select((c, j) => c.GetAs(ParamBank.AuxBanks[r.Item1].GetParamFromName(activeParam))).ToList()).ToList());

                List<string> pinnedFields = _paramEditor._projectSettings.PinnedFields.GetValueOrDefault(activeParam, null);
                if (pinnedFields?.Count > 0)
                {
                    PropEditorParamRow_PinnedFields(pinnedFields, bank, row, vrow, auxRows, crow, cols, vcols, auxCols, ref imguiId, activeParam, selection);
                    EditorDecorations.ImguiTableSeparator();
                }

                PropEditorParamRow_MainFields(meta, bank, row, vrow, auxRows, crow, cols, vcols, auxCols, ref imguiId, activeParam, selection);
                ImGui.EndTable();
            }
            if (meta.CalcCorrectDef != null || meta.SoulCostDef != null)
            {
                EditorDecorations.DrawCalcCorrectGraph(_paramEditor, meta, row);
            }
            //ImGui.EndChild();
        }

        // Many parameter options, which may be simplified.
        private void PropEditorPropInfoRow(ParamBank bank, Param.Row row, Param.Row vrow, List<(string, Param.Row)> auxRows, Param.Row crow, PropertyInfo prop, string visualName, ref int imguiId, ParamEditorSelectionState selection)
        {
            PropEditorPropRow(
                bank,
                prop.GetValue(row),
                crow != null ? prop.GetValue(crow) : null,
                vrow != null ? prop.GetValue(vrow) : null,
                auxRows.Select((r, i) => r.Item2 != null ? prop.GetValue(r.Item2) : null).ToList(),
                ref imguiId,
                "header",
                visualName,
                null,
                prop.PropertyType,
                prop,
                null,
                row,
                null,
                false,
                null,
                selection);
        }
        private void PropEditorPropCellRow(ParamBank bank, Param.Row row, Param.Row crow, (PseudoColumn, Param.Column) col, Param.Row vrow, (PseudoColumn, Param.Column) vcol, List<(string, Param.Row)> auxRows, List<(PseudoColumn, Param.Column)> auxCols, string fieldOffset, ref int imguiId, string activeParam, bool isPinned, ParamEditorSelectionState selection)
        {
            PropEditorPropRow(
                bank,
                row.Get(col),
                crow?.Get(col),
                vcol.IsColumnValid() ? vrow?.Get(vcol) : null,
                auxRows.Select((r, i) => auxCols[i].IsColumnValid() ? r.Item2?.Get(auxCols[i]) : null).ToList(),
                ref imguiId,
                fieldOffset != null ? "0x" + fieldOffset : null, col.Item2.Def.InternalName,
                FieldMetaData.Get(col.Item2.Def),
                col.GetColumnType(),
                typeof(Param.Cell).GetProperty("Value"),
                row[col.Item2],
                row,
                activeParam,
                isPinned,
                col.Item2,
                selection);
        }
        private void PropEditorPropRow(ParamBank bank, object oldval, object compareval, object vanillaval, List<object> auxVals, ref int imguiId, string fieldOffset, string internalName, FieldMetaData cellMeta, Type propType, PropertyInfo proprow, Param.Cell? nullableCell, Param.Row row, string activeParam, bool isPinned, Param.Column? col, ParamEditorSelectionState selection)
        {
            string Wiki = cellMeta?.Wiki;

            List<ParamRef> RefTypes = cellMeta?.RefTypes;
            List<FMGRef> FmgRef = cellMeta?.FmgRef;
            List<ExtRef> ExtRefs = cellMeta?.ExtRefs;
            string VirtualRef = cellMeta?.VirtualRef;

            ParamEnum Enum = cellMeta?.EnumType;
            bool IsBool = cellMeta?.IsBool ?? false;

            object newval = null;

            ImGui.PushID(imguiId);
            if (ImGui.TableNextColumn())
            {
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
                PropertyRowNameContextMenu(bank, internalName, cellMeta, activeParam, activeParam != null, isPinned, col, selection);

                EditorDecorations.ParamRefText(RefTypes, row);
                EditorDecorations.FmgRefText(FmgRef, row);
                EditorDecorations.EnumNameText(Enum == null ? null : Enum.name);
            }

            bool diffVanilla = ParamUtils.IsValueDiff(ref oldval, ref vanillaval, propType);
            bool diffCompare = ParamUtils.IsValueDiff(ref oldval, ref compareval, propType);
            List<bool> diffAuxVanilla = auxVals.Select((o, i) => ParamUtils.IsValueDiff(ref o, ref vanillaval, propType)).ToList();
            List<bool> diffAuxPrimaryAndVanilla = auxVals.Select((o, i) => ParamUtils.IsValueDiff(ref o, ref oldval, propType) && ParamUtils.IsValueDiff(ref o, ref vanillaval, propType)).ToList();
            int count = diffAuxPrimaryAndVanilla.Where((x)=>x).Count();
            bool conflict = ((diffVanilla ? 1:0) + diffAuxPrimaryAndVanilla.Where((x)=>x).Count()) > 1;

            bool matchDefault = nullableCell?.Def.Default != null && nullableCell.Value.Def.Default.Equals(oldval);
            bool isRef = (CFG.Current.Param_HideReferenceRows == false && (RefTypes != null || FmgRef != null)) || (CFG.Current.Param_HideEnums == false && Enum != null) || VirtualRef != null || ExtRefs != null;
            
            if (ImGui.TableNextColumn())
            {
                ImGui.SetNextItemWidth(-1);
                if (conflict)
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.25f, 0.2f, 0.2f, 1.0f));
                else if (diffVanilla)
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.22f, 0.2f, 1.0f));
                if (isRef)
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 1.0f, 1.0f));
                else if (matchDefault)
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.75f, 0.75f, 0.75f, 1.0f));

                // Property Editor UI
                ParamEditorCommon.PropertyField(propType, oldval, ref newval, IsBool);

                if (isRef || matchDefault) //if diffVanilla, remove styling later
                    ImGui.PopStyleColor();

                // Tooltip
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNormal | ImGuiHoveredFlags.NoSharedDelay))
                {
                    string str = $"Value Type: {propType.Name}";
                    if (propType.IsValueType)
                    {
                        var min = propType.GetField("MinValue")?.GetValue(propType);
                        var max = propType.GetField("MaxValue")?.GetValue(propType);
                        if (min != null & max != null)
                        {
                            str += $" (Min {min}, Max {max})";
                        }
                    }
                    if (Wiki != null)
                    {
                        str += $"\n\n{Wiki}";
                    }
                    ImGui.SetTooltip(str);
                }

                PropertyRowValueContextMenu(bank, row, internalName, VirtualRef, ExtRefs, oldval);

                if (CFG.Current.Param_HideReferenceRows == false && RefTypes != null)
                    EditorDecorations.ParamRefsSelectables(bank, RefTypes, row, oldval);
                if (CFG.Current.Param_HideReferenceRows == false && FmgRef != null)
                    EditorDecorations.FmgRefSelectable(_paramEditor, FmgRef, row, oldval);
                if (CFG.Current.Param_HideEnums == false && Enum != null)
                    EditorDecorations.EnumValueText(Enum.values, oldval.ToString());

                if (CFG.Current.Param_HideReferenceRows == false || CFG.Current.Param_HideEnums == false)
                {
                    if (EditorDecorations.ParamRefEnumContextMenu(bank, oldval, ref newval, RefTypes, row, FmgRef, Enum, ContextActionManager))
                    {
                        ParamEditorCommon.SetLastPropertyManual(newval);
                    }
                }
                if (conflict || diffVanilla)
                    ImGui.PopStyleColor();
            }
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.180f, 0.180f, 0.196f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 0.9f, 1.0f));
            if (conflict)
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.25f, 0.2f, 0.2f, 1.0f));
            if (CFG.Current.Param_ShowVanillaParams && ImGui.TableNextColumn())
            {
                AdditionalColumnValue(vanillaval, propType, bank, RefTypes, FmgRef, row, Enum, "vanilla");
            }
            for (int i=0; i<auxVals.Count; i++)
            {
                if (ImGui.TableNextColumn())
                {
                    if (!conflict && diffAuxVanilla[i])
                        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.2f, 0.35f, 1.0f));
                        AdditionalColumnValue(auxVals[i], propType, bank, RefTypes, FmgRef, row, Enum, i.ToString());
                    if (!conflict && diffAuxVanilla[i])
                        ImGui.PopStyleColor();
                }
            }
            if (conflict)
                ImGui.PopStyleColor();
            if (compareval != null && ImGui.TableNextColumn())
            {
                if(diffCompare)
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.2f, 0.2f, 0.35f, 1.0f));
                AdditionalColumnValue(compareval, propType, bank, RefTypes, FmgRef, row, Enum, "compRow");
                if (diffCompare)
                    ImGui.PopStyleColor();
            }
            ImGui.PopStyleColor(2);

            bool committed = ParamEditorCommon.UpdateProperty(ContextActionManager, nullableCell != null ? nullableCell : row, proprow, oldval);
            if (committed && !ParamBank.VanillaBank.IsLoadingParams)
                ParamBank.PrimaryBank.RefreshParamRowDiffs(row, activeParam);
            ImGui.PopID();
            imguiId++;
        }

        private void AdditionalColumnValue(object colVal, Type propType, ParamBank bank, List<ParamRef> RefTypes, List<FMGRef> FmgRef, Param.Row context, ParamEnum Enum, string imguiSuffix)
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
                    EditorDecorations.FmgRefSelectable(_paramEditor, FmgRef, context, colVal);
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

        private void PropertyRowNameContextMenu(ParamBank bank, string internalName, FieldMetaData cellMeta, string activeParam, bool showPinOptions, bool isPinned, Param.Column col, ParamEditorSelectionState selection)
        {
            float scale = MapStudioNew.GetUIScale();
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
                        ImGui.TextColored(new Vector4(1f, .7f, .4f, 1f), Utils.ImGuiEscape(internalName, "", true));
                    }
                    else
                        ImGui.TextColored(new Vector4(1f, .7f, .4f, 1f), Utils.ImGuiEscape(altName, "", true));
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
                if (col != null && ImGui.MenuItem("Compare field"))
                {
                    selection.SetCompareCol(col);
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
        private void PropertyRowValueContextMenu(ParamBank bank, Param.Row row, string internalName, string VirtualRef, List<ExtRef> ExtRefs, dynamic oldval)
        {
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
                        EditorCommandQueue.AddCommand($@"param/menu/massEditRegex/selection && !added: {Regex.Escape(internalName)}: = vanilla;");
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
                if (ExtRefs != null)
                {
                    foreach (ExtRef currentRef in ExtRefs)
                    {
                        List<string> matchedExtRefPath = currentRef.paths.Select((x) => (string)(string.Format(x, oldval))).ToList();
                        AssetLocator al = ParamBank.PrimaryBank.AssetLocator;
                        ExtRefItem(row, internalName, $"modded {currentRef.name}", matchedExtRefPath, al.GameModDirectory);
                        ExtRefItem(row, internalName, $"vanilla {currentRef.name}", matchedExtRefPath, al.GameRootDirectory);
                    }
                }
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
        private void ExtRefItem(Param.Row keyRow, string fieldKey, string menuText, List<string> matchedExtRefPath, string dir)
        {
            bool exist = UICache.GetCached(_paramEditor, keyRow, $"extRef{menuText}{fieldKey}", () => Path.Exists(Path.Join(dir, matchedExtRefPath[0])));
            if (exist && ImGui.Selectable($"Go to {menuText} file..."))
            {
                string path = ResolveExtRefPath(matchedExtRefPath, dir);
                if (File.Exists(path))
                    Process.Start("explorer.exe", $"/select,\"{path}\"");
                else
                {
                    TaskLogs.AddLog($"\"{path}\" could not be found. It may be map or chr specific",
                        Microsoft.Extensions.Logging.LogLevel.Warning);
                    UICache.ClearCaches();
                }
            }
        }
        private string ResolveExtRefPath(List<string> matchedExtRefPath, string baseDir)
        {
            string currentPath = baseDir;
            foreach (string nextStage in matchedExtRefPath)
            {
                string thisPathF = Path.Join(currentPath, nextStage);
                string thisPathD = Path.Join(currentPath, nextStage.Replace('.', '-'));
                if (Directory.Exists(thisPathD))
                {
                    currentPath = thisPathD;
                    continue;
                }
                if (File.Exists(thisPathF))
                    currentPath = thisPathF;
                break;
            }
            if (currentPath == baseDir)
                return null;
            return currentPath;
        }
    }
}