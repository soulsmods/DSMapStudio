using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using SoulsFormats;
using System.Threading.Tasks;
using StudioCore.Editor;

namespace StudioCore.TextEditor
{
    /// <summary>
    /// Static class that stores all the strings for a Souls game
    /// </summary>
    public static class FMGBank
    {
        public enum ItemCategory
        {
            None,
            Goods,
            Weapons,
            Armor,
            Rings,
            Spells,
            Characters,
            Locations,
            DSRGoods,
            DSRWeapons,
            DSRArmor,
            DSRRings,
            DSRSpells,
            DSRCharacters,
            DSRLocations,
            Gem,
            Message,
            SwordArts,
            Effect,
            Misc,
        }

        public enum ItemType
        {
            None,
            Title,
            Summary,
            Description,
        }

        /// <summary>
        /// BND IDs for the various fmg files in the item bnd
        /// </summary>
        public enum ItemFMGTypes
        {
            TitleGoods = 10,
            TitleWeapons = 11,
            TitleArmor = 12,
            TitleRings = 13,
            TitleSpells = 14,
            TitleTest = 15,
            TitleTest2 = 16,
            TitleTest3 = 17,
            TitleCharacters = 18,
            TitleLocations = 19,
            SummaryGoods = 20,
            SummaryWeapons = 21,
            SummaryArmor = 22,
            SummaryRings = 23,
            DescriptionGoods = 24,
            DescriptionWeapons = 25,
            DescriptionArmor = 26,
            DescriptionRings = 27,
            SummarySpells = 28,
            DescriptionSpells = 29,
            DescriptionSkills = 40,
            // DSR unks?
            DSRDescriptionGoods = 100,
            DSRDescriptionSpells = 105,
            DSRDescriptionWeapons = 106,
            DSRDescriptionArmor = 108,
            DSRDescriptionRings = 109,
            DSRSummaryGoods = 110,
            DSRTitleGoods = 111,
            DSRSummaryRings = 112,
            DSRTitleRings = 113,
            DSRSummaryWeapons = 114,
            DSRTitleWeapons = 115,
            DSRSummaryArmor = 116,
            DSRTitleArmor = 117,
            DSRTitleSpells = 118,
            DSRTitleCharacters = 119,
            DSRTitleLocations = 120,
            /* Missing the following DSR overrides:
            15: TitleTest: 28
            16: TitleTest2: 25
            17: TitleTest3: 25
            28: SummarySpells: 474
            */

            // DS3 DLC1
            TitleGoodsDLC1 = 210,
            TitleWeaponsDLC1 = 211,
            TitleArmorDLC1 = 212,
            TitleRingsDLC1 = 213,
            TitleSpellsDLC1 = 214,
            TitleCharactersDLC1 = 215,
            TitleLocationsDLC1 = 216,
            SummaryGoodsDLC1 = 217,
            SummaryRingsDLC1 = 220,
            DescriptionGoodsDLC1 = 221,
            DescriptionWeaponsDLC1 = 222,
            DescriptionArmorDLC1 = 223,
            DescriptionRingsDLC1 = 224,
            SummarySpellsDLC1 = 225,
            DescriptionSpellsDLC1 = 226,

            // DS3 DLC2
            TitleGoodsDLC2 = 250,
            TitleWeaponsDLC2 = 251,
            TitleArmorDLC2 = 252,
            TitleRingsDLC2 = 253,
            TitleSpellsDLC2 = 254,
            TitleCharactersDLC2 = 255,
            TitleLocationsDLC2 = 256,
            SummaryGoodsDLC2 = 257,
            SummaryRingsDLC2 = 260,
            DescriptionGoodsDLC2 = 261,
            DescriptionWeaponsDLC2 = 262,
            DescriptionArmorDLC2 = 263,
            DescriptionRingsDLC2 = 264,
            SummarySpellsDLC2 = 265,
            DescriptionSpellsDLC2 = 266,

            // ER - out of order
            TitleGem = 35,
            SummaryGem = 36,
            DescriptionGem = 37,
            TitleMessage = 41,
            TitleSwordArts = 42,
            SummarySwordArts = 43,
            SummaryEffect = 44,
            ERUnk45 = 45,
            SummaryMiscER = 46,
        }

