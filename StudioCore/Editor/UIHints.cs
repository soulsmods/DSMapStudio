using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using ImGuiNET;

namespace StudioCore.Editor
{
    public class UIHints
    {
        public static string MassEditHint =
        @"Mass Edit Commands utilise Regex, and CAPITALISED words in commands indicate that a Regex expression may be used instead of plain text.
Multiple commands can be given at once.
A mass edit command is formed of a selector and an operation.

The selector may be 'selection: FIELD' to indicate that you wish to edit all currently selected rows.
It may also be 'param PARAM: ' to select by a specific param followed by any of the following row selectors:
    'modified: ' to select modified rows,
    'original: ' to select unmodified rows,
    'id VALUE: ' to select rows by their ids,
    'name NAME: ' to select rows with a matching name,
    'prop FIELD VALUE: ' to select rows that have a field that matches the given value. FIELD must be exact, but \s may be used instead of a space.
    'propref FIELD NAME: ' to select rows that have a field that is a reference to a row with a matching name. FIELD must be exact, but \s may be used instead of a space.
And finally followed with 'FIELD: ' to indicate the field you wish to change.

An operation is given by 'OP VALUE;'
VALUE is either a given number or 'field NAME;', indicating the value to be used is read from the given field (per row).
The valid values of OP are:
    '=' assigns the value to the field
    '*' multiplies the current value of the field by the given value
    '/' divides the current value of the field by the given value
    '+' adds the given value to the value of the field
    '-' subtracts the given value from the value of the field
    'ref' searches for a row with a given name in a field supporting references and assigns it to that field
    'replace' works for names only, and requires an argument in the form stringA:stringB, where stringA is replaced by stringB.

A complete command may look like the following DS3 examples:
selection: throwAtkRate: = 30;
param EquipParamWeapon: name Dagger.*: throwAtkRate: * 2;
param EquipParamWeapon: prop weaponCategory 0: correctAgility: + field correctStrength;";

        public static string SearchBarHint =
@"This searchbar utilises Regex, and CAPITALISED words in a search expression indicates that a Regex expression may be used instead of plain text.
Searches are case-insensitive and the searched term may appear anywhere in the target rows.

The following options determine how rows a filtered:
    'modified' to select modified rows,
    'original' to select unmodified rows,
    'id VALUE' to select rows by their ids,
    'name NAME' to select rows with a matching name,
    'prop FIELD VALUE' to select rows that have a field that matches the given value. FIELD must be exact, but \s may be used instead of a space.
    'propref FIELD NAME' to select rows that have a field that is a reference to a row with a matching name. FIELD must be exact, but \s may be used instead of a space.

A complete search may look like the following DS3 examples:
id 10000
name Dagger
propref originEquipWep0 Dagger";

        public static bool AddImGuiHintButton(string id, ref string hint, bool canEdit = false)
        {
            bool ret = false;
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(0.6f, 0.6f, 1.0f, 1.0f), "Help");
            if (ImGui.BeginPopupContextItem(id))
            {
                if (ParamEditor.ParamEditorScreen.EditorMode && canEdit) //remove this, editor mode should be called earlier
                {
                    ImGui.InputTextMultiline("", ref hint, 8196, new Vector2(720, 480));
                    if (ImGui.IsItemDeactivatedAfterEdit())
                        ret = true;
                }
                else
                    ImGui.Text(hint);
                ImGui.EndPopup();
            }
            return ret;
        }
    }
}