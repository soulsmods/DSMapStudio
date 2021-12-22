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

namespace StudioCore.Editor
{
    public class EditorDecorations
    {
        public static void HelpIcon(string id, string hint)
        {
            if (hint == null)
                return;
            UIHints.AddImGuiHintButton(id, ref hint); //presently a hack, move code here
        }

        public static void ParamRefText(List<string> paramRefs)
        {
            if (paramRefs != null && ParamEditor.ParamEditorScreen.HideReferenceRowsPreference == false) //Move preference
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
                ImGui.TextUnformatted($@"  <{String.Join(',', paramRefs)}>");
                ImGui.PopStyleColor();
            }
        }

        public static void EnumNameText(string enumName)
        {
            if (enumName != null && ParamEditor.ParamEditorScreen.HideEnumsPreference == false) //Move preference
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
                ImGui.TextUnformatted($@"  {enumName}");
                ImGui.PopStyleColor();
            }
        }

        public static void VirtualParamRefSelectables(string vref, object searchValue)
        {
            // Add Goto statements
            foreach (var param in ParamEditor.ParamBank.Params)
            {
                PARAMDEF.Field foundfield = null;
                //get field
                foreach (PARAMDEF.Field f in param.Value.AppliedParamdef.Fields)
                { 
                    if (ParamEditor.FieldMetaData.Get(f).VirtualRef != null && ParamEditor.FieldMetaData.Get(f).VirtualRef.Equals(vref))
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
                    foreach (PARAM.Row row in param.Value.Rows)
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

        public static void ParamRefsSelectables(List<string> paramRefs, dynamic oldval)
        {
            // Add named row and context menu
            // Lists located params
            // May span lines
            int originalValue = (int) oldval; //make sure to explicitly cast from dynamic or C# complains. Object or Convert.ToInt32 fail.
            bool entryFound = false;
            foreach (string rt in paramRefs)
            {
                string hint = "";
                if (ParamEditor.ParamBank.Params.ContainsKey(rt))
                {
                    PARAM param = ParamEditor.ParamBank.Params[rt];
                    ParamEditor.ParamMetaData meta = ParamEditor.ParamMetaData.Get(ParamEditor.ParamBank.Params[rt].AppliedParamdef);
                    if (meta != null && meta.Row0Dummy && originalValue == 0)
                        continue;
                    PARAM.Row r = param[originalValue];
                    ImGui.SameLine();
                    if (r == null && originalValue > 0 && meta != null)
                    {
                        int altval = originalValue;
                        if (meta.FixedOffset != 0)
                        {
                            altval = originalValue + meta.FixedOffset;
                            hint += meta.FixedOffset>0 ? "+"+meta.FixedOffset.ToString() : meta.FixedOffset.ToString();
                        }
                        if (meta.OffsetSize > 0)
                        {
                            altval = altval - altval % meta.OffsetSize;
                            hint += "+"+(originalValue % meta.OffsetSize).ToString();
                        }
                        r = ParamEditor.ParamBank.Params[rt][altval];
                    }
                    if (r == null)
                        continue;
                    entryFound = true;
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 0.5f, 1.0f));
                    if (r.Name == null || r.Name.Equals(""))
                    {
                        ImGui.TextUnformatted("Unnamed Row");
                    }
                    else
                    {
                        ImGui.TextUnformatted(r.Name + hint);
                    }
                    ImGui.PopStyleColor();
                    ImGui.NewLine();
                }
            }
            ImGui.SameLine();
            if (!entryFound)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
                ImGui.TextUnformatted("___");
                ImGui.PopStyleColor();
            }
        }

        public static void EnumValueText(Dictionary<string, string> enumValues, string value)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.5f, 0.5f, 1.0f));
            ImGui.TextUnformatted(enumValues.GetValueOrDefault(value, "Not Enumerated"));
            ImGui.PopStyleColor();
        }

    }
}