        public static AssetLocator AssetLocator = null;

        private static Dictionary<ItemFMGTypes, FMG> _fmgs = null;

        /// <summary>
        /// DS2 uses a different system with loose fmgs instead of IDs
        /// </summary>
        private static Dictionary<string, FMG> _ds2fmgs = null;

        public static bool IsLoaded { get; private set; } = false;
        public static bool IsLoading { get; private set; } = false;

        public static IReadOnlyDictionary<string, FMG> DS2Fmgs
        {
            get
            {
                return _ds2fmgs;
            }
        }

        public static ItemCategory ItemCategoryOf(ItemFMGTypes ftype)
        {
            switch(ftype)
            {
                case ItemFMGTypes.TitleTest:
                case ItemFMGTypes.TitleTest2:
                case ItemFMGTypes.TitleTest3:
                case ItemFMGTypes.DescriptionSkills:
                case ItemFMGTypes.ERUnk45:
                    return ItemCategory.None;
                
                case ItemFMGTypes.DescriptionGoods:
                case ItemFMGTypes.DescriptionGoodsDLC1:
                case ItemFMGTypes.DescriptionGoodsDLC2:
                case ItemFMGTypes.SummaryGoods:
                case ItemFMGTypes.SummaryGoodsDLC1:
                case ItemFMGTypes.SummaryGoodsDLC2:
                case ItemFMGTypes.TitleGoods:
                case ItemFMGTypes.TitleGoodsDLC1:
                case ItemFMGTypes.TitleGoodsDLC2:
                    return ItemCategory.Goods;
                case ItemFMGTypes.DSRDescriptionGoods:
                case ItemFMGTypes.DSRSummaryGoods:
                case ItemFMGTypes.DSRTitleGoods:
                    return ItemCategory.DSRGoods;

                case ItemFMGTypes.DescriptionWeapons:
                case ItemFMGTypes.DescriptionWeaponsDLC1:
                case ItemFMGTypes.DescriptionWeaponsDLC2:
                case ItemFMGTypes.SummaryWeapons:
                case ItemFMGTypes.TitleWeapons:
                case ItemFMGTypes.TitleWeaponsDLC1:
                case ItemFMGTypes.TitleWeaponsDLC2:
                    return ItemCategory.Weapons;
                case ItemFMGTypes.DSRDescriptionWeapons:
                case ItemFMGTypes.DSRSummaryWeapons:
                case ItemFMGTypes.DSRTitleWeapons:
                    return ItemCategory.DSRWeapons;

                case ItemFMGTypes.DescriptionArmor:
                case ItemFMGTypes.DescriptionArmorDLC1:
                case ItemFMGTypes.DescriptionArmorDLC2:
                case ItemFMGTypes.SummaryArmor:
                case ItemFMGTypes.TitleArmor:
                case ItemFMGTypes.TitleArmorDLC1:
                case ItemFMGTypes.TitleArmorDLC2:
                    return ItemCategory.Armor;
                case ItemFMGTypes.DSRDescriptionArmor:
                case ItemFMGTypes.DSRSummaryArmor:
                case ItemFMGTypes.DSRTitleArmor:
                    return ItemCategory.DSRArmor;

                case ItemFMGTypes.DescriptionRings:
                case ItemFMGTypes.DescriptionRingsDLC1:
                case ItemFMGTypes.DescriptionRingsDLC2:
                case ItemFMGTypes.SummaryRings:
                case ItemFMGTypes.SummaryRingsDLC1:
                case ItemFMGTypes.SummaryRingsDLC2:
                case ItemFMGTypes.TitleRings:
                case ItemFMGTypes.TitleRingsDLC1:
                case ItemFMGTypes.TitleRingsDLC2:
                    return ItemCategory.Rings;
                case ItemFMGTypes.DSRDescriptionRings:
                case ItemFMGTypes.DSRSummaryRings:
                case ItemFMGTypes.DSRTitleRings:
                    return ItemCategory.DSRRings;

                case ItemFMGTypes.DescriptionSpells:
                case ItemFMGTypes.DescriptionSpellsDLC1:
                case ItemFMGTypes.DescriptionSpellsDLC2:
                case ItemFMGTypes.SummarySpells:
                case ItemFMGTypes.SummarySpellsDLC1:
                case ItemFMGTypes.SummarySpellsDLC2:
                case ItemFMGTypes.TitleSpells:
                case ItemFMGTypes.TitleSpellsDLC1:
                case ItemFMGTypes.TitleSpellsDLC2:
                    return ItemCategory.Spells;
                case ItemFMGTypes.DSRDescriptionSpells:
                case ItemFMGTypes.DSRTitleSpells:
                    return ItemCategory.DSRSpells;

                case ItemFMGTypes.TitleCharacters:
                case ItemFMGTypes.TitleCharactersDLC1:
                case ItemFMGTypes.TitleCharactersDLC2:
                    return ItemCategory.Characters;
                case ItemFMGTypes.DSRTitleCharacters:
                    return ItemCategory.DSRCharacters;

                case ItemFMGTypes.TitleLocations:
                case ItemFMGTypes.TitleLocationsDLC1:
                case ItemFMGTypes.TitleLocationsDLC2:
                    return ItemCategory.Locations;
                case ItemFMGTypes.DSRTitleLocations:
                    return ItemCategory.DSRLocations;

                case ItemFMGTypes.TitleGem:
                case ItemFMGTypes.SummaryGem:
                case ItemFMGTypes.DescriptionGem:
                    return ItemCategory.Gem;

                case ItemFMGTypes.TitleSwordArts:
                case ItemFMGTypes.SummarySwordArts:
                    return ItemCategory.SwordArts;

                case ItemFMGTypes.TitleMessage:
                    return ItemCategory.Message;

                case ItemFMGTypes.SummaryEffect:
                    return ItemCategory.Effect;

                case ItemFMGTypes.SummaryMiscER:
                    return ItemCategory.Misc;
                default:
                    throw new Exception("Unrecognized FMG type");
            }
        }

