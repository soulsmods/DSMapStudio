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
using FSParam;
using StudioCore;
using StudioCore.Editor;
using StudioCore.ParamEditor;
using StudioCore.TextEditor;

namespace StudioCore.Editor
{
    public class EditorDecorations
    {
        private static string _refContextCurrentAutoComplete = "";
        
        public static bool HelpIcon(string id, ref string hint, bool canEdit)
        {
            if (hint == null)
                return false;
            return UIHints.AddImGuiHintButton(id, ref hint, canEdit, true); //presently a hack, move code here
        }

        public static void ParamRefText(List<ParamRef> paramRefs, Param.Row context)
        {
            if (paramRefs == null || paramRefs.Count == 0)
                return;
            if (CFG.Current.Param_HideReferenceRows == false) //Move preference
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
                ImGui.TextUnformatted($@"  <");
                List<string> inactiveRefs = new List<string>();
                bool first = true;
                foreach (ParamRef r in paramRefs)
                {
                    Param.Cell? c = context?[r.conditionField];
                    bool inactiveRef = context != null && c != null && Convert.ToInt32(c.Value.Value) != r.conditionValue;
                    if (inactiveRef)
                    {
                        inactiveRefs.Add(r.param);
                    }
                    else
                    {
                        ImGui.SameLine();
                        if (first)
                            ImGui.TextUnformatted(r.param);
                        else
                            ImGui.TextUnformatted(","+r.param);
                        first = false;
                    }
                }

                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
                foreach (string inactive in inactiveRefs)
                {
                    ImGui.SameLine();
                    if (first)
                        ImGui.TextUnformatted(inactive);
                    else
                        ImGui.TextUnformatted(","+inactive);
                    first = false;
                }
                ImGui.PopStyleColor();

                ImGui.SameLine();
                ImGui.TextUnformatted(">");
                ImGui.PopStyleColor();
                ImGui.PopStyleVar();
            }
        }
        public static void FmgRefText(string fmgRef)
        {
            if (fmgRef == null)
                return;
            if (CFG.Current.Param_HideReferenceRows == false) //Move preference
            {
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
                ImGui.TextUnformatted($@"  [");
                ImGui.SameLine();
                ImGui.TextUnformatted(fmgRef);
                ImGui.SameLine();
                ImGui.TextUnformatted("]");
                ImGui.PopStyleColor();
                ImGui.PopStyleVar();
            }
        }
        public static void ParamRefsSelectables(ParamBank bank, List<ParamRef> paramRefs, Param.Row context, dynamic oldval)
        {
            if (paramRefs == null)
                return;
            // Add named row and context menu
            // Lists located params
            // May span lines
            List<(string, Param.Row, string)> matches = resolveRefs(bank, paramRefs, context, oldval);
            bool entryFound = matches.Count > 0;
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 0.5f, 1.0f));
            ImGui.BeginGroup();
            foreach ((string param, Param.Row row, string adjName) in matches)
            {
                ImGui.TextUnformatted(adjName);
            }
            ImGui.PopStyleColor();
            if (!entryFound)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
                ImGui.TextUnformatted("___");
                ImGui.PopStyleColor();
            }
            ImGui.EndGroup();
        }
        private static List<(string, Param.Row, string)> resolveRefs(ParamBank bank, List<ParamRef> paramRefs, Param.Row context, dynamic oldval)
        {
            List<(string, Param.Row, string)> rows = new List<(string, Param.Row, string)>();
            if (bank.Params == null)
            {
                return rows;
            }
            int originalValue = (int)oldval; //make sure to explicitly cast from dynamic or C# complains. Object or Convert.ToInt32 fail.
            foreach (ParamRef rf in paramRefs)
            {
                Param.Cell? c = context?[rf.conditionField];
                bool inactiveRef = context != null && c != null && Convert.ToInt32(c.Value.Value) != rf.conditionValue;
                if (inactiveRef)
                    continue;
                string rt = rf.param;
                string hint = "";
                if (bank.Params.ContainsKey(rt))
                {
                    int altval = originalValue;
                    if (rf.offset != 0)
                    {
                        altval += rf.offset;
                        hint += rf.offset > 0 ? "+" + rf.offset.ToString() : rf.offset.ToString();
                    }
                    Param param = bank.Params[rt];
                    ParamMetaData meta = ParamMetaData.Get(bank.Params[rt].AppliedParamdef);
                    if (meta != null && meta.Row0Dummy && altval == 0)
                        continue;
                    Param.Row r = param[altval];
                    if (r == null && altval > 0 && meta != null)
                    {
                        if (meta.FixedOffset != 0)
                        {
                            altval = originalValue + meta.FixedOffset;
                            hint += meta.FixedOffset > 0 ? "+" + meta.FixedOffset.ToString() : meta.FixedOffset.ToString();
                        }
                        if (meta.OffsetSize > 0)
                        {
                            altval = altval - altval % meta.OffsetSize;
                            hint += "+" + (originalValue % meta.OffsetSize).ToString();
                        }
                        r = bank.Params[rt][altval];
                    }
                    if (r == null)
                        continue;
                    if (r.Name == null || r.Name.Trim().Equals(""))
                        rows.Add((rf.param, r, "Unnamed Row" + hint));
                    else
                        rows.Add((rf.param, r, r.Name + hint));
                }
            }
            return rows;
        }
        public static void FmgRefSelectable(string fmgName, dynamic oldval)
        {
            if (fmgName == null || !FMGBank.IsLoaded)
                return;
            FMGBank.FMGInfo fmgInfo = FMGBank.FmgInfoBank.Find((x) => x.Name == fmgName);
            if (fmgInfo == null)
                return;
            FMG.Entry entry = fmgInfo.GetPatchedEntries().Find((x) => x.ID == (int)oldval);
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 0.5f, 1.0f));
            if (entry == null || entry.Text == null || entry.Text.Trim().Equals(""))
                ImGui.TextUnformatted("%null%");
            else
                ImGui.TextUnformatted(entry.Text);
            ImGui.PopStyleColor();
        }
        public static void EnumNameText(string enumName)
        {
            if (enumName != null && CFG.Current.Param_HideEnums == false) //Move preference
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
                ImGui.TextUnformatted($@"  {enumName}");
                ImGui.PopStyleColor();
            }
        }
        public static void EnumValueText(Dictionary<string, string> enumValues, string value)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 0.5f, 1.0f));
            ImGui.TextUnformatted(enumValues.GetValueOrDefault(value, "Not Enumerated"));
            ImGui.PopStyleColor();
        }

        public static void VirtualParamRefSelectables(ParamBank bank, string virtualRefName, object searchValue)
        {
            // Add Goto statements
            if (bank.Params == null)
                return;
            foreach (var param in bank.Params)
            {
                PARAMDEF.Field foundfield = null;
                //get field
                foreach (PARAMDEF.Field f in param.Value.AppliedParamdef.Fields)
                {
                    if (FieldMetaData.Get(f).VirtualRef != null && FieldMetaData.Get(f).VirtualRef.Equals(virtualRefName))
                    {
                        foundfield = f;
                        break;
                    }
                }

                if (foundfield == null)
                    continue;
                //add selectable
                if (ImGui.Selectable($@"Go to first in {param.Key}"))
                {
                    foreach (Param.Row row in param.Value.Rows)
                    {
                        if (row[foundfield.InternalName].Value.ToString().Equals(searchValue.ToString()))
                        {
                            EditorCommandQueue.AddCommand($@"param/select/-1/{param.Key}/{row.ID}");
                            break;
                        }
                    }
                }
            }
        }
        
        public static bool ParamRefEnumContextMenu(ParamBank bank, object oldval, ref object newval, List<ParamRef> RefTypes, Param.Row context, FMGBank.FMGInfo fmgInfo, ParamEnum Enum)
        {
            if ((CFG.Current.Param_HideReferenceRows || RefTypes == null) && (CFG.Current.Param_HideEnums || Enum == null) && (CFG.Current.Param_HideReferenceRows || fmgInfo == null))
                return false;
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left) && (InputTracker.GetKey(Veldrid.Key.ControlLeft) || InputTracker.GetKey(Veldrid.Key.ControlRight)))
            {
                if (RefTypes != null)
                {
                    var primaryRef = resolveRefs(bank, RefTypes, context, oldval)?.First();
                    if (primaryRef != null)
                    {
                        if (InputTracker.GetKey(Veldrid.Key.ShiftLeft) || InputTracker.GetKey(Veldrid.Key.ShiftRight))
                            EditorCommandQueue.AddCommand($@"param/select/new/{primaryRef?.Item1}/{primaryRef?.Item2.ID}");
                        else
                            EditorCommandQueue.AddCommand($@"param/select/-1/{primaryRef?.Item1}/{primaryRef?.Item2.ID}");
                    }
                }
                if (fmgInfo != null)
                {
                    EditorCommandQueue.AddCommand($@"text/select/{fmgInfo.Name}/{(int)oldval}");
                }
            }
            bool result = false;
            if (ImGui.BeginPopupContextItem("rowMetaValue"))
            {
                if (RefTypes != null)
                    result |= PropertyRowRefsContextItems(bank, RefTypes, context, oldval, ref newval);
                if (Enum != null)
                    result |= PropertyRowEnumContextItems(Enum, oldval, ref newval);
                if (fmgInfo != null && ImGui.Selectable($@"Goto {fmgInfo.Name} Text"))
                    EditorCommandQueue.AddCommand($@"text/select/{fmgInfo.Name}/{(int)oldval}");
                ImGui.EndPopup();
            }
            return result;
        }

        public static bool PropertyRowRefsContextItems(ParamBank bank, List<ParamRef> reftypes, Param.Row context, dynamic oldval, ref object newval)
        {
            if (bank.Params == null)
                return false;
            // Add Goto statements
            List<(string, Param.Row, string)> refs = resolveRefs(bank, reftypes, context, oldval);
            foreach (var rf in refs)
            {
                if (ImGui.Selectable($@"Go to {rf.Item3}"))
                    EditorCommandQueue.AddCommand($@"param/select/-1/{rf.Item1}/{rf.Item2.ID}");
                if (ImGui.Selectable($@"Go to {rf.Item3} in new view"))
                    EditorCommandQueue.AddCommand($@"param/select/new/{rf.Item1}/{rf.Item2.ID}");
            }
            // Add searchbar for named editing
            ImGui.InputText("##value", ref _refContextCurrentAutoComplete, 128);
            // This should be replaced by a proper search box with a scroll and everything
            if (_refContextCurrentAutoComplete != "")
            {
                foreach (ParamRef rf in reftypes)
                {
                    string rt = rf.param;
                    if (!bank.Params.ContainsKey(rt))
                        continue;
                    ParamMetaData meta = ParamMetaData.Get(bank.Params[rt].AppliedParamdef);
                    int maxResultsPerRefType = 15 / reftypes.Count;
                    List<Param.Row> rows = RowSearchEngine.rse.Search((bank, bank.Params[rt]), _refContextCurrentAutoComplete, true, true);
                    foreach (Param.Row r in rows)
                    {
                        if (maxResultsPerRefType <= 0)
                            break;
                        if (ImGui.Selectable($@"({rt}){r.ID}: {r.Name}"))
                        {
                            if (meta != null && meta.FixedOffset != 0)
                                newval = (int)r.ID - meta.FixedOffset - rf.offset;
                            else
                                newval = (int)r.ID - rf.offset;
                            _refContextCurrentAutoComplete = "";
                            return true;
                        }
                        maxResultsPerRefType--;
                    }
                }
            }
            return false;
        }
        public static bool PropertyRowEnumContextItems(ParamEnum en, object oldval, ref object newval)
        {
            try
            {
                foreach (KeyValuePair<string, string> option in en.values)
                {
                    if (ImGui.Selectable($"{option.Key}: {option.Value}"))
                    {
                        newval = Convert.ChangeType(option.Key, oldval.GetType());
                        return true;
                    }
                }
            }
            catch
            {

            }
            return false;
        }
    }
}