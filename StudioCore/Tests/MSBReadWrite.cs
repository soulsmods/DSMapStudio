using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SoulsFormats;
using StudioCore.ParamEditor;
using StudioCore.TextEditor;

namespace StudioCore.Tests
{
    public static class MSBReadWrite
    {
        public static bool Run(AssetLocator locator)
        {
            List<string> output = new();

            Dictionary<int, string> nameDict = new(); // behaviorVariationID, weapon name

            var wepParam = ParamBank.PrimaryBank.Params["EquipParamWeapon"];
            var behParam = ParamBank.PrimaryBank.Params["BehaviorParam_PC"];
            var entries = FMGBank.GetFmgEntriesByCategoryAndTextType(FmgEntryCategory.Weapons, FmgEntryTextType.Title);

            foreach (var wep in wepParam.Rows)
            {
                var entry = entries.Find(e => e.ID == wep.ID);
                if (entry != default)
                    nameDict[(int)wep["behaviorJudgeId"].Value.Value] = entry.Text;
            }

            foreach (var beh in behParam.Rows)
            {
                if (nameDict.TryGetValue((int)beh["behaviorJudgeId"].Value.Value, out string name))
                {
                    output.Add($"{beh.ID} {name}");
                }
            }

            File.WriteAllLines("BehaviorParam_PC.txt", output);

            return true;
        }
    }
}