        public static ItemType ItemTypeOf(ItemFMGTypes ftype)
        {
            switch (ftype)
            {
                case ItemFMGTypes.DescriptionGoods:
                case ItemFMGTypes.DescriptionGoodsDLC1:
                case ItemFMGTypes.DescriptionGoodsDLC2:
                case ItemFMGTypes.DescriptionWeapons:
                case ItemFMGTypes.DescriptionWeaponsDLC1:
                case ItemFMGTypes.DescriptionWeaponsDLC2:
                case ItemFMGTypes.DescriptionArmor:
                case ItemFMGTypes.DescriptionArmorDLC1:
                case ItemFMGTypes.DescriptionArmorDLC2:
                case ItemFMGTypes.DescriptionRings:
                case ItemFMGTypes.DescriptionRingsDLC1:
                case ItemFMGTypes.DescriptionRingsDLC2:
                case ItemFMGTypes.DescriptionSpells:
                case ItemFMGTypes.DescriptionSpellsDLC1:
                case ItemFMGTypes.DescriptionSpellsDLC2:
                case ItemFMGTypes.DescriptionSkills:
                case ItemFMGTypes.DSRDescriptionArmor:
                case ItemFMGTypes.DSRDescriptionGoods:
                case ItemFMGTypes.DSRDescriptionRings:
                case ItemFMGTypes.DSRDescriptionSpells:
                case ItemFMGTypes.DSRDescriptionWeapons:
                case ItemFMGTypes.DescriptionGem:
                    return ItemType.Description;
                case ItemFMGTypes.SummaryGoods:
                case ItemFMGTypes.SummaryGoodsDLC1:
                case ItemFMGTypes.SummaryGoodsDLC2:
                case ItemFMGTypes.SummaryWeapons:
                case ItemFMGTypes.SummaryArmor:
                case ItemFMGTypes.SummaryRings:
                case ItemFMGTypes.SummaryRingsDLC1:
                case ItemFMGTypes.SummaryRingsDLC2:
                case ItemFMGTypes.SummarySpells:
                case ItemFMGTypes.SummarySpellsDLC1:
                case ItemFMGTypes.SummarySpellsDLC2:
                case ItemFMGTypes.DSRSummaryArmor:
                case ItemFMGTypes.DSRSummaryGoods:
                case ItemFMGTypes.DSRSummaryRings:
                case ItemFMGTypes.DSRSummaryWeapons:
                case ItemFMGTypes.SummaryGem:
                case ItemFMGTypes.SummarySwordArts:
                    return ItemType.Summary;
                case ItemFMGTypes.TitleGoods:
                case ItemFMGTypes.TitleGoodsDLC1:
                case ItemFMGTypes.TitleGoodsDLC2:
                case ItemFMGTypes.TitleWeapons:
                case ItemFMGTypes.TitleWeaponsDLC1:
                case ItemFMGTypes.TitleWeaponsDLC2:
                case ItemFMGTypes.TitleArmor:
                case ItemFMGTypes.TitleArmorDLC1:
                case ItemFMGTypes.TitleArmorDLC2:
                case ItemFMGTypes.TitleRings:
                case ItemFMGTypes.TitleRingsDLC1:
                case ItemFMGTypes.TitleRingsDLC2:
                case ItemFMGTypes.TitleSpells:
                case ItemFMGTypes.TitleSpellsDLC1:
                case ItemFMGTypes.TitleSpellsDLC2:
                case ItemFMGTypes.TitleCharacters:
                case ItemFMGTypes.TitleCharactersDLC1:
                case ItemFMGTypes.TitleCharactersDLC2:  
                case ItemFMGTypes.TitleLocations:
                case ItemFMGTypes.TitleLocationsDLC1:
                case ItemFMGTypes.TitleLocationsDLC2:
                case ItemFMGTypes.TitleTest:
                case ItemFMGTypes.TitleTest2:
                case ItemFMGTypes.TitleTest3:
                case ItemFMGTypes.DSRTitleArmor:
                case ItemFMGTypes.DSRTitleCharacters:
                case ItemFMGTypes.DSRTitleGoods:
                case ItemFMGTypes.DSRTitleLocations:
                case ItemFMGTypes.DSRTitleRings:
                case ItemFMGTypes.DSRTitleSpells:
                case ItemFMGTypes.DSRTitleWeapons:
                case ItemFMGTypes.TitleGem:
                case ItemFMGTypes.TitleMessage:
                case ItemFMGTypes.TitleSwordArts:
                //Summaries without titles
                case ItemFMGTypes.SummaryEffect:
                case ItemFMGTypes.SummaryMiscER:
                    return ItemType.Title;
                case ItemFMGTypes.ERUnk45:
                    return ItemType.None;
                default:
                    throw new Exception("Unrecognized FMG type");
            }
        }

        public static List<FMG.Entry> GetEntriesOfCategoryAndType(ItemCategory cat, ItemType type)
        {
            var list = new List<FMG.Entry>();
            foreach (var fmg in _fmgs)
            {
                if (ItemCategoryOf(fmg.Key) == cat && ItemTypeOf(fmg.Key) == type)
                {
                    list.AddRange(fmg.Value.Entries);
                }
            }
            return list;
        }

        public static void LookupItemID(int id, ItemCategory cat, out FMG.Entry title, out FMG.Entry summary, out FMG.Entry description)
        {
            title = null;
            summary = null;
            description = null;

            foreach (var item in GetEntriesOfCategoryAndType(cat, ItemType.Title))
            {
                if (item.ID == id)
                {
                    title = item;
                }
            }

            foreach (var item in GetEntriesOfCategoryAndType(cat, ItemType.Summary))
            {
                if (item.ID == id)
                {
                    summary = item;
                }
            }

            foreach (var item in GetEntriesOfCategoryAndType(cat, ItemType.Description))
            {
                if (item.ID == id)
                {
                    description = item;
                }
            }
        }

        public static FMG.Entry LookupItemID(int id, ItemCategory cat)
        {
            if (!IsLoaded || IsLoading)
                return null;
            foreach (var item in GetEntriesOfCategoryAndType(cat, ItemType.Title))
            {
                if (item.ID == id)
                {
                    return item;
                }
            }
            return null;
        }

