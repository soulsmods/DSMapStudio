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
                case ItemFMGTypes.DescriptionWeapons:
                case ItemFMGTypes.DescriptionWeaponsDLC1:
                case ItemFMGTypes.DescriptionWeaponsDLC2:
                case ItemFMGTypes.SummaryWeapons:
                case ItemFMGTypes.TitleWeapons:
                case ItemFMGTypes.TitleWeaponsDLC1:
                case ItemFMGTypes.TitleWeaponsDLC2:
                    return ItemCategory.Weapons;
                case ItemFMGTypes.DescriptionArmor:
                case ItemFMGTypes.DescriptionArmorDLC1:
                case ItemFMGTypes.DescriptionArmorDLC2:
                case ItemFMGTypes.SummaryArmor:
                case ItemFMGTypes.TitleArmor:
                case ItemFMGTypes.TitleArmorDLC1:
                case ItemFMGTypes.TitleArmorDLC2:
                    return ItemCategory.Armor;
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
                case ItemFMGTypes.TitleCharacters:
                case ItemFMGTypes.TitleCharactersDLC1:
                case ItemFMGTypes.TitleCharactersDLC2:
                    return ItemCategory.Characters;
                case ItemFMGTypes.TitleLocations:
                case ItemFMGTypes.TitleLocationsDLC1:
                case ItemFMGTypes.TitleLocationsDLC2:
                    return ItemCategory.Locations;
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
                    return ItemType.Title;
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

            TaskManager.Run("FB:Reload", true, false, () =>
            {
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
            });
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
