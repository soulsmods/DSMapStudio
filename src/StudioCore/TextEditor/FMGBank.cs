using Microsoft.Extensions.Logging;
using SoulsFormats;
using StudioCore.Editor;
using StudioCore.MsbEditor;
using StudioCore.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudioCore.TextEditor;

/// <summary>
///     Static class that stores all the strings for a Souls game.
/// </summary>
public partial class FMGBank
{
    /// <summary>
    ///     List of strings to compare with "FmgIDType" name to identify patch FMGs.
    /// </summary>
    private static readonly List<string> patchStrings = new() { "_Patch", "_DLC1", "_DLC2" };


    private Project Project;

    public FMGBank(Project project)
    {
        this.Project = project;
    }

    public bool IsLoaded { get; private set; }
    public bool IsLoading { get; private set; }
    public string LanguageFolder { get; private set; } = "";

    public List<FMGInfo> FmgInfoBank { get; private set; } = new();

    public Dictionary<FmgUICategory, bool> ActiveUITypes { get; private set; } = new();
    /// <summary>
    ///     Removes patch/DLC identifiers from strings for the purposes of finding patch FMGs. Kinda dumb.
    /// </summary>
    private static string RemovePatchStrings(string str)
    {
        foreach (var badString in patchStrings)
        {
            str = str.Replace(badString, "", StringComparison.CurrentCultureIgnoreCase);
        }

        return str;
    }

    private FMGInfo GenerateFMGInfo(BinderFile file)
    {
        FMG fmg = FMG.Read(file.Bytes);
        var name = Enum.GetName(typeof(FmgIDType), file.ID);
        FMGInfo info = new()
        {
            Owner = this,
            FileName = file.Name.Split("\\").Last(),
            Name = name,
            FmgID = (FmgIDType)file.ID,
            Fmg = fmg,
            EntryType = FMGEnums.GetFmgTextType(file.ID),
            EntryCategory = FMGEnums.GetFmgCategory(file.ID)
        };
        info.UICategory = FMGEnums.GetFMGUICategory(info.EntryCategory);

        ActiveUITypes[info.UICategory] = true;

        return info;
    }

    private void SetFMGInfoDS2(string file)
    {
        // TODO: DS2 FMG grouping & UI sorting (copy SetFMGInfo)
        FMG fmg = FMG.Read(file);
        var name = Path.GetFileNameWithoutExtension(file);
        FMGInfo info = new()
        {
            Owner = this,
            FileName = file.Split("\\").Last(),
            Name = name,
            FmgID = FmgIDType.None,
            Fmg = fmg,
            EntryType = FmgEntryTextType.TextBody,
            EntryCategory = FmgEntryCategory.None,
            UICategory = FmgUICategory.Text
        };
        ActiveUITypes[info.UICategory] = true;
        FmgInfoBank.Add(info);
    }

    /// <summary>
    ///     Loads item and menu MsgBnds from paths, generates FMGInfo, and fills FmgInfoBank.
    /// </summary>
    /// <returns>True if successful; false otherwise.</returns>
    private bool LoadItemMenuMsgBnds(AssetDescription itemMsgPath, AssetDescription menuMsgPath)
    {
        if (!LoadMsgBnd(itemMsgPath.AssetPath, "item.msgbnd")
            || !LoadMsgBnd(menuMsgPath.AssetPath, "menu.msgbnd"))
        {
            return false;
        }

        foreach (FMGInfo info in FmgInfoBank)
        {
            ApplyGameDifferences(info);
            if (CFG.Current.FMG_NoGroupedFmgEntries)
            {
                info.EntryType = FmgEntryTextType.TextBody;
            }
        }

        if (!CFG.Current.FMG_NoFmgPatching)
        {
            foreach (FMGInfo info in FmgInfoBank)
            {
                SetFMGInfoPatchParent(info);
            }
        }

        if (CFG.Current.FMG_NoGroupedFmgEntries)
        {
            FmgInfoBank = FmgInfoBank.OrderBy(e => e.EntryCategory).ThenBy(e => e.FmgID).ToList();
        }
        else
        {
            FmgInfoBank = FmgInfoBank.OrderBy(e => e.Name).ToList();
        }

        HandleDuplicateEntries();

        return true;
    }

