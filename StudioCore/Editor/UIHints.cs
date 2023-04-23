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
        @"For help with regex or examples, consult the main help menu.
Mass Edit Commands utilise Regex, and words surrounded by ! in commands indicate that a Regex expression may be used instead of plain text.
All other words in capitals are parameters for the given command.
A mass edit command is formed of selectors and an operation.
There are multiple stages of selection, going from params, to rows, to cells (fields).
Multiple selectors can be given for a single stage by separating them with &&.

Param selection is done through any of the following:
    modified: to select params changed from vanilla
    original: to select unmodified params
    param !NAME!: to select params with a matching name

Row selection is done through any of the following:
    modified: to select rows changed from vanilla
    original: to select unmodified rows
    id !VALUE!: to select rows with a matching ID
    idrange MIN MAX: to select rows with an ID within the given bounds
    name !NAME!: to select rows with a matching name
    prop FIELD !VALUE!: to select rows who have a matching value for the given field
    proprange FIELD MIN MAX: to select rows who have a value for the given field within the given bounds
    propref FIELD !NAME!: to select rows that have a reference to another row with a matching name.

An optional combined form of selecting both params and rows is given by the following:
    selection: to select the param and rows in the active view's selection.

Cell selection is done through the following:
    !FIELD!: to select cells with a matching name

An operation is given by any of the following:
    OP VALUE; to perform the operation OP with the given literal value
    OP field NAME; to perform the operation OP with a value read from the given field of the row being modified.

The valid operations (OP) are:
    = assigns the given value to the field
    * multiplies by the given value and is invalid for names or arrays
    / divides by the given value and is invalid for names or arrays
    + adds the given value and is invalid for names or arrays
    - subtracts the given value and is invalid for names or arrays
    ref !NAME! searches for a row with a given name in a field supporting references and assigns it to that field
    replace OLD:NEW replaces parts of the text matching OLD with NEW
    replacex !OLD!:NEW replaces parts of the text matching the given regex OLD with NEW, where new may contain regex groups