        public static void ReloadFMGsDS2()
        {
            var desc = AssetLocator.GetEnglishItemMsgbnd(true);
            var files = Directory.GetFileSystemEntries($@"{AssetLocator.GameRootDirectory}\{desc.AssetPath}", @"*.fmg").ToList();
            _ds2fmgs = new Dictionary<string, FMG>();
            foreach (var file in files)
            {
                var modfile = $@"{AssetLocator.GameModDirectory}\{desc.AssetPath}\{Path.GetFileName(file)}";
                if (AssetLocator.GameModDirectory != null && File.Exists(modfile))
                {
                    var fmg = FMG.Read(modfile);
                    _ds2fmgs.Add(Path.GetFileNameWithoutExtension(modfile), fmg);
                }
                else
                {
                    var fmg = FMG.Read(file);
                    _ds2fmgs.Add(Path.GetFileNameWithoutExtension(file), fmg);
                }
            }
        }

        public static void ReloadFMGs()
        {
            IsLoaded = false;
            IsLoading = true;
            if (AssetLocator.Type == GameType.Undefined)
            {
                return;
            }

            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                ReloadFMGsDS2();
                IsLoading = false;
                IsLoaded = true;
                return;
            }

            IBinder fmgBinder;
            var desc = AssetLocator.GetEnglishItemMsgbnd();
            if (AssetLocator.Type == GameType.DemonsSouls || AssetLocator.Type == GameType.DarkSoulsPTDE || AssetLocator.Type == GameType.DarkSoulsRemastered)
            {
                fmgBinder = BND3.Read(desc.AssetPath);
            }
            else
            {
                fmgBinder = BND4.Read(desc.AssetPath);
            }

            _fmgs = new Dictionary<ItemFMGTypes, FMG>();
            foreach (var file in fmgBinder.Files)
            {
                _fmgs.Add((ItemFMGTypes)file.ID, FMG.Read(file.Bytes));
            }
            IsLoaded = true;
            IsLoading = false;
        }

        public static void SaveFMGsDS2()
        {
            foreach (var fmg in _ds2fmgs)
            {
                Utils.WriteWithBackup(AssetLocator.GameRootDirectory, AssetLocator.GameModDirectory, $@"menu\text\english\{fmg.Key}.fmg", fmg.Value);
            }
        }

        public static void SaveFMGs()
        {
            if (!IsLoaded)
                return;
            if (AssetLocator.Type == GameType.Undefined)
            {
                return;
            }

            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                SaveFMGsDS2();
                return;
            }

            // Load the fmg bnd, replace fmgs, and save
            IBinder fmgBinder;
            var desc = AssetLocator.GetEnglishItemMsgbnd();
            if (AssetLocator.Type == GameType.DemonsSouls || AssetLocator.Type == GameType.DarkSoulsPTDE || AssetLocator.Type == GameType.DarkSoulsRemastered)
            {
                fmgBinder = BND3.Read(desc.AssetPath);
            }
            else
            {
                fmgBinder = BND4.Read(desc.AssetPath);
            }

            foreach (var file in fmgBinder.Files)
            {
                if (_fmgs.ContainsKey((ItemFMGTypes)file.ID))
                {
                    file.Bytes = _fmgs[(ItemFMGTypes)file.ID].Write();
                }
            }

            var descw = AssetLocator.GetEnglishItemMsgbnd(true);
            if (fmgBinder is BND3 bnd3)
            {
                Utils.WriteWithBackup(AssetLocator.GameRootDirectory,
                    AssetLocator.GameModDirectory, descw.AssetPath, bnd3);
            }
            else if (fmgBinder is BND4 bnd4)
            {
                Utils.WriteWithBackup(AssetLocator.GameRootDirectory,
                    AssetLocator.GameModDirectory, descw.AssetPath, bnd4);
            }
        }

        public static void SetAssetLocator(AssetLocator l)
        {
            AssetLocator = l;
            //ReloadFMGs();
        }
    }
}