    /// <summary>
    ///     Loads MsgBnd from path, generates FMGInfo, and fills FmgInfoBank.
    /// </summary>
    /// <returns>True if successful; false otherwise.</returns>
    private bool LoadMsgBnd(string path, string msgBndType = "UNDEFINED")
    {
        if (path == null)
        {
            if (LanguageFolder != "")
            {
                TaskLogs.AddLog(
                    $"Could locate text data files when looking for \"{msgBndType}\" in \"{LanguageFolder}\" folder",
                    LogLevel.Warning);
            }
            else
            {
                TaskLogs.AddLog(
                    $"Could not locate text data files when looking for \"{msgBndType}\" in Default English folder",
                    LogLevel.Warning);
            }

            IsLoaded = false;
            IsLoading = false;
            return false;
        }

        IBinder fmgBinder;
        if (Project.AssetLocator.Type == GameType.DemonsSouls || Project.AssetLocator.Type == GameType.DarkSoulsPTDE ||
            Project.AssetLocator.Type == GameType.DarkSoulsRemastered)
        {
            fmgBinder = BND3.Read(path);
        }
        else
        {
            fmgBinder = BND4.Read(path);
        }

        foreach (BinderFile file in fmgBinder.Files)
        {
            FmgInfoBank.Add(GenerateFMGInfo(file));
        }

        fmgBinder.Dispose();
        return true;
    }

    public void ReloadFMGs(string languageFolder = "")
    {
        TaskManager.Run(new TaskManager.LiveTask("FMG - Load Text", TaskManager.RequeueType.WaitThenRequeue, true,
            () =>
            {
                IsLoaded = false;
                IsLoading = true;

                LanguageFolder = languageFolder;

                ActiveUITypes = new Dictionary<FmgUICategory, bool>();
                foreach (var e in Enum.GetValues(typeof(FmgUICategory)))
                {
                    ActiveUITypes.Add((FmgUICategory)e, false);
                }

                if (Project.AssetLocator.Type == GameType.Undefined)
                {
                    return;
                }

                if (Project.AssetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    if (ReloadDS2FMGs())
                    {
                        IsLoading = false;
                        IsLoaded = true;
                    }

                    return;
                }

                SetDefaultLanguagePath();

                AssetDescription itemMsgPath = Project.AssetLocator.GetItemMsgbnd(LanguageFolder);
                AssetDescription menuMsgPath = Project.AssetLocator.GetMenuMsgbnd(LanguageFolder);

                FmgInfoBank = new List<FMGInfo>();
                if (!LoadItemMenuMsgBnds(itemMsgPath, menuMsgPath))
                {
                    FmgInfoBank = new List<FMGInfo>();
                    IsLoaded = false;
                    IsLoading = false;
                    return;
                }

                IsLoaded = true;
                IsLoading = false;
            }));
    }

    private bool ReloadDS2FMGs()
    {
        SetDefaultLanguagePath();
        AssetDescription desc = Project.AssetLocator.GetItemMsgbnd(LanguageFolder, true);

        if (desc.AssetPath == null)
        {
            if (LanguageFolder != "")
            {
                TaskLogs.AddLog($"Could not locate text data files when using \"{LanguageFolder}\" folder",
                    LogLevel.Warning);
            }
            else
            {
                TaskLogs.AddLog("Could not locate text data files when using Default English folder",
                    LogLevel.Warning);
            }

            IsLoaded = false;
            IsLoading = false;
            return false;
        }

        IEnumerable<string> files = Project.AssetLocator.GetAllAssets($@"{desc.AssetPath}", [@"*.fmg"]);
        FmgInfoBank = new List<FMGInfo>();
        foreach (var file in files)
        {
            FMG fmg = FMG.Read(file);
            SetFMGInfoDS2(file);
        }

        FmgInfoBank = FmgInfoBank.OrderBy(e => e.Name).ToList();
        HandleDuplicateEntries();
        return true;
    }

    private void SetDefaultLanguagePath()
    {
        if (LanguageFolder == "")
        {
            // By default, try to find path to English folder.
            foreach (KeyValuePair<string, string> lang in Project.AssetLocator.GetMsgLanguages())
            {
                var folder = lang.Value.Split("\\").Last();
                if (folder.Contains("eng", StringComparison.CurrentCultureIgnoreCase))
                {
                    LanguageFolder = folder;
                    break;
                }
            }
        }
    }