A complete command may look like the following examples:
selection: throwAtkRate: = 30; (This selects from the current selection, the field throwAtkRate and makes its value 30)
param EquipParamWeapon: name Dagger.*: throwAtkRate: * 2; (This selects from EquipParamWeapon all rows beginning with Dagger, and multiplies the values in throwAtkRate by 2)
param EquipParamWeapon: prop weaponCategory 0: correctAgility: + field correctStrength; (This selects from EquipParamWeapon all rows whose weaponCategory is 0, and adds the row's correctStrength to its correctAgility)
param EquipParamWeapon: id .*: name: replace Dark:Holy; (This selects from EquipParamWeapon ALL rows, and replaces all cases of Dark in their name with Holy
param EquipParamWeapon: name Dagger.* && idrange 10000 Infinity: throwAtkRate: * 2; (This selects from EquipParamWeapon all rows beginning with Dagger and with an id higher than 9999, and multiplies the values in throwAtkRate by 2)";

        public static string SearchBarHint =
@"For help with regex or examples, consult the main help menu.
This searchbar utilise Regex, and words surrounded by ! in commands indicate that a Regex expression may be used instead of plain text.
All other words in capitals are parameters for the given command.
Regex searches are case-insensitive and the searched term may appear anywhere in the target rows. To specify an exact match, surround the text with ^ and $ (eg. ^10$) or use a range variant.
Multiple selectors can be given by separating them with &&.

Row selection is done through any of the following:
    !VALUE!: to select rows with a matching ID or a matching name
    modified: to select rows changed from vanilla
    original: to select unmodified rows
    id !VALUE!: to select rows with a matching ID
    idrange MIN MAX: to select rows with an ID within the given bounds
    name !NAME!: to select rows with a matching name
    prop FIELD !VALUE!: to select rows who have a matching value for the given field
    proprange FIELD MIN MAX: to select rows who have a value for the given field within the given bounds
    propref FIELD !NAME!: to select rows that have a reference to another row with a matching name.

A complete search may look like the following examples:
id 10000 (This searches for all rows with an id containing 10000. This includes 10000, 1000010, 210000)
name Dagger (This searches for all rows with a name containing Dagger. This includes Blood Dagger, Sharp daggers and daggerfall)
propref originEquipWep0 Dagger (This searches for all rows whose field originEquipWep0 refers to a row with a name containing Dagger, following the same rules above.
name Dagger && idrange 10000 Infinity (This searches for all rows with a name containing Dagger and that have an id greater than 9999)";

        public static string MassEditExamples = @"A complete MassEdit command may look like the following examples:
selection: throwAtkRate: = 30; (This selects from the current selection, the field throwAtkRate and makes its value 30)
param EquipParamWeapon: name Dagger.*: throwAtkRate: * 2; (This selects from EquipParamWeapon all rows beginning with Dagger, and multiplies the values in throwAtkRate by 2)
param EquipParamWeapon: prop weaponCategory 0: correctAgility: + field correctStrength; (This selects from EquipParamWeapon all rows whose weaponCategory is 0, and adds the row's correctStrength to its correctAgility)
param EquipParamWeapon: id .*: name: replace Dark:Holy; (This selects from EquipParamWeapon ALL rows, and replaces all cases of Dark in their name with Holy
param EquipParamWeapon: name Dagger.* && idrange 10000 Infinity: throwAtkRate: * 2; (This selects from EquipParamWeapon all rows beginning with Dagger and with an id higher than 9999, and multiplies the values in throwAtkRate by 2)";
        public static string SearchExamples = @"A complete search may look like the following examples:
id 10000 (This searches for all rows with an id containing 10000. This includes 10000, 1000010, 210000)
name Dagger (This searches for all rows with a name containing Dagger. This includes Blood Dagger, Sharp daggers and daggerfall)
propref originEquipWep0 Dagger (This searches for all rows whose field originEquipWep0 refers to a row with a name containing Dagger, following the same rules above.
name Dagger && idrange 10000 Infinity (This searches for all rows with a name containing Dagger and that have an id greater than 9999)";

        public static string RegexCheatSheet = @"Regex is a common way to write an expression that 'matches' strings or words, finding occurances within a passage of text.

For letters and numbers, regex matches only those explicit characters and nothing else. This means searching for dog, you will only match when 'dog' is found.
Regex also has many meta-characters, and most symbols have some meaning. For example, * means 'any number of the previous character'.
In this case, Do*g matches Dg, Dog, Doog, Dooog... etc.

Regex can also provide options with the | symbol (OR symbol). Dog|Frog matches Dog or Frog.
This isn't always useful as maybe only part of your expression is optional. You can use brackets to seperate that part.
In this manner, (D|Fr)og matches Dog or Frog. You can even use * on a bracketed expression.

Sometimes it doesn't make sense to write (0|1|2|3|4|5|6|7|8|9), so regex also has something called ranges.
This is written as [abc123*+] and matches any single character inside. Many metacharacters also don't function inside, and merely match themselves literally.
They can also include expressions like [1-9] and [a-zA-Z] which include the middle numbers and letters.

As if that wasn't enough, for common ones of these there are more shorthands called character classes.
\d matches [0-9], \w matches any letter, number, or _, \s matches any whitespace, and . matches any character at all - which makes .* a handy 'match any amount of anything'.
Character classes can also begin with ^ to mean any character NOT given. So [^0] is any nonzero character.

Finally, regex is often used to find occurances within text, not to match the entire text. The character ^ is used to match the start of the text, and $ the end.
A statement like ^[^-] will only match text that begins with something other than - for example.

Regex has a broad syntax and application, but with this quick guide and an online cheatsheet, many interesting combinations are possible.
While regex is aimed at text and is not perfect for numbers, it can still be useful.
Some common tools for mapstudio include:
.* (match anything)
^10$ (match only when the entire input is 10)
[^0] (match anything that isn't just 0)
^[^-] (match anything that doesn't begin with -)
^2\d\d$ (match any number from 200 to 299)";

        public static bool AddImGuiHintButton(string id, ref string hint, bool canEdit = false, bool isRowHint = false)
        {
            float scale = ImGuiRenderer.GetUIScale();
            bool ret = false;
            /*
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
            */

            // Even worse of a hack than it was before. eat my shorts (all of this should be redone)
            if (isRowHint)
            {
                ImGui.TextColored(new Vector4(0.6f, 0.6f, 1.0f, 1f), "?");
                if (ImGui.BeginPopupContextItem(id))
                {
                    if (ParamEditor.ParamEditorScreen.EditorMode && canEdit) //remove this, editor mode should be called earlier
                    {
                        ImGui.InputTextMultiline("", ref hint, 8196, new Vector2(720, 480) * scale);
                        if (ImGui.IsItemDeactivatedAfterEdit())
                            ret = true;
                    }
                    else
                        ImGui.Text(hint);
                    ImGui.EndPopup();
                }
                ImGui.SameLine();
            }
            else
            {
                ImGui.SameLine(0, 20f);
                if (ImGui.Button("Help"))
                {
                    ImGui.OpenPopup("##ParamHelp");
                }
                if (ImGui.BeginPopup("##ParamHelp"))
                {

                    if (ParamEditor.ParamEditorScreen.EditorMode && canEdit) //remove this, editor mode should be called earlier
                    {
                        ImGui.InputTextMultiline("", ref hint, 8196, new Vector2(720, 480) * scale);
                        if (ImGui.IsItemDeactivatedAfterEdit())
                            ret = true;
                    }
                    else
                        ImGui.Text(hint);
                    ImGui.EndPopup();
                }
            }
            return ret;
        }
    }
}