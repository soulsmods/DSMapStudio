using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using SoulsFormats;
using System.Threading.Tasks;
using StudioCore.Editor;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace StudioCore.TextEditor
{
    /// <summary>
    /// Static class that stores all the strings for a Souls game.
    /// </summary>
    public static class FMGBank
    {
        /// <summary>
        /// Value pair with an entry and the FMG it belongs to.
        /// </summary>
        public class EntryFMGInfoPair
        {
            public EntryFMGInfoPair(FMGInfo fmgInfo, FMG.Entry entry)
            {
                FmgInfo = fmgInfo;
                Entry = entry;
            }

            public FMGInfo FmgInfo { get; set; }
            public FMG.Entry Entry { get; set; }
        }

        /// <summary>
        /// Base object that stores an FMG and information regarding it.
        /// </summary>
        public class FMGInfo
        {
            public string FileName;
            public string Name;
            public FmgIDType FmgID;
            public FMG Fmg;
            /// <summary>
            /// List of associated children to this FMGInfo used to get patch entry data.
            /// </summary>
            public List<FMGInfo> PatchChildren = new();
            public FMGInfo PatchParent;
            public FmgUICategory UICategory;
            public FmgEntryCategory EntryCategory;
            public FmgEntryTextType EntryType;
            public bool GroupedEntry = false;

            /// <summary>
            /// Returns a patched list of Entry & FMGInfo value pairs from this FMGInfo and its children.
            /// If a PatchParent exists, it will be checked instead.
            /// </summary>
            public List<EntryFMGInfoPair> GetPatchedEntryFMGPairs(bool sort = true)
            {
                if (PatchParent != null)
                {
                    return PatchParent.GetPatchedEntryFMGPairs(sort);
                }

                List<EntryFMGInfoPair> list = new();
                foreach (var entry in Fmg.Entries)
                {
                    list.Add(new EntryFMGInfoPair(this, entry));
                }

                // Check and apply patch entries
                foreach (var child in PatchChildren.OrderBy(e => (int)e.FmgID))
                {
                    foreach (var entry in child.Fmg.Entries)
                    {
                        var match = list.Find(e => e.Entry.ID == entry.ID);
                        if (match != null)
                        {
                            // Only non-null text will overrwrite
                            if (entry.Text != null)
                            {
                                match.Entry = entry;
                                match.FmgInfo = child;
                            }
                        }
                        else
                        {
                            list.Add(new EntryFMGInfoPair(child, entry));
                        }
                    }
                }
                if (sort)
                    list = list.OrderBy(e => e.Entry.ID).ToList();
                return list;
            }

            /// <summary>
            /// Returns a patched list of entries in this FMGInfo and its children.
            /// If a PatchParent exists, it will be checked instead.
            /// </summary>
            public List<FMG.Entry> GetPatchedEntries(bool sort = true)
            {
                if (PatchParent != null)
                {
                    return PatchParent.GetPatchedEntries(sort);
                }

                List<FMG.Entry> list = new();
                list.AddRange(Fmg.Entries);

                // Check and apply patch entries
                foreach (var child in PatchChildren.OrderBy(e => (int)e.FmgID))
                {
                    foreach (var entry in child.Fmg.Entries)
                    {
                        var match = list.Find(e => e.ID == entry.ID);
                        if (match != null)
                        {
                            // Only non-null text will overrwrite
                            if (entry.Text != null)
                            {
                                match = entry;
                            }
                        }
                        else
                        {
                            list.Add(entry);
                        }
                    }
                }
                if (sort)
                    list = list.OrderBy(e => e.ID).ToList();
                return list;
            }

            /// <summary>
            /// Adds an entry to the end of the FMG.
            /// </summary>
            public void AddEntry(FMG.Entry entry, bool sort = true)
            {
                Fmg.Entries.Add(entry);
                if (sort)
                    Fmg.Entries.Sort();
            }

            /// <summary>
            /// Inserts an entry into FMG at the specified index.
            /// </summary>
            public void InsertEntry(int index, FMG.Entry newEntry)
            {
                Fmg.Entries.Insert(index, newEntry);
            }

            /// <summary>
            /// Copies an entry within the FMG.
            /// If desired, it will also get and set the next unused entry ID.
            /// </summary>
            /// <param name="getUnusedID">If true, get next unused ID and set entry ID to it</param>
            /// <returns>The new entry</returns>
            public FMG.Entry CopyEntry(FMG.Entry entry, bool getUnusedID = false)
            {
                FMG.Entry newEntry = new(entry.ID, entry.Text);
                if (getUnusedID)
                {
                    do
                    {
                        newEntry.ID++;
                    }
                    while (Fmg.Entries.Find(e => e.ID == newEntry.ID) != null);
                }
                return newEntry;
            }

            /// <summary>
            /// Removes an entry from FMGInfo's FMG.
            /// </summary>
            public void DeleteEntry(FMG.Entry entry)
            {
                Fmg.Entries.Remove(entry);
            }
        }


        /// <summary>
        /// A group of entries that may be associated (such as title, summary, description) along with respective FMGs.
        /// </summary>
        public class EntryGroup
        {
            public FMG.Entry TextBody;
            public FMGInfo TextBodyInfo;
            public FMG.Entry Title;
            public FMGInfo TitleInfo;
            public FMG.Entry Summary;
            public FMGInfo SummaryInfo;
            public FMG.Entry Description;
            public FMGInfo DescriptionInfo;

            private int _ID = -1;
            public int ID
            {
                set
                {
                    _ID = value;
                    if (TextBody != null)
                        TextBody.ID = _ID;
                    if (Title != null)
                        Title.ID = _ID;
                    if (Summary != null)
                        Summary.ID = _ID;
                    if (Description != null)
                        Description.ID = _ID;
                }
                get => _ID;
            }

            /// <summary>
            /// Gets next unused entry ID.
            /// </summary>
            public int GetNextUnusedID()
            {
                var id = ID;
                if (TextBody != null)
                {
                    var entries = TextBodyInfo.GetPatchedEntries();
                    do
                    {
                        id++;
                    }
                    while (entries.Find(e => e.ID == id) != null);
                }
                else if (Title != null)
                {
                    var entries = TitleInfo.GetPatchedEntries();
                    do
                    {
                        id++;
                    }
                    while (entries.Find(e => e.ID == id) != null);
                }
                else if (Summary != null)
                {
                    var entries = SummaryInfo.GetPatchedEntries();
                    do
                    {
                        id++;
                    }
                    while (entries.Find(e => e.ID == id) != null);
                }
                else if (Description != null)
                {
                    var entries = DescriptionInfo.GetPatchedEntries();
                    do
                    {
                        id++;
                    }
                    while (entries.Find(e => e.ID == id) != null);
                }
                return id;
            }

            /// <summary>
            /// Sets ID of all entries to the next unused entry ID.
            /// </summary>
            public void SetNextUnusedID()
            {
                ID = GetNextUnusedID();
                return;
            }

            /// <summary>
            /// Inserts all entries into their assigned FMGs.
            /// </summary>
            public void InsertEntries(int index)
            {
                if (TextBody != null)
                {
                    TextBodyInfo.InsertEntry(index, TextBody);
                }
                if (Title != null)
                {
                    TitleInfo.InsertEntry(index, Title);
                }
                if (Summary != null)
                {
                    SummaryInfo.InsertEntry(index, Summary);
                }
                if (Description != null)
                {
                    DescriptionInfo.InsertEntry(index, Description);
                }
            }

            /// <summary>
            /// Duplicates all entries within their assigned FMGs.
            /// New entries are inserted into their assigned FMGs.
            /// </summary>
            /// <returns>The new EntryGroup</returns>
            public EntryGroup DuplicateEntries()
            {
                var index = GetIndex();
                if (index == -1)
                {
                    throw new Exception($"Could not find EntryGroup entries in assigned FMGs. {this}");
                }
                EntryGroup newGroup = new();
                if (TextBody != null)
                {
                    newGroup.TextBodyInfo = TextBodyInfo;
                    newGroup.TextBody = TextBodyInfo.CopyEntry(TextBody);
                    TextBodyInfo.InsertEntry(index, newGroup.TextBody);
                }
                if (Title != null)
                {
                    newGroup.TitleInfo = TitleInfo;
                    newGroup.Title = TitleInfo.CopyEntry(Title);
                    TitleInfo.InsertEntry(index, newGroup.Title);
                }
                if (Summary != null)
                {
                    newGroup.SummaryInfo = SummaryInfo;
                    newGroup.Summary = SummaryInfo.CopyEntry(Summary);
                    SummaryInfo.InsertEntry(index, newGroup.Summary);
                }
                if (Description != null)
                {
                    newGroup.DescriptionInfo = DescriptionInfo;
                    newGroup.Description = DescriptionInfo.CopyEntry(Description);
                    DescriptionInfo.InsertEntry(index, newGroup.Description);
                }
                newGroup.ID = ID;
                return newGroup;
            }

            /// <summary>
            /// Copies EntryGroup and returns 
            /// </summary>
            /// <returns>The new EntryGroup</returns>
            public EntryGroup CopyEntryGroup()
            {
                EntryGroup newGroup = new();
                if (TextBody != null)
                {
                    newGroup.TextBodyInfo = TextBodyInfo;
                    newGroup.TextBody = TextBodyInfo.CopyEntry(TextBody);
                }
                if (Title != null)
                {
                    newGroup.TitleInfo = TitleInfo;
                    newGroup.Title = TitleInfo.CopyEntry(Title);
                }
                if (Summary != null)
                {
                    newGroup.SummaryInfo = SummaryInfo;
                    newGroup.Summary = SummaryInfo.CopyEntry(Summary);
                }
                if (Description != null)
                {
                    newGroup.DescriptionInfo = DescriptionInfo;
                    newGroup.Description = DescriptionInfo.CopyEntry(Description);
                }
                return newGroup;
            }

            /// <summary>
            /// Removes all entries from their assigned FMGs.
            /// </summary>
            public void DeleteEntries()
            {
                if (TextBody != null)
                {
                    TextBodyInfo.DeleteEntry(TextBody);
                }
                if (Title != null)
                {
                    TitleInfo.DeleteEntry(Title);
                }
                if (Summary != null)
                {
                    SummaryInfo.DeleteEntry(Summary);
                }
                if (Description != null)
                {
                    DescriptionInfo.DeleteEntry(Description);
                }
            }

            /// <summary>
            /// Finds shared index of entries in FMGs.
            /// </summary>
            /// <returns>The zero-based index if found; otherwise -1.</returns>
            public int GetIndex()
            {
                if (TextBody != null)
                {
                    return TextBodyInfo.Fmg.Entries.FindIndex(e => e.ID == TextBody.ID);
                }
                else if (Title != null)
                {
                    return TitleInfo.Fmg.Entries.FindIndex(e => e.ID == Title.ID);
                }
                else if (Summary != null)
                {
                    return SummaryInfo.Fmg.Entries.FindIndex(e => e.ID == Summary.ID);
                }
                else if (Description != null)
                {
                    return DescriptionInfo.Fmg.Entries.FindIndex(e => e.ID == Description.ID);
                }
                return -1;
            }
        }

        internal static AssetLocator AssetLocator;
        public static bool IsLoaded { get; private set; } = false;
        public static bool IsLoading { get; private set; } = false;
        private static string _languageFolder = "";
        public static string LanguageFolder => _languageFolder;

        private static List<FMGInfo> _fmgInfoBank = new();
        public static List<FMGInfo> FmgInfoBank
        {
            get { return _fmgInfoBank; }
        }

        public static Dictionary<FmgUICategory, bool> ActiveUITypes { get; private set; } = new();

        /// <summary>
        /// FMG sections in UI
        /// </summary>
        public enum FmgUICategory
        {
            Text = 0,
            Item = 1,
            Menu = 2,
        }

        /// <summary>
        /// Entry type for Title, Summary, Description, or other.
        /// </summary>
        public enum FmgEntryTextType
        {
            TextBody = 0,
            Title = 1,
            Summary = 2,
            Description = 3,
            ExtraInfo = 4,
        }

        /// <summary>
        /// Text categories used for grouping multiple FMGs or broad identification
        /// </summary>
        public enum FmgEntryCategory
        {
            None,
            Goods,
            Weapons,
            Armor,
            Rings,
            Spells,
            Characters,
            Locations,
            Gem,
            Message,
            SwordArts,
            Effect,
            ActionButtonText,
        }

        /// <summary>
        /// BND IDs for FMG files used for identification
        /// </summary>
        public enum FmgIDType
        {
            // Note: Matching names with _DLC and _PATCH are used as identifiers for patch FMGs. This is a little dumb and patch fmg handling should probably be redone.
            None = -1,

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
            //
            TalkMsg = 1,
            BloodMsg = 2,
            MovieSubtitle = 3,
            Event = 30,
            MenuInGame = 70,
            MenuCommon = 76,
            MenuOther = 77,
            MenuDialog = 78,
            MenuKeyGuide = 79,
            MenuLineHelp = 80,
            MenuContext = 81,
            MenuTags = 90,
            Win32Tags = 91,
            Win32Messages = 92,
            Event_Patch = 101,
            MenuDialog_Patch = 102,
            Win32Messages_Patch = 103,
            NpcDialog_Patch = 104,
            BloodMessage_Patch = 107,
            MenuOneLine_Patch = 121,
            MenuKeyGuide_Patch = 122,
            MenuOther_Patch = 123,
            MenuCommon_Patch = 124,

            // DS1 _DLC
            DescriptionGoods_Patch = 100,
            DescriptionSpells_Patch = 105,
            DescriptionWeapons_Patch = 106,
            DescriptionArmor_Patch = 108,
            DescriptionRings_Patch = 109,
            SummaryGoods_Patch = 110,
            TitleGoods_Patch = 111,
            SummaryRings_Patch = 112,
            TitleRings_Patch = 113,
            SummaryWeapons_Patch = 114,
            TitleWeapons_Patch = 115,
            SummaryArmor_Patch = 116,
            TitleArmor_Patch = 117,
            TitleSpells_Patch = 118,
            TitleCharacters_Patch = 119,
            TitleLocations_Patch = 120,

            // DS3 _DLC1
            TitleWeapons_DLC1 = 211,
            TitleArmor_DLC1 = 212,
            TitleRings_DLC1 = 213,
            TitleSpells_DLC1 = 214,
            TitleCharacters_DLC1 = 215,
            TitleLocations_DLC1 = 216,
            SummaryGoods_DLC1 = 217,
            SummaryRings_DLC1 = 220,
            DescriptionGoods_DLC1 = 221,
            DescriptionWeapons_DLC1 = 222,
            DescriptionArmor_DLC1 = 223,
            DescriptionRings_DLC1 = 224,
            SummarySpells_DLC1 = 225,
            DescriptionSpells_DLC1 = 226,
            //
            Modern_MenuText = 200,
            Modern_LineHelp = 201,
            Modern_KeyGuide = 202,
            Modern_System_Message_win64 = 203,
            Modern_Dialogues = 204,
            TalkMsg_DLC1 = 230,
            Event_DLC1 = 231,
            Modern_MenuText_DLC1 = 232,
            Modern_LineHelp_DLC1 = 233,
            Modern_System_Message_win64_DLC1 = 235,
            Modern_Dialogues_DLC1 = 236,
            SystemMessage_PS4_DLC1 = 237,
            SystemMessage_XboxOne_DLC1 = 238,
            BloodMsg_DLC1 = 239,

            // DS3 _DLC2
            TitleGoods_DLC2 = 250,
            TitleWeapons_DLC2 = 251,
            TitleArmor_DLC2 = 252,
            TitleRings_DLC2 = 253,
            TitleSpells_DLC2 = 254,
            TitleCharacters_DLC2 = 255,
            TitleLocations_DLC2 = 256,
            SummaryGoods_DLC2 = 257,
            SummaryRings_DLC2 = 260,
            DescriptionGoods_DLC2 = 261,
            DescriptionWeapons_DLC2 = 262,
            DescriptionArmor_DLC2 = 263,
            DescriptionRings_DLC2 = 264,
            SummarySpells_DLC2 = 265,
            DescriptionSpells_DLC2 = 266,
            //
            TalkMsg_DLC2 = 270,
            Event_DLC2 = 271,
            Modern_MenuText_DLC2 = 272,
            Modern_LineHelp_DLC2 = 273,
            Modern_System_Message_win64_DLC2 = 275,
            Modern_Dialogues_DLC2 = 276,
            SystemMessage_PS4_DLC2 = 277,
            SystemMessage_XboxOne_DLC2 = 278,
            BloodMsg_DLC2 = 279,

            // SDT
            Skills = 40,

            // ER
            TitleGem = 35,
            SummaryGem = 36,
            DescriptionGem = 37,
            TitleMessage = 41,
            TitleSwordArts = 42,
            SummarySwordArts = 43,
            WeaponEffect = 44,
            ERUnk45 = 45,
            GoodsInfo2 = 46,
            //
            TalkMsg_FemalePC_Alt = 4,
            NetworkMessage = 31,
            EventTextForTalk = 33,
            EventTextForMap = 34,
            TutorialTitle = 207,
            TutorialBody = 208,
            TextEmbedImageName_win64 = 209,

            // Multiple use cases. Differences are applied in ApplyGameDifferences();
            ReusedFMG_32 = 32,
            // FMG 32
            // BB:  GemExtraInfo
            // DS3: ActionButtonText
            // SDT: ActionButtonText
            // ER:  ActionButtonText
            ReusedFMG_210 = 210,
            // FMG 210
            // DS3: TitleGoods_DLC1
            // SDT: ?
            // ER:  ToS_win64
            ReusedFMG_205 = 205,
            // FMG 205
            // DS3: SystemMessage_PS4
            // SDT: TutorialText
            // ER:  LoadingTitle
            ReusedFMG_206 = 206,
            // FMG 206
            // DS3: SystemMessage_XboxOne 
            // SDT: TutorialTitle
            // ER:  LoadingText
        }

        /// <summary>
        /// Get category for grouped entries (Goods, Weapons, etc)
        /// </summary>
        public static FmgEntryCategory GetFmgCategory(int id)
        {
            switch ((FmgIDType)id)
            {
                case FmgIDType.TitleTest:
                case FmgIDType.TitleTest2:
                case FmgIDType.TitleTest3:
                case FmgIDType.ERUnk45:
                    return FmgEntryCategory.None;

                case FmgIDType.DescriptionGoods:
                case FmgIDType.DescriptionGoods_Patch:
                case FmgIDType.DescriptionGoods_DLC1:
                case FmgIDType.DescriptionGoods_DLC2:
                case FmgIDType.SummaryGoods:
                case FmgIDType.SummaryGoods_Patch:
                case FmgIDType.SummaryGoods_DLC1:
                case FmgIDType.SummaryGoods_DLC2:
                case FmgIDType.TitleGoods:
                case FmgIDType.TitleGoods_Patch:
                case FmgIDType.TitleGoods_DLC2:
                    return FmgEntryCategory.Goods;

                case FmgIDType.DescriptionWeapons:
                case FmgIDType.DescriptionWeapons_DLC1:
                case FmgIDType.DescriptionWeapons_DLC2:
                case FmgIDType.SummaryWeapons:
                case FmgIDType.TitleWeapons:
                case FmgIDType.TitleWeapons_DLC1:
                case FmgIDType.TitleWeapons_DLC2:
                case FmgIDType.DescriptionWeapons_Patch:
                case FmgIDType.SummaryWeapons_Patch:
                case FmgIDType.TitleWeapons_Patch:
                    return FmgEntryCategory.Weapons;

                case FmgIDType.DescriptionArmor:
                case FmgIDType.DescriptionArmor_DLC1:
                case FmgIDType.DescriptionArmor_DLC2:
                case FmgIDType.SummaryArmor:
                case FmgIDType.TitleArmor:
                case FmgIDType.TitleArmor_DLC1:
                case FmgIDType.TitleArmor_DLC2:
                case FmgIDType.DescriptionArmor_Patch:
                case FmgIDType.SummaryArmor_Patch:
                case FmgIDType.TitleArmor_Patch:
                    return FmgEntryCategory.Armor;

                case FmgIDType.DescriptionRings:
                case FmgIDType.DescriptionRings_DLC1:
                case FmgIDType.DescriptionRings_DLC2:
                case FmgIDType.SummaryRings:
                case FmgIDType.SummaryRings_DLC1:
                case FmgIDType.SummaryRings_DLC2:
                case FmgIDType.TitleRings:
                case FmgIDType.TitleRings_DLC1:
                case FmgIDType.TitleRings_DLC2:
                case FmgIDType.DescriptionRings_Patch:
                case FmgIDType.SummaryRings_Patch:
                case FmgIDType.TitleRings_Patch:
                    return FmgEntryCategory.Rings;

                case FmgIDType.DescriptionSpells:
                case FmgIDType.DescriptionSpells_DLC1:
                case FmgIDType.DescriptionSpells_DLC2:
                case FmgIDType.SummarySpells:
                case FmgIDType.SummarySpells_DLC1:
                case FmgIDType.SummarySpells_DLC2:
                case FmgIDType.TitleSpells:
                case FmgIDType.TitleSpells_DLC1:
                case FmgIDType.TitleSpells_DLC2:
                case FmgIDType.DescriptionSpells_Patch:
                case FmgIDType.TitleSpells_Patch:
                    return FmgEntryCategory.Spells;

                case FmgIDType.TitleCharacters:
                case FmgIDType.TitleCharacters_DLC1:
                case FmgIDType.TitleCharacters_DLC2:
                case FmgIDType.TitleCharacters_Patch:
                    return FmgEntryCategory.Characters;

                case FmgIDType.TitleLocations:
                case FmgIDType.TitleLocations_DLC1:
                case FmgIDType.TitleLocations_DLC2:
                case FmgIDType.TitleLocations_Patch:
                    return FmgEntryCategory.Locations;

                case FmgIDType.TitleGem:
                case FmgIDType.SummaryGem:
                case FmgIDType.DescriptionGem:
                    return FmgEntryCategory.Gem;

                case FmgIDType.TitleSwordArts:
                case FmgIDType.SummarySwordArts:
                    return FmgEntryCategory.SwordArts;

                case FmgIDType.TitleMessage:
                    return FmgEntryCategory.Message;

                case FmgIDType.WeaponEffect:
                default:
                    return FmgEntryCategory.None;
            }
        }

        /// <summary>
        /// Get entry text type (such as weapon Title, Summary, Description)
        /// </summary>
        public static FmgEntryTextType GetFmgTextType(int id)
        {
            switch ((FmgIDType)id)
            {
                case FmgIDType.DescriptionGoods:
                case FmgIDType.DescriptionGoods_DLC1:
                case FmgIDType.DescriptionGoods_DLC2:
                case FmgIDType.DescriptionWeapons:
                case FmgIDType.DescriptionWeapons_DLC1:
                case FmgIDType.DescriptionWeapons_DLC2:
                case FmgIDType.DescriptionArmor:
                case FmgIDType.DescriptionArmor_DLC1:
                case FmgIDType.DescriptionArmor_DLC2:
                case FmgIDType.DescriptionRings:
                case FmgIDType.DescriptionRings_DLC1:
                case FmgIDType.DescriptionRings_DLC2:
                case FmgIDType.DescriptionSpells:
                case FmgIDType.DescriptionSpells_DLC1:
                case FmgIDType.DescriptionSpells_DLC2:
                case FmgIDType.DescriptionArmor_Patch:
                case FmgIDType.DescriptionGoods_Patch:
                case FmgIDType.DescriptionRings_Patch:
                case FmgIDType.DescriptionSpells_Patch:
                case FmgIDType.DescriptionWeapons_Patch:
                case FmgIDType.DescriptionGem:
                    return FmgEntryTextType.Description;

                case FmgIDType.SummaryGoods:
                case FmgIDType.SummaryGoods_DLC1:
                case FmgIDType.SummaryGoods_DLC2:
                case FmgIDType.SummaryWeapons:
                case FmgIDType.SummaryArmor:
                case FmgIDType.SummaryRings:
                case FmgIDType.SummaryRings_DLC1:
                case FmgIDType.SummaryRings_DLC2:
                case FmgIDType.SummarySpells:
                case FmgIDType.SummarySpells_DLC1:
                case FmgIDType.SummarySpells_DLC2:
                case FmgIDType.SummaryArmor_Patch:
                case FmgIDType.SummaryGoods_Patch:
                case FmgIDType.SummaryRings_Patch:
                case FmgIDType.SummaryWeapons_Patch:
                case FmgIDType.SummaryGem:
                case FmgIDType.SummarySwordArts:
                    return FmgEntryTextType.Summary;

                case FmgIDType.TitleGoods:
                case FmgIDType.TitleGoods_DLC2:
                case FmgIDType.TitleWeapons:
                case FmgIDType.TitleWeapons_DLC1:
                case FmgIDType.TitleWeapons_DLC2:
                case FmgIDType.TitleArmor:
                case FmgIDType.TitleArmor_DLC1:
                case FmgIDType.TitleArmor_DLC2:
                case FmgIDType.TitleRings:
                case FmgIDType.TitleRings_DLC1:
                case FmgIDType.TitleRings_DLC2:
                case FmgIDType.TitleSpells:
                case FmgIDType.TitleSpells_DLC1:
                case FmgIDType.TitleSpells_DLC2:
                case FmgIDType.TitleCharacters:
                case FmgIDType.TitleCharacters_DLC1:
                case FmgIDType.TitleCharacters_DLC2:
                case FmgIDType.TitleLocations:
                case FmgIDType.TitleLocations_DLC1:
                case FmgIDType.TitleLocations_DLC2:
                case FmgIDType.TitleTest:
                case FmgIDType.TitleTest2:
                case FmgIDType.TitleTest3:
                case FmgIDType.TitleArmor_Patch:
                case FmgIDType.TitleCharacters_Patch:
                case FmgIDType.TitleGoods_Patch:
                case FmgIDType.TitleLocations_Patch:
                case FmgIDType.TitleRings_Patch:
                case FmgIDType.TitleSpells_Patch:
                case FmgIDType.TitleWeapons_Patch:
                case FmgIDType.TitleGem:
                case FmgIDType.TitleMessage:
                case FmgIDType.TitleSwordArts:
                    return FmgEntryTextType.Title;

                case FmgIDType.ERUnk45:
                // TODO: implement these two into a 4th text slot? Figure out how where they get used.
                case FmgIDType.WeaponEffect:
                case FmgIDType.GoodsInfo2:
                    return FmgEntryTextType.TextBody;
                default:
                    return FmgEntryTextType.TextBody;
            }
        }

        /// <summary>
        /// List of strings to compare with "FmgIDType" name to identify patch FMGs.
        /// </summary>
        private readonly static List<string> patchStrings = new()
        {
            "_Patch",
            "_DLC1",
            "_DLC2",
        };

        /// <summary>
        /// Removes patch/DLC identifiers from strings for the purposes of finding patch FMGs. Kinda dumb.
        /// </summary>
        private static string RemovePatchStrings(string str)
        {
            foreach (var badString in patchStrings)
            {
                str = str.Replace(badString, "", StringComparison.CurrentCultureIgnoreCase);
            }
            return str;
        }

        private static FMGInfo GenerateFMGInfo(BinderFile file)
        {
            var fmg = FMG.Read(file.Bytes);
            var name = Enum.GetName(typeof(FmgIDType), file.ID);
            FMGInfo info = new()
            {
                FileName = file.Name.Split("\\").Last(),
                Name = name,
                FmgID = (FmgIDType)file.ID,
                Fmg = fmg,
                EntryType = GetFmgTextType(file.ID),
                EntryCategory = GetFmgCategory(file.ID)
            };
            switch (info.EntryCategory)
            {
                case FmgEntryCategory.Goods:
                case FmgEntryCategory.Weapons:
                case FmgEntryCategory.Armor:
                case FmgEntryCategory.Rings:
                case FmgEntryCategory.Gem:
                case FmgEntryCategory.SwordArts:
                    info.UICategory = FmgUICategory.Item;
                    info.GroupedEntry = true;
                    break;
                default:
                    info.UICategory = FmgUICategory.Menu;
                    break;
            }

            foreach (var parentInfo in _fmgInfoBank)
            {
                if (parentInfo.Name == RemovePatchStrings(info.FmgID.ToString()))
                {
                    // Patch FMG found
                    info.PatchParent = parentInfo;
                    parentInfo.PatchChildren.Add(info);
                }
            }

            ActiveUITypes[info.UICategory] = true;

            return info;
        }

        private static void SetFMGInfoDS2(string file)
        {
            // TODO: DS2 FMG grouping & UI sorting (copy SetFMGInfo)
            var fmg = FMG.Read(file);
            var name = Path.GetFileNameWithoutExtension(file);
            FMGInfo info = new()
            {
                FileName = file.Split("\\").Last(),
                Name = name,
                FmgID = FmgIDType.None,
                Fmg = fmg,
                EntryType = FmgEntryTextType.TextBody,
                EntryCategory = FmgEntryCategory.None,
                UICategory = FmgUICategory.Text,
            };
            ActiveUITypes[info.UICategory] = true;
            _fmgInfoBank.Add(info);
        }

        /// <summary>
        /// Loads item and menu MsgBnds from paths, generates FMGInfo, and fills FmgInfoBank.
        /// </summary>
        /// <returns>True if successful; false otherwise.</returns>
        private static bool LoadItemMenuMsgBnds(AssetDescription itemMsgPath, AssetDescription menuMsgPath)
        {
            if (!LoadMsgBnd(itemMsgPath.AssetPath, "item.msgbnd")
                || !LoadMsgBnd(menuMsgPath.AssetPath, "menu.msgbnd"))
            {
                return false;
            }

            foreach (var info in _fmgInfoBank)
                ApplyGameDifferences(info);

            _fmgInfoBank = _fmgInfoBank.OrderBy(e => e.Name).ToList();
            HandleDuplicateEntries();

            return true;
        }
        /// <summary>
        /// Loads MsgBnd from path, generates FMGInfo, and fills FmgInfoBank.
        /// </summary>
        /// <returns>True if successful; false otherwise.</returns>
        private static bool LoadMsgBnd(string path, string msgBndType = "UNDEFINED")
        {
            if (path == null)
            {
                if (_languageFolder == "")
                {
                    // Default language folder could not be found.
                }
                else
                {
                    MessageBox.Show($"Could not find {msgBndType} in language folder \"{_languageFolder}\".\nText data will not be loaded.", "Error");
                }
                
                IsLoaded = false;
                IsLoading = false;
                return false;
            }

            IBinder fmgBinder;
            if (AssetLocator.Type == GameType.DemonsSouls || AssetLocator.Type == GameType.DarkSoulsPTDE || AssetLocator.Type == GameType.DarkSoulsRemastered)
            {
                fmgBinder = BND3.Read(path);
            }
            else
            {
                fmgBinder = BND4.Read(path);
            }

            foreach (var file in fmgBinder.Files)
                _fmgInfoBank.Add(GenerateFMGInfo(file));

            return true;
        }

        public static void ReloadFMGs(string languageFolder = "")
        {
            try
            {
                _languageFolder = languageFolder;
                IsLoaded = false;
                IsLoading = true;

                ActiveUITypes.Clear();
                foreach (var e in Enum.GetValues(typeof(FmgUICategory)))
                {
                    ActiveUITypes.Add((FmgUICategory)e, false);
                }

                //TaskManager.Run("FB:Reload", true, false, true, () =>
                if (AssetLocator.Type == GameType.Undefined)
                {
                    return;
                }

                if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    ReloadDS2FMGs(ref _languageFolder);
                    IsLoading = false;
                    IsLoaded = true;
                    return;
                }

                var itemMsgPath = AssetLocator.GetItemMsgbnd(ref _languageFolder);
                var menuMsgPath = AssetLocator.GetMenuMsgbnd(ref _languageFolder);

                _fmgInfoBank.Clear();
                if (!LoadItemMenuMsgBnds(itemMsgPath, menuMsgPath))
                {
                    _fmgInfoBank.Clear();
                    IsLoaded = false;
                    IsLoading = false;
                    return;
                }
                IsLoaded = true;
                IsLoading = false;
            }
            catch (Exception e) when (e is DirectoryNotFoundException or FileNotFoundException)
            {
                _fmgInfoBank.Clear();
                IsLoaded = false;
                IsLoading = false;
            }
        }

        private static void ReloadDS2FMGs(ref string languageFolder)
        {
            var desc = AssetLocator.GetItemMsgbnd(ref languageFolder, true);
            var files = Directory.GetFileSystemEntries($@"{AssetLocator.GameRootDirectory}\{desc.AssetPath}", @"*.fmg").ToList();
            _fmgInfoBank.Clear();
            foreach (var file in files)
            {
                var modfile = $@"{AssetLocator.GameModDirectory}\{desc.AssetPath}\{Path.GetFileName(file)}";
                if (AssetLocator.GameModDirectory != null && File.Exists(modfile))
                {
                    var fmg = FMG.Read(modfile);
                    SetFMGInfoDS2(modfile);
                }
                else
                {
                    var fmg = FMG.Read(file);
                    SetFMGInfoDS2(file);
                }
            }
            _fmgInfoBank = _fmgInfoBank.OrderBy(e => e.Name).ToList();
            HandleDuplicateEntries();
        }

        /// <summary>
        /// Checks and applies FMG info that differs per-game.
        /// </summary>
        private static void ApplyGameDifferences(FMGInfo info)
        {
            var gameType = AssetLocator.Type;
            switch (info.FmgID)
            {
                case FmgIDType.ReusedFMG_32:
                    if (gameType == GameType.Bloodborne)
                    {
                        info.Name = "GemExtraInfo";
                        info.UICategory = FmgUICategory.Item;
                        info.EntryCategory = FmgEntryCategory.Gem;
                        //info.EntryType = FmgEntryTextType.ExtraInfo; // TODO
                    }
                    else
                    {
                        info.Name = "ActionButtonText";
                        info.EntryCategory = FmgEntryCategory.ActionButtonText;
                    }
                    break;
                case FmgIDType.Event:
                case FmgIDType.Event_Patch:
                    if (gameType is GameType.DemonsSouls or GameType.DarkSoulsPTDE or GameType.DarkSoulsRemastered or GameType.Bloodborne)
                        info.EntryCategory = FmgEntryCategory.ActionButtonText;
                    break;
                case FmgIDType.ReusedFMG_205:
                    if (gameType == GameType.EldenRing)
                        info.Name = "LoadingTitle";
                    else if (gameType == GameType.Sekiro)
                        info.Name = "LoadingText";
                    else
                        info.Name = "SystemMessage_PS4";
                    break;
                case FmgIDType.ReusedFMG_206:
                    if (gameType == GameType.EldenRing)
                        info.Name = "LoadingText";
                    else if (gameType == GameType.Sekiro)
                        info.Name = "LoadingTitle";
                    else
                        info.Name = "SystemMessage_XboxOne";
                    break;
                case FmgIDType.ReusedFMG_210:
                    if (gameType == GameType.EldenRing)
                    {
                        info.Name = "ToS_win64";
                        info.UICategory = FmgUICategory.Menu;
                        info.EntryType = FmgEntryTextType.TextBody;
                        info.EntryCategory = FmgEntryCategory.None;
                    }
                    else
                    {
                        info.Name = "TitleGoods_DLC1";
                        info.UICategory = FmgUICategory.Item;
                        info.EntryType = FmgEntryTextType.Title;
                        info.EntryCategory = FmgEntryCategory.Goods;
                        var parent = _fmgInfoBank.Find(e => e.FmgID == FmgIDType.TitleGoods);
                        info.PatchParent = parent;
                        parent.PatchChildren.Add(info);
                    }
                    break;
            }
            return;
        }

        /// <summary>
        /// Get patched FMG Entries for the specified category, with TextType Title or TextBody.
        /// </summary>
        /// <returns>List of patched entries if found; empty list otherwise.</returns>
        public static List<FMG.Entry> GetFmgEntriesByCategory(FmgEntryCategory category, bool sort = true)
        {
            foreach (var info in _fmgInfoBank)
            {
                if (info.EntryCategory == category && info.EntryType is FmgEntryTextType.Title or FmgEntryTextType.TextBody)
                    return info.GetPatchedEntries(sort);
            }
            return new List<FMG.Entry>();
        }

        /// <summary>
        /// Get patched FMG Entries for the specified category and text type.
        /// </summary>
        /// <returns>List of patched entries if found; empty list otherwise.</returns>
        public static List<FMG.Entry> GetFmgEntriesByCategoryAndTextType(FmgEntryCategory category, FmgEntryTextType textType, bool sort = true)
        {
            foreach (var info in _fmgInfoBank)
            {
                if (info.EntryCategory == category && info.EntryType == textType)
                    return info.GetPatchedEntries(sort);
            }
            return new List<FMG.Entry>();
        }

        /// <summary>
        /// Get patched FMG Entries for the specified FmgIDType.
        /// </summary>
        /// <returns>List of patched entries if found; empty list otherwise.</returns>
        public static List<FMG.Entry> GetFmgEntriesByFmgIDType(FmgIDType fmgID, bool sort = true)
        {
            foreach (var info in _fmgInfoBank)
            {
                if (info.FmgID == fmgID)
                    return info.GetPatchedEntries(sort);
            }
            return new List<FMG.Entry>();
        }

        /// <summary>
        /// Generate a new EntryGroup using a given ID and FMGInfo.
        /// Data is updated using FMGInfo PatchChildren.
        /// </summary>
        public static EntryGroup GenerateEntryGroup(int id, FMGInfo fmgInfo)
        {
            EntryGroup eGroup = new()
            {
                ID = id,
            };
            foreach (var info in _fmgInfoBank)
            {
                if (info.EntryCategory == fmgInfo.EntryCategory && info.PatchParent == null)
                {
                    var entryPairs = info.GetPatchedEntryFMGPairs();
                    var pair = entryPairs.Find(e => e.Entry.ID == id);
                    if (pair != null)
                    {
                        switch (info.EntryType)
                        {
                            case FmgEntryTextType.Title:
                                eGroup.Title = pair.Entry;
                                eGroup.TitleInfo = pair.FmgInfo;
                                break;
                            case FmgEntryTextType.Summary:
                                eGroup.Summary = pair.Entry;
                                eGroup.SummaryInfo = pair.FmgInfo;
                                break;
                            case FmgEntryTextType.Description:
                                eGroup.Description = pair.Entry;
                                eGroup.DescriptionInfo = pair.FmgInfo;
                                break;
                            case FmgEntryTextType.TextBody:
                                eGroup.TextBody = pair.Entry;
                                eGroup.TextBodyInfo = pair.FmgInfo;
                                break;
                        }
                    }
                }
            }
            return eGroup;
        }

        private static void HandleDuplicateEntries()
        {
            bool askedAboutDupes = false;
            bool ignoreDupes = true;
            foreach (var info in _fmgInfoBank)
            {
                var dupes = info.Fmg.Entries.GroupBy(e => e.ID).SelectMany(g => g.SkipLast(1));
                if (dupes.Any())
                {
                    if (!askedAboutDupes && MessageBox.Show("Duplicate Text Entries within the same FMG have been found.\n\nRemove all duplicates? (Latest entries are kept)", "Duplicate Text Entries", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ignoreDupes = false;
                    }
                    askedAboutDupes = true;

                    if (!ignoreDupes)
                    {
                        foreach (var dupe in dupes)
                        {
                            info.Fmg.Entries.Remove(dupe);
                        }
                    }
                }
            }
        }

        private class JsonFMG
        {
            public FmgIDType FmgID;
            public FMG Fmg;
            public JsonFMG(FmgIDType fmg_id, FMG fmg)
            {
                FmgID = fmg_id;
                Fmg = fmg;
            }
        }

        private static string FormatJson(string json)
        {
            json = json.Replace("{\"ID\"", "\r\n{\"ID\"");
            json = json.Replace("],", "\r\n],");
            return json;
        }

        public static bool ExportFMGs()
        {
            FolderBrowserDialog folderDialog = new();
            folderDialog.UseDescriptionForTitle = true;
            folderDialog.Description = "Choose Export Folder";
            if (folderDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            var path = folderDialog.SelectedPath;
            int filecount = 0;
            if (AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                Directory.CreateDirectory(path);

                foreach (var info in _fmgInfoBank)
                {
                    var fmgPair = new JsonFMG(info.FmgID, info.Fmg);
                    var json = JsonConvert.SerializeObject(fmgPair, Formatting.None);
                    json = FormatJson(json);

                    var fileName = info.Name;
                    if (CFG.Current.FMG_ShowOriginalNames)
                        fileName = info.FileName;

                    File.WriteAllText($@"{path}\{fileName}.fmg.json", json);

                    filecount++;
                }
            }
            else
            {
                var itemPath = $@"{path}\Item Text";
                var menuPath = $@"{path}\Menu Text";
                Directory.CreateDirectory(itemPath);
                Directory.CreateDirectory(menuPath);
                foreach (var info in _fmgInfoBank)
                {
                    if (info.UICategory == FmgUICategory.Item)
                    {
                        path = itemPath;
                    }
                    else if (info.UICategory == FmgUICategory.Menu)
                    {
                        path = menuPath;
                    }
                    var fmgPair = new JsonFMG(info.FmgID, info.Fmg);
                    var json = JsonConvert.SerializeObject(fmgPair, Formatting.None);
                    json = FormatJson(json);

                    var fileName = info.Name;
                    if (CFG.Current.FMG_ShowOriginalNames)
                        fileName = info.FileName;

                    File.WriteAllText($@"{path}\{fileName}.fmg.json", json);

                    filecount++;
                }
            }
            MessageBox.Show($"Exported {filecount} text files", "Finished", MessageBoxButtons.OK);
            return true;
        }

        public static bool ImportFMGs()
        {
            OpenFileDialog fileDialog = new();
            fileDialog.Title = "Choose Files to Import";
            fileDialog.Filter = "Exported FMGs|*.fmg.json|All files|*.*";
            fileDialog.Multiselect = true;
            if (fileDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            var files = fileDialog.FileNames;

            if (files.Length == 0)
            {
                return false;
            }

            int filecount = 0;
            foreach (var filePath in files)
            {
                try
                {
                    var file = File.ReadAllText(filePath);
                    var json = JsonConvert.DeserializeObject<JsonFMG>(@file);
                    bool success = false;
                    foreach (var info in _fmgInfoBank)
                    {
                        if (info.FmgID == json.FmgID)
                        {
                            info.Fmg = json.Fmg;
                            success = true;
                            filecount++;
                            break;
                        }
                    }
                    if (!success)
                    {
                        MessageBox.Show($"Couldn't locate FMG using FMG ID `{json.FmgID}`", "Import Error", MessageBoxButtons.OK);
                    }
                }
                catch (JsonReaderException e)
                {
                    MessageBox.Show($"{e.Message}\n\nCouldn't import '{filePath}'", "Import Error", MessageBoxButtons.OK);
                }
            }

            if (filecount == 0)
                return false;

            HandleDuplicateEntries();
            MessageBox.Show($"Imported {filecount} text files", "Finished", MessageBoxButtons.OK);
            return true;
        }

        private static void SaveFMGsDS2()
        {
            foreach (var info in _fmgInfoBank)
            {
                Utils.WriteWithBackup(AssetLocator.GameRootDirectory, AssetLocator.GameModDirectory, $@"menu\text\{_languageFolder}\{info.Name}.fmg", info.Fmg);
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
            IBinder fmgBinderItem;
            IBinder fmgBinderMenu;
            var itemMsgPath = AssetLocator.GetItemMsgbnd(ref _languageFolder);
            var menuMsgPath = AssetLocator.GetMenuMsgbnd(ref _languageFolder);
            if (AssetLocator.Type == GameType.DemonsSouls || AssetLocator.Type == GameType.DarkSoulsPTDE || AssetLocator.Type == GameType.DarkSoulsRemastered)
            {
                fmgBinderItem = BND3.Read(itemMsgPath.AssetPath);
                fmgBinderMenu = BND3.Read(menuMsgPath.AssetPath);
            }
            else
            {
                fmgBinderItem = BND4.Read(itemMsgPath.AssetPath);
                fmgBinderMenu = BND4.Read(menuMsgPath.AssetPath);
            }

            foreach (var file in fmgBinderItem.Files)
            {
                var info = _fmgInfoBank.Find(e => e.FmgID == (FmgIDType)file.ID);
                if (info != null)
                {
                    file.Bytes = info.Fmg.Write();
                }
            }

            foreach (var file in fmgBinderMenu.Files)
            {
                var info = _fmgInfoBank.Find(e => e.FmgID == (FmgIDType)file.ID);
                if (info != null)
                {
                    file.Bytes = info.Fmg.Write();
                }
            }

            var itemMsgPathDest = AssetLocator.GetItemMsgbnd(ref _languageFolder, true);
            var menuMsgPathDest = AssetLocator.GetMenuMsgbnd(ref _languageFolder, true);
            if (fmgBinderItem is BND3 bnd3)
            {
                Utils.WriteWithBackup(AssetLocator.GameRootDirectory,
                    AssetLocator.GameModDirectory, itemMsgPathDest.AssetPath, bnd3);
                Utils.WriteWithBackup(AssetLocator.GameRootDirectory,
                    AssetLocator.GameModDirectory, menuMsgPathDest.AssetPath, (BND3)fmgBinderMenu);
            }
            else if (fmgBinderItem is BND4 bnd4)
            {
                Utils.WriteWithBackup(AssetLocator.GameRootDirectory,
                    AssetLocator.GameModDirectory, itemMsgPathDest.AssetPath, bnd4);
                Utils.WriteWithBackup(AssetLocator.GameRootDirectory,
                    AssetLocator.GameModDirectory, menuMsgPathDest.AssetPath, (BND4)fmgBinderMenu);
            }
        }

        public static void SetAssetLocator(AssetLocator l)
        {
            AssetLocator = l;
            //ReloadFMGs();
        }
    }
}