    private void SetFMGInfoPatchParent(FMGInfo info)
    {
        var strippedName = RemovePatchStrings(info.Name);
        if (strippedName != info.Name)
        {
            // This is a patch FMG, try to find parent FMG.
            foreach (FMGInfo parentInfo in FmgInfoBank)
            {
                if (parentInfo.Name == strippedName)
                {
                    info.AddParent(parentInfo);
                    return;
                }
            }

            TaskLogs.AddLog($"Couldn't find patch parent for FMG \"{info.Name}\" with ID {info.FmgID}",
                LogLevel.Error);
        }
    }

    /// <summary>
    ///     Checks and applies FMG info that differs per-game.
    /// </summary>
    private void ApplyGameDifferences(FMGInfo info)
    {
        GameType gameType = Project.AssetLocator.Type;
        switch (info.FmgID)
        {
            case FmgIDType.ReusedFMG_32:
                if (gameType == GameType.Bloodborne)
                {
                    info.Name = "GemExtraInfo";
                    info.UICategory = FmgUICategory.Item;
                    info.EntryCategory = FmgEntryCategory.Gem;
                    info.EntryType = FmgEntryTextType.ExtraText;
                }
                else
                {
                    info.Name = "ActionButtonText";
                    info.EntryCategory = FmgEntryCategory.ActionButtonText;
                }

                break;
            case FmgIDType.ReusedFMG_35:
                if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "TitleGenerator";
                    info.UICategory = FmgUICategory.Item;
                    info.EntryCategory = FmgEntryCategory.Generator;
                    info.EntryType = FmgEntryTextType.Title;
                }
                else
                {
                    info.Name = "TitleGem";
                    info.UICategory = FmgUICategory.Item;
                    info.EntryCategory = FmgEntryCategory.Gem;
                    info.EntryType = FmgEntryTextType.Title;
                }

                break;
            case FmgIDType.ReusedFMG_36:
                if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "DescriptionGenerator";
                    info.UICategory = FmgUICategory.Item;
                    info.EntryCategory = FmgEntryCategory.Generator;
                    info.EntryType = FmgEntryTextType.Description;
                }
                else
                {
                    info.Name = "SummaryGem";
                    info.UICategory = FmgUICategory.Item;
                    info.EntryCategory = FmgEntryCategory.Gem;
                    info.EntryType = FmgEntryTextType.Summary;
                }

                break;
            case FmgIDType.ReusedFMG_41:
                if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "TitleFCS";
                    info.UICategory = FmgUICategory.Item;
                    info.EntryCategory = FmgEntryCategory.FCS;
                    info.EntryType = FmgEntryTextType.Title;
                }
                else
                {
                    info.Name = "TitleMessage";
                    info.UICategory = FmgUICategory.Menu;
                    info.EntryCategory = FmgEntryCategory.Message;
                    info.EntryType = FmgEntryTextType.TextBody;
                }

                break;
            case FmgIDType.ReusedFMG_42:
                if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "DescriptionFCS";
                    info.UICategory = FmgUICategory.Item;
                    info.EntryCategory = FmgEntryCategory.FCS;
                    info.EntryType = FmgEntryTextType.Description;
                }
                else
                {
                    info.Name = "TitleSwordArts";
                    info.UICategory = FmgUICategory.Item;
                    info.EntryCategory = FmgEntryCategory.SwordArts;
                    info.EntryType = FmgEntryTextType.Title;
                }

                break;
            case FmgIDType.Event:
            case FmgIDType.Event_Patch:
                if (gameType is GameType.DemonsSouls or GameType.DarkSoulsPTDE or GameType.DarkSoulsRemastered
                    or GameType.Bloodborne)
                {
                    info.EntryCategory = FmgEntryCategory.ActionButtonText;
                }

                break;
            case FmgIDType.ReusedFMG_205:
                if (gameType == GameType.EldenRing)
                {
                    info.Name = "LoadingTitle";
                    info.EntryType = FmgEntryTextType.Title;
                    info.EntryCategory = FmgEntryCategory.LoadingScreen;
                }
                else if (gameType == GameType.Sekiro)
                {
                    info.Name = "LoadingText";
                    info.EntryType = FmgEntryTextType.Description;
                    info.EntryCategory = FmgEntryCategory.LoadingScreen;
                }
                else if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "MenuContext";
                }
                else
                {
                    info.Name = "SystemMessage_PS4";
                }

                break;
            case FmgIDType.ReusedFMG_206:
                if (gameType == GameType.EldenRing)
                {
                    info.Name = "LoadingText";
                    info.EntryType = FmgEntryTextType.Description;
                    info.EntryCategory = FmgEntryCategory.LoadingScreen;
                }
                else if (gameType == GameType.Sekiro)
                {
                    info.Name = "LoadingTitle";
                    info.EntryType = FmgEntryTextType.Title;
                    info.EntryCategory = FmgEntryCategory.LoadingScreen;
                }
                else
                {
                    info.Name = "SystemMessage_XboxOne";
                }

                break;
            case FmgIDType.ReusedFMG_210:
                if (gameType == GameType.EldenRing)
                {
                    info.Name = "ToS_win64";
                    info.UICategory = FmgUICategory.Menu;
                    info.EntryType = FmgEntryTextType.TextBody;
                    info.EntryCategory = FmgEntryCategory.None;
                }
                else if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "TextEmbeddedImageNames";
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
                    FMGInfo parent = FmgInfoBank.Find(e => e.FmgID == FmgIDType.TitleGoods);
                    info.AddParent(parent);
                }

                break;
        }
    }

    /// <summary>
    ///     Get patched FMG Entries for the specified category, with TextType Title or TextBody.
    /// </summary>
    /// <param name="category">FMGEntryCategory. If "None", an empty list will be returned.</param>
    /// <returns>List of patched entries if found; empty list otherwise.</returns>
    public List<FMG.Entry> GetFmgEntriesByCategory(FmgEntryCategory category, bool sort = true)
    {
        if (category == FmgEntryCategory.None)
        {
            return new List<FMG.Entry>();
        }

        foreach (FMGInfo info in FmgInfoBank)
        {
            if (info.EntryCategory == category &&
                info.EntryType is FmgEntryTextType.Title or FmgEntryTextType.TextBody)
            {
                return info.GetPatchedEntries(sort);
            }
        }

        return new List<FMG.Entry>();
    }

    /// <summary>
    ///     Get patched FMG Entries for the specified category and text type.
    /// </summary>
    /// <param name="category">FMGEntryCategory . If "None", an empty list will be returned.</param>
    /// <returns>List of patched entries if found; empty list otherwise.</returns>
    public List<FMG.Entry> GetFmgEntriesByCategoryAndTextType(FmgEntryCategory category,
        FmgEntryTextType textType, bool sort = true)
    {
        if (category == FmgEntryCategory.None)
        {
            return new List<FMG.Entry>();
        }

        foreach (FMGInfo info in FmgInfoBank)
        {
            if (info.EntryCategory == category && info.EntryType == textType)
            {
                return info.GetPatchedEntries(sort);
            }
        }

        return new List<FMG.Entry>();
    }

    /// <summary>
    ///     Get patched FMG Entries for the specified FmgIDType.
    /// </summary>
    /// <returns>List of patched entries if found; empty list otherwise.</returns>
    public List<FMG.Entry> GetFmgEntriesByFmgIDType(FmgIDType fmgID, bool sort = true)
    {
        foreach (FMGInfo info in FmgInfoBank)
        {
            if (info.FmgID == fmgID)
            {
                return info.GetPatchedEntries(sort);
            }
        }

        return new List<FMG.Entry>();
    }

    /// <summary>
    ///     Generate a new EntryGroup using a given ID and FMGInfo.
    ///     Data is updated using FMGInfo PatchChildren.
    /// </summary>
    public EntryGroup GenerateEntryGroup(int id, FMGInfo fmgInfo)
    {
        EntryGroup eGroup = new() { ID = id };

        if (fmgInfo.EntryCategory == FmgEntryCategory.None || CFG.Current.FMG_NoGroupedFmgEntries)
        {
            List<EntryFMGInfoPair> entryPairs = fmgInfo.GetPatchedEntryFMGPairs();
            EntryFMGInfoPair pair = entryPairs.Find(e => e.Entry.ID == id);
            if (pair == null)
            {
                return eGroup;
            }

            eGroup.TextBody = pair.Entry;
            eGroup.TextBodyInfo = pair.FmgInfo;
            return eGroup;
        }

        foreach (FMGInfo info in FmgInfoBank)
        {
            if (info.EntryCategory == fmgInfo.EntryCategory && info.PatchParent == null)
            {
                List<EntryFMGInfoPair> entryPairs = info.GetPatchedEntryFMGPairs();
                EntryFMGInfoPair pair = entryPairs.Find(e => e.Entry.ID == id);
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
                        case FmgEntryTextType.ExtraText:
                            eGroup.ExtraText = pair.Entry;
                            eGroup.ExtraTextInfo = pair.FmgInfo;
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

    private void HandleDuplicateEntries()
    {
        var askedAboutDupes = false;
        var ignoreDupes = true;
        foreach (FMGInfo info in FmgInfoBank)
        {
            IEnumerable<FMG.Entry> dupes = info.Fmg.Entries.GroupBy(e => e.ID).SelectMany(g => g.SkipLast(1));
            if (dupes.Any())
            {
                var dupeList = string.Join(", ", dupes.Select(dupe => dupe.ID));
                if (!askedAboutDupes && PlatformUtils.Instance.MessageBox(
                        $"Duplicate text entries have been found in FMG {Path.GetFileNameWithoutExtension(info.FileName)} for the following row IDs:\n\n{dupeList}\n\nRemove all duplicates? (Latest entries are kept)",
                        "Duplicate Text Entries", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ignoreDupes = false;
                }

                askedAboutDupes = true;

                if (!ignoreDupes)
                {
                    foreach (FMG.Entry dupe in dupes)
                    {
                        info.Fmg.Entries.Remove(dupe);
                    }
                }
            }
        }
    }

    private static string FormatJson(string json)
    {
        json = json.Replace("{\"ID\"", "\r\n{\"ID\"");
        json = json.Replace("],", "\r\n],");
        return json;
    }

    private void SaveFMGsDS2()
    {
        foreach (FMGInfo info in FmgInfoBank)
        {
            Utils.WriteWithBackup(Project.ParentProject.AssetLocator.RootDirectory, Project.AssetLocator.RootDirectory,
                $@"menu\text\{LanguageFolder}\{info.Name}.fmg", info.Fmg);
        }
    }

    public void SaveFMGs()
    {
        try
        {
            if (!IsLoaded)
            {
                return;
            }

            if (Project.AssetLocator.Type == GameType.Undefined)
            {
                return;
            }

            if (Project.AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                SaveFMGsDS2();
                TaskLogs.AddLog("Saved FMG text");
                return;
            }

            // Load the fmg bnd, replace fmgs, and save
            IBinder fmgBinderItem;
            IBinder fmgBinderMenu;
            AssetDescription itemMsgPath = Project.AssetLocator.GetItemMsgbnd(LanguageFolder);
            AssetDescription menuMsgPath = Project.AssetLocator.GetMenuMsgbnd(LanguageFolder);
            if (Project.AssetLocator.Type == GameType.DemonsSouls || Project.AssetLocator.Type == GameType.DarkSoulsPTDE ||
                Project.AssetLocator.Type == GameType.DarkSoulsRemastered)
            {
                fmgBinderItem = BND3.Read(itemMsgPath.AssetPath);
                fmgBinderMenu = BND3.Read(menuMsgPath.AssetPath);
            }
            else
            {
                fmgBinderItem = BND4.Read(itemMsgPath.AssetPath);
                fmgBinderMenu = BND4.Read(menuMsgPath.AssetPath);
            }

            foreach (BinderFile file in fmgBinderItem.Files)
            {
                FMGInfo info = FmgInfoBank.Find(e => e.FmgID == (FmgIDType)file.ID);
                if (info != null)
                {
                    file.Bytes = info.Fmg.Write();
                }
            }

            foreach (BinderFile file in fmgBinderMenu.Files)
            {
                FMGInfo info = FmgInfoBank.Find(e => e.FmgID == (FmgIDType)file.ID);
                if (info != null)
                {
                    file.Bytes = info.Fmg.Write();
                }
            }

            AssetDescription itemMsgPathDest = Project.AssetLocator.GetItemMsgbnd(LanguageFolder, true);
            AssetDescription menuMsgPathDest = Project.AssetLocator.GetMenuMsgbnd(LanguageFolder, true);
            if (fmgBinderItem is BND3 bnd3)
            {
                Utils.WriteWithBackup(Project.ParentProject.AssetLocator.RootDirectory,
                    Project.AssetLocator.RootDirectory, itemMsgPathDest.AssetPath, bnd3);
                Utils.WriteWithBackup(Project.ParentProject.AssetLocator.RootDirectory,
                    Project.AssetLocator.RootDirectory, menuMsgPathDest.AssetPath, (BND3)fmgBinderMenu);
                if (Project.AssetLocator.Type is GameType.DemonsSouls)
                {
                    bnd3.Compression = DCX.Type.None;
                    ((BND3)fmgBinderMenu).Compression = DCX.Type.None;
                    Utils.WriteWithBackup(Project.ParentProject.AssetLocator.RootDirectory,
                    Project.AssetLocator.RootDirectory, itemMsgPathDest.AssetPath[..^4], bnd3);
                    Utils.WriteWithBackup(Project.ParentProject.AssetLocator.RootDirectory,
                    Project.AssetLocator.RootDirectory, menuMsgPathDest.AssetPath[..^4], (BND3)fmgBinderMenu);
                }
            }
            else if (fmgBinderItem is BND4 bnd4)
            {
                Utils.WriteWithBackup(Project.ParentProject.AssetLocator.RootDirectory,
                    Project.AssetLocator.RootDirectory, itemMsgPathDest.AssetPath, bnd4);
                Utils.WriteWithBackup(Project.ParentProject.AssetLocator.RootDirectory,
                    Project.AssetLocator.RootDirectory, menuMsgPathDest.AssetPath, (BND4)fmgBinderMenu);
            }

            fmgBinderItem.Dispose();
            fmgBinderMenu.Dispose();
            TaskLogs.AddLog("Saved FMG text");
        }
        catch (SavingFailedException e)
        {
            TaskLogs.AddLog(e.Wrapped.Message,
                LogLevel.Error, TaskLogs.LogPriority.High, e.Wrapped);
        }
    }

    /// <summary>
    ///     Value pair with an entry and the FMG it belongs to.
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
    ///     Base object that stores an FMG and information regarding it.
    /// </summary>
    public class FMGInfo
    {
        public FMGBank Owner;

        public FmgEntryCategory EntryCategory;
        public FmgEntryTextType EntryType;
        public string FileName;
        public FMG Fmg;
        public FmgIDType FmgID;
        public string Name;

        /// <summary>
        ///     List of associated children to this FMGInfo used to get patch entry data.
        /// </summary>
        public List<FMGInfo> PatchChildren = new();

        public FMGInfo PatchParent;
        public FmgUICategory UICategory;

        private string _patchPrefix = null;
        public string PatchPrefix
        {
            get
            {
                _patchPrefix ??= Name.Replace(RemovePatchStrings(Name), "");
                return _patchPrefix;
            }
        }

        public void AddParent(FMGInfo parent)
        {
            if (CFG.Current.FMG_NoFmgPatching)
            {
                return;
            }

            PatchParent = parent;
            parent.PatchChildren.Add(this);
        }

        /// <summary>
        ///     Returns a patched list of Entry & FMGInfo value pairs from this FMGInfo and its children.
        ///     If a PatchParent exists, it will be checked instead.
        /// </summary>
        public List<EntryFMGInfoPair> GetPatchedEntryFMGPairs(bool sort = true)
        {
            if (PatchParent != null)
            {
                return PatchParent.GetPatchedEntryFMGPairs(sort);
            }

            List<EntryFMGInfoPair> list = new();
            foreach (FMG.Entry entry in Fmg.Entries)
            {
                list.Add(new EntryFMGInfoPair(this, entry));
            }

            // Check and apply patch entries
            foreach (FMGInfo child in PatchChildren.OrderBy(e => (int)e.FmgID))
            {
                foreach (FMG.Entry entry in child.Fmg.Entries)
                {
                    EntryFMGInfoPair match = list.Find(e => e.Entry.ID == entry.ID);
                    if (match != null)
                    {
                        // This is a patch entry
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
            {
                list = list.OrderBy(e => e.Entry.ID).ToList();
            }

            return list;
        }

        /// <summary>
        ///     Returns a patched list of entries in this FMGInfo and its children.
        ///     If a PatchParent exists, it will be checked instead.
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
            foreach (FMGInfo child in PatchChildren.OrderBy(e => (int)e.FmgID))
            {
                foreach (FMG.Entry entry in child.Fmg.Entries)
                {
                    FMG.Entry match = list.Find(e => e.ID == entry.ID);
                    if (match != null)
                    {
                        // This is a patch entry
                        if (entry.Text != null)
                        {
                            // Text is not null, so it will overwrite non-patch entries.
                            list.Remove(match);
                            list.Add(entry);
                        }
                    }
                    else
                    {
                        list.Add(entry);
                    }
                }
            }

            if (sort)
            {
                list = list.OrderBy(e => e.ID).ToList();
            }

            return list;
        }

        /// <summary>
        ///     Returns title FMGInfo that shares this FMGInfo's EntryCategory.
        ///     If none are found, an exception will be thrown.
        /// </summary>
        public FMGInfo GetTitleFmgInfo()
        {
            foreach (var info in Owner.FmgInfoBank)
            {
                if (info.EntryCategory == EntryCategory && info.EntryType == FmgEntryTextType.Title && info.PatchPrefix == PatchPrefix)
                {
                    return info;
                }
            }
            throw new InvalidOperationException($"Couldn't find title FMGInfo for {this.Name}");
        }

        /// <summary>
        ///     Adds an entry to the end of the FMG.
        /// </summary>
        public void AddEntry(FMG.Entry entry)
        {
            Fmg.Entries.Add(entry);
        }

        /// <summary>
        ///     Clones an FMG entry.
        /// </summary>
        /// <returns>Cloned entry</returns>
        public FMG.Entry CloneEntry(FMG.Entry entry)
        {
            FMG.Entry newEntry = new(entry.ID, entry.Text);
            return newEntry;
        }

        /// <summary>
        ///     Removes an entry from FMGInfo's FMG.
        /// </summary>
        public void DeleteEntry(FMG.Entry entry)
        {
            Fmg.Entries.Remove(entry);
        }
    }


    /// <summary>
    ///     A group of entries that may be associated (such as title, summary, description) along with respective FMGs.
    /// </summary>
    public class EntryGroup
    {
        private int _ID = -1;
        public FMG.Entry Description;
        public FMGInfo DescriptionInfo;
        public FMG.Entry ExtraText;
        public FMGInfo ExtraTextInfo;
        public FMG.Entry Summary;
        public FMGInfo SummaryInfo;
        public FMG.Entry TextBody;
        public FMGInfo TextBodyInfo;
        public FMG.Entry Title;
        public FMGInfo TitleInfo;

        public int ID
        {
            set
            {
                _ID = value;
                if (TextBody != null)
                {
                    TextBody.ID = _ID;
                }

                if (Title != null)
                {
                    Title.ID = _ID;
                }

                if (Summary != null)
                {
                    Summary.ID = _ID;
                }

                if (Description != null)
                {
                    Description.ID = _ID;
                }

                if (ExtraText != null)
                {
                    ExtraText.ID = _ID;
                }
            }
            get => _ID;
        }

        /// <summary>
        ///     Gets next unused entry ID.
        /// </summary>
        public int GetNextUnusedID()
        {
            var id = ID;
            if (TextBody != null)
            {
                List<FMG.Entry> entries = TextBodyInfo.GetPatchedEntries();
                do
                {
                    id++;
                } while (entries.Find(e => e.ID == id) != null);
            }
            else if (Title != null)
            {
                List<FMG.Entry> entries = TitleInfo.GetPatchedEntries();
                do
                {
                    id++;
                } while (entries.Find(e => e.ID == id) != null);
            }
            else if (Summary != null)
            {
                List<FMG.Entry> entries = SummaryInfo.GetPatchedEntries();
                do
                {
                    id++;
                } while (entries.Find(e => e.ID == id) != null);
            }
            else if (Description != null)
            {
                List<FMG.Entry> entries = DescriptionInfo.GetPatchedEntries();
                do
                {
                    id++;
                } while (entries.Find(e => e.ID == id) != null);
            }
            else if (ExtraText != null)
            {
                List<FMG.Entry> entries = ExtraTextInfo.GetPatchedEntries();
                do
                {
                    id++;
                } while (entries.Find(e => e.ID == id) != null);
            }

            return id;
        }

        /// <summary>
        ///     Sets ID of all entries to the next unused entry ID.
        /// </summary>
        public void SetNextUnusedID()
        {
            ID = GetNextUnusedID();
        }

        /// <summary>
        ///     Places all entries within this EntryGroup into their assigned FMGs.
        /// </summary>
        public void ImplementEntryGroup()
        {
            if (TextBody != null)
            {
                TextBodyInfo.AddEntry(TextBody);
            }

            if (Title != null)
            {
                TitleInfo.AddEntry(Title);
            }

            if (Summary != null)
            {
                SummaryInfo.AddEntry(Summary);
            }

            if (Description != null)
            {
                DescriptionInfo.AddEntry(Description);
            }

            if (ExtraText != null)
            {
                ExtraTextInfo.AddEntry(ExtraText);
            }
        }

        /// <summary>
        ///     Duplicates all entries within their assigned FMGs.
        ///     New entries are inserted into their assigned FMGs.
        /// </summary>
        /// <returns>New EntryGroup.</returns>
        public EntryGroup DuplicateFMGEntries()
        {
            EntryGroup newGroup = new();
            if (TextBody != null)
            {
                newGroup.TextBodyInfo = TextBodyInfo;
                newGroup.TextBody = TextBodyInfo.CloneEntry(TextBody);
                TextBodyInfo.AddEntry(newGroup.TextBody);
            }

            if (Title != null)
            {
                newGroup.TitleInfo = TitleInfo;
                newGroup.Title = TitleInfo.CloneEntry(Title);
                TitleInfo.AddEntry(newGroup.Title);
            }

            if (Summary != null)
            {
                newGroup.SummaryInfo = SummaryInfo;
                newGroup.Summary = SummaryInfo.CloneEntry(Summary);
                SummaryInfo.AddEntry(newGroup.Summary);
            }

            if (Description != null)
            {
                newGroup.DescriptionInfo = DescriptionInfo;
                newGroup.Description = DescriptionInfo.CloneEntry(Description);
                DescriptionInfo.AddEntry(newGroup.Description);
            }

            if (ExtraText != null)
            {
                newGroup.ExtraTextInfo = ExtraTextInfo;
                newGroup.ExtraText = ExtraTextInfo.CloneEntry(ExtraText);
                ExtraTextInfo.AddEntry(newGroup.ExtraText);
            }

            newGroup.ID = ID;
            return newGroup;
        }

        /// <summary>
        ///     Clones this EntryGroup and returns a duplicate.
        /// </summary>
        /// <returns>Cloned EntryGroup.</returns>
        public EntryGroup CloneEntryGroup()
        {
            EntryGroup newGroup = new();
            if (TextBody != null)
            {
                newGroup.TextBodyInfo = TextBodyInfo;
                newGroup.TextBody = TextBodyInfo.CloneEntry(TextBody);
            }

            if (Title != null)
            {
                newGroup.TitleInfo = TitleInfo;
                newGroup.Title = TitleInfo.CloneEntry(Title);
            }

            if (Summary != null)
            {
                newGroup.SummaryInfo = SummaryInfo;
                newGroup.Summary = SummaryInfo.CloneEntry(Summary);
            }

            if (Description != null)
            {
                newGroup.DescriptionInfo = DescriptionInfo;
                newGroup.Description = DescriptionInfo.CloneEntry(Description);
            }

            if (ExtraText != null)
            {
                newGroup.ExtraTextInfo = ExtraTextInfo;
                newGroup.ExtraText = ExtraTextInfo.CloneEntry(ExtraText);
            }

            return newGroup;
        }

        /// <summary>
        ///     Removes all entries from their assigned FMGs.
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

            if (ExtraText != null)
            {
                ExtraTextInfo.DeleteEntry(ExtraText);
            }
        }
    }
}
