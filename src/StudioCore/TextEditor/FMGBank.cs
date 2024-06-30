using Microsoft.Extensions.Logging;
using SoapstoneLib.Proto.Internal;
using SoulsFormats;
using StudioCore.Editor;
using StudioCore.MsbEditor;
using StudioCore.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StudioCore.TextEditor;

/*
 * FMGFileSet represents a grouped set of FMGInfos source from the same bnd or loose folder, within a single language.
 */
public class FMGFileSet
{
    internal FMGFileSet(FMGLanguage owner, FmgFileCategory category)
    {
        Lang = owner;
        FileCategory = category;
    }
    internal FMGLanguage Lang;
    internal FmgFileCategory FileCategory;
    internal bool IsLoaded = false;
    internal bool IsLoading = false;

    internal List<FMGInfo> FmgInfos = new();

    internal void InsertFmgInfo(FMGInfo info)
    {
        FmgInfos.Add(info);
    }

    /// <summary>
    ///     Loads MsgBnd from path, generates FMGInfo, and fills FmgInfoBank.
    /// </summary>
    /// <returns>True if successful; false otherwise.</returns>
    internal bool LoadMsgBnd(string path, string msgBndType = "UNDEFINED")
    {
        if (path == null)
        {
            if (Lang.LanguageFolder != "")
            {
                TaskLogs.AddLog(
                    $"Could locate text data files when looking for \"{msgBndType}\" in \"{Lang.LanguageFolder}\" folder",
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
        GameType Type = Lang.Owner.Project.AssetLocator.Type;
        if (Type == GameType.DemonsSouls || Type == GameType.DarkSoulsPTDE ||
            Type == GameType.DarkSoulsRemastered)
        {
            fmgBinder = BND3.Read(path);
        }
        else
        {
            fmgBinder = BND4.Read(path);
        }

        foreach (BinderFile file in fmgBinder.Files)
        {
            FmgInfos.Add(GenerateFMGInfo(file));
        }
        // I hate this 2 parse system. Solve game differences by including game in the get enum functions. Maybe parentage is solvable by pre-sorting binderfiles but that does seem silly. FMG patching sucks. 
        foreach (FMGInfo info in FmgInfos)
        {
            /* TODO sorting without modifying data
            if (CFG.Current.FMG_NoGroupedFmgEntries)
            {
                info.EntryType = FmgEntryTextType.TextBody;
            }*/
            SetFMGInfoPatchParent(info);
        }

        fmgBinder.Dispose();
        
        HandleDuplicateEntries();
        IsLoaded = true;
        IsLoading = false;
        return true;
    }

    internal bool LoadLooseMsgsDS2(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            FMGInfo info = GenerateFMGInfoDS2(file);        
            InsertFmgInfo(info);
        }

        //TODO ordering
        //FmgInfoBank = FmgInfoBank.OrderBy(e => e.Name).ToList();
        HandleDuplicateEntries();
        IsLoaded = true;
        IsLoading = false;
        return true;
    }

    internal void HandleDuplicateEntries()
    {
        var askedAboutDupes = false;
        var ignoreDupes = true;
        foreach (FMGInfo info in FmgInfos)
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
    internal FMGInfo GenerateFMGInfo(BinderFile file)
    {
        FMG fmg = FMG.Read(file.Bytes);
        var name = Enum.GetName(typeof(FmgIDType), file.ID);
        FMGInfo info = new()
        {
            FileSet = this,
            FileName = file.Name.Split("\\").Last(),
            Name = name,
            FmgID = (FmgIDType)file.ID,
            Fmg = fmg,
            EntryType = FMGEnums.GetFmgTextType(file.ID),
            EntryCategory = FMGEnums.GetFmgCategory(file.ID)
        };
        info.FileCategory = FMGEnums.GetFMGUICategory(info.EntryCategory);

        ApplyGameDifferences(info);
        return info;
    }

    internal FMGInfo GenerateFMGInfoDS2(string file)
    {
        // TODO: DS2 FMG grouping & UI sorting (copy SetFMGInfo)
        FMG fmg = FMG.Read(file);
        var name = Path.GetFileNameWithoutExtension(file);
        FMGInfo info = new()
        {
            FileSet = this,
            FileName = file.Split("\\").Last(),
            Name = name,
            FmgID = FmgIDType.None,
            Fmg = fmg,
            EntryType = FmgEntryTextType.TextBody,
            EntryCategory = FmgEntryCategory.None,
            FileCategory = FmgFileCategory.Loose
        };

        return info;
    }
    private void SetFMGInfoPatchParent(FMGInfo info)
    {
        var strippedName = FMGBank.RemovePatchStrings(info.Name);
        if (strippedName != info.Name)
        {
            // This is a patch FMG, try to find parent FMG.
            foreach (FMGInfo parentInfo in FmgInfos)
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
        GameType gameType = Lang.Owner.Project.AssetLocator.Type;
        switch (info.FmgID)
        {
            case FmgIDType.ReusedFMG_32:
                if (gameType == GameType.Bloodborne)
                {
                    info.Name = "GemExtraInfo";
                    info.FileCategory = FmgFileCategory.Item;
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
                    info.FileCategory = FmgFileCategory.Item;
                    info.EntryCategory = FmgEntryCategory.Generator;
                    info.EntryType = FmgEntryTextType.Title;
                }
                else
                {
                    info.Name = "TitleGem";
                    info.FileCategory = FmgFileCategory.Item;
                    info.EntryCategory = FmgEntryCategory.Gem;
                    info.EntryType = FmgEntryTextType.Title;
                }

                break;
            case FmgIDType.ReusedFMG_36:
                if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "DescriptionGenerator";
                    info.FileCategory = FmgFileCategory.Item;
                    info.EntryCategory = FmgEntryCategory.Generator;
                    info.EntryType = FmgEntryTextType.Description;
                }
                else
                {
                    info.Name = "SummaryGem";
                    info.FileCategory = FmgFileCategory.Item;
                    info.EntryCategory = FmgEntryCategory.Gem;
                    info.EntryType = FmgEntryTextType.Summary;
                }

                break;
            case FmgIDType.ReusedFMG_41:
                if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "TitleFCS";
                    info.FileCategory = FmgFileCategory.Item;
                    info.EntryCategory = FmgEntryCategory.FCS;
                    info.EntryType = FmgEntryTextType.Title;
                }
                else
                {
                    info.Name = "TitleMessage";
                    info.FileCategory = FmgFileCategory.Menu;
                    info.EntryCategory = FmgEntryCategory.Message;
                    info.EntryType = FmgEntryTextType.TextBody;
                }

                break;
            case FmgIDType.ReusedFMG_42:
                if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "DescriptionFCS";
                    info.FileCategory = FmgFileCategory.Item;
                    info.EntryCategory = FmgEntryCategory.FCS;
                    info.EntryType = FmgEntryTextType.Description;
                }
                else
                {
                    info.Name = "TitleSwordArts";
                    info.FileCategory = FmgFileCategory.Item;
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
                    info.FileCategory = FmgFileCategory.Menu;
                    info.EntryType = FmgEntryTextType.TextBody;
                    info.EntryCategory = FmgEntryCategory.None;
                }
                else if (gameType == GameType.ArmoredCoreVI)
                {
                    info.Name = "TextEmbeddedImageNames";
                    info.FileCategory = FmgFileCategory.Menu;
                    info.EntryType = FmgEntryTextType.TextBody;
                    info.EntryCategory = FmgEntryCategory.None;
                }
                else
                {
                    info.Name = "TitleGoods_DLC1";
                    info.FileCategory = FmgFileCategory.Item;
                    info.EntryType = FmgEntryTextType.Title;
                    info.EntryCategory = FmgEntryCategory.Goods;
                    FMGInfo parent = FmgInfos.FirstOrDefault(e => e.FmgID == FmgIDType.TitleGoods);
                    info.AddParent(parent);
                }

                break;
        }
    }
}

/*
 * FMGLanguage represents a grouped set of FMGFileSets containing FMGInfos sourced from the same language, within a project's FMGBank.
 */
public class FMGLanguage
{
    internal FMGLanguage(FMGBank owner, string language)
    {
        Owner = owner;
        LanguageFolder = language;
    }
    internal readonly FMGBank Owner;
    internal readonly string LanguageFolder;
    internal bool IsLoaded => _FmgInfoBanks.Count != 0 && _FmgInfoBanks.All((fs) => fs.Value.IsLoaded);
    internal bool IsLoading => _FmgInfoBanks.Count != 0 && _FmgInfoBanks.Any((fs) => fs.Value.IsLoading);
    internal readonly Dictionary<FmgFileCategory, FMGFileSet> _FmgInfoBanks = new();

    /// <summary>
    ///     Loads item and menu MsgBnds from paths, generates FMGInfo, and fills FmgInfoBank.
    /// </summary>
    /// <returns>True if successful; false otherwise.</returns>
    internal bool LoadItemMenuMsgBnds(AssetDescription itemMsgPath, AssetDescription menuMsgPath)
    {
        FMGFileSet itemMsgBnd = new FMGFileSet(this, FmgFileCategory.Item);
        if (itemMsgBnd.LoadMsgBnd(itemMsgPath.AssetPath, "item.msgbnd"))
            _FmgInfoBanks.Add(itemMsgBnd.FileCategory, itemMsgBnd);
        FMGFileSet menuMsgBnd = new FMGFileSet(this, FmgFileCategory.Menu);
        if (menuMsgBnd.LoadMsgBnd(menuMsgPath.AssetPath, "menu.msgbnd"))
            _FmgInfoBanks.Add(menuMsgBnd.FileCategory, menuMsgBnd);
        if (_FmgInfoBanks.Count == 0)
            return false;
        return true;
    }
    
    internal bool LoadNormalFmgs()
    {
        AssetDescription itemMsgPath = Owner.Project.AssetLocator.GetItemMsgbnd(LanguageFolder);
        AssetDescription menuMsgPath = Owner.Project.AssetLocator.GetMenuMsgbnd(LanguageFolder);
        if (LoadItemMenuMsgBnds(itemMsgPath, menuMsgPath))
        {
            return true;
        }
        return false;

    }
    internal bool LoadDS2FMGs()
    {
        AssetDescription desc = Owner.Project.AssetLocator.GetItemMsgbnd(LanguageFolder, true);

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
            return false;
        }

        IEnumerable<string> files = Owner.Project.AssetLocator.GetAllAssets($@"{desc.AssetPath}", [@"*.fmg"]);
        
        FMGFileSet looseMsg = new FMGFileSet(this, FmgFileCategory.Loose);
        if (looseMsg.LoadLooseMsgsDS2(files))
        {
            _FmgInfoBanks.Add(looseMsg.FileCategory, looseMsg);
            return true;
        }
        return false;
    }
    
    public void SaveFMGs()
    {
        try
        {
            if (!IsLoaded)
            {
                return;
            }

            if (Owner.Project.AssetLocator.Type == GameType.Undefined)
            {
                return;
            }

            if (Owner.Project.AssetLocator.Type == GameType.DarkSoulsIISOTFS)
            {
                SaveFMGsDS2();
            }
            else
            {
                SaveFMGsNormal();
            }
            TaskLogs.AddLog("Saved FMG text");
        }
        catch (SavingFailedException e)
        {
            TaskLogs.AddLog(e.Wrapped.Message,
                LogLevel.Error, TaskLogs.LogPriority.High, e.Wrapped);
        }
    }

    private void SaveFMGsDS2()
    {
        foreach (FMGInfo info in _FmgInfoBanks.SelectMany((x) => x.Value.FmgInfos))
        {
            Utils.WriteWithBackup(Owner.Project.ParentProject.AssetLocator.RootDirectory, Owner.Project.AssetLocator.RootDirectory,
                $@"menu\text\{LanguageFolder}\{info.Name}.fmg", info.Fmg);
        }
    }
    private void SaveFMGsNormal()
    {
        // Load the fmg bnd, replace fmgs, and save
        IBinder fmgBinderItem;
        IBinder fmgBinderMenu;
        AssetDescription itemMsgPath = Owner.Project.AssetLocator.GetItemMsgbnd(LanguageFolder);
        AssetDescription menuMsgPath = Owner.Project.AssetLocator.GetMenuMsgbnd(LanguageFolder);
        if (Owner.Project.AssetLocator.Type == GameType.DemonsSouls || Owner.Project.AssetLocator.Type == GameType.DarkSoulsPTDE ||
            Owner.Project.AssetLocator.Type == GameType.DarkSoulsRemastered)
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
            FMGInfo info = _FmgInfoBanks.SelectMany((x) => x.Value.FmgInfos).FirstOrDefault(e => e.FmgID == (FmgIDType)file.ID);
            if (info != null)
            {
                file.Bytes = info.Fmg.Write();
            }
        }

        foreach (BinderFile file in fmgBinderMenu.Files)
        {
            FMGInfo info = _FmgInfoBanks.SelectMany((x) => x.Value.FmgInfos).FirstOrDefault(e => e.FmgID == (FmgIDType)file.ID);
            if (info != null)
            {
                file.Bytes = info.Fmg.Write();
            }
        }

        AssetDescription itemMsgPathDest = Owner.Project.AssetLocator.GetItemMsgbnd(LanguageFolder, true);
        AssetDescription menuMsgPathDest = Owner.Project.AssetLocator.GetMenuMsgbnd(LanguageFolder, true);
        var parentDir = Owner.Project.ParentProject.AssetLocator.RootDirectory;
        var modDir = Owner.Project.AssetLocator.RootDirectory;
        if (fmgBinderItem is BND3 bnd3)
        {
            Utils.WriteWithBackup(parentDir, modDir, itemMsgPathDest.AssetPath, bnd3);
            Utils.WriteWithBackup(parentDir, modDir, menuMsgPathDest.AssetPath, (BND3)fmgBinderMenu);
            if (Owner.Project.AssetLocator.Type is GameType.DemonsSouls)
            {
                bnd3.Compression = DCX.Type.None;
                ((BND3)fmgBinderMenu).Compression = DCX.Type.None;
                Utils.WriteWithBackup(parentDir, modDir, itemMsgPathDest.AssetPath[..^4], bnd3);
                Utils.WriteWithBackup(parentDir, modDir, menuMsgPathDest.AssetPath[..^4], (BND3)fmgBinderMenu);
            }
        }
        else if (fmgBinderItem is BND4 bnd4)
        {
            Utils.WriteWithBackup(parentDir, modDir, itemMsgPathDest.AssetPath, bnd4);
            Utils.WriteWithBackup(parentDir, modDir, menuMsgPathDest.AssetPath, (BND4)fmgBinderMenu);
        }

        fmgBinderItem.Dispose();
        fmgBinderMenu.Dispose();
    }
}

/// <summary>
///     Class that stores all the strings for a Souls project.
/// </summary>
public class FMGBank
{
    public Project Project;

    public FMGBank(Project project)
    {
        Project = project;
    }

    
    public bool IsLoaded => fmgLangs.Count != 0 && fmgLangs.All((fs) => fs.Value.IsLoaded);
    public bool IsLoading => fmgLangs.Count != 0 && fmgLangs.Any((fs) => fs.Value.IsLoading);
    public string LanguageFolder { get; private set; } = "";

    public IEnumerable<FMGInfo> FmgInfoBank { get => fmgLangs[LanguageFolder]._FmgInfoBanks.SelectMany((x) => x.Value.FmgInfos); }
    public IEnumerable<FMGInfo> SortedFmgInfoBank {
        get {
            //This check shouldn't be here. Should do better housekeeping.
            if (IsLoading || !IsLoaded || !fmgLangs.ContainsKey(LanguageFolder))
            {
                return [];
            }
            if (CFG.Current.FMG_NoGroupedFmgEntries)
            {
                return FmgInfoBank.OrderBy(e => e.EntryCategory).ThenBy(e => e.FmgID);
            }
            else
            {
                return FmgInfoBank.OrderBy(e => e.Name);
            }
        }
    }
    public Dictionary<string, FMGLanguage> fmgLangs = new();
    public IEnumerable<FmgFileCategory> currentFmgInfoBanks {
        get {
            if (IsLoading || !IsLoaded || !fmgLangs.ContainsKey(LanguageFolder))
            {
                return [];
            }
            return fmgLangs[LanguageFolder]._FmgInfoBanks.Keys;
        }
    }

    
    /// <summary>
    ///     List of strings to compare with "FmgIDType" name to identify patch FMGs.
    /// </summary>
    private static readonly List<string> patchStrings = new() { "_Patch", "_DLC1", "_DLC2" };
    /// <summary>
    ///     Removes patch/DLC identifiers from strings for the purposes of finding patch FMGs. Kinda dumb.
    /// </summary>
    internal static string RemovePatchStrings(string str)
    {
        foreach (var badString in patchStrings)
        {
            str = str.Replace(badString, "", StringComparison.CurrentCultureIgnoreCase);
        }

        return str;
    }
    public static void ReloadFMGs()
    {
        Locator.ActiveProject.FMGBank.LoadFMGs();
    }
    public void LoadFMGs(string languageFolder = "")
    {
        TaskManager.Run(new TaskManager.LiveTask("FMG - Load Text - " + languageFolder, TaskManager.RequeueType.WaitThenRequeue, true,
            () =>
            {
                LanguageFolder = languageFolder;
                SetDefaultLanguagePath();
                if (fmgLangs.ContainsKey(LanguageFolder))
                {
                    return;
                }

                if (Project.AssetLocator.Type == GameType.Undefined)
                {
                    return;
                }

                FMGLanguage lang = new FMGLanguage(this, LanguageFolder);
                bool success = false;
                if (Project.AssetLocator.Type == GameType.DarkSoulsIISOTFS)
                {
                    success = lang.LoadDS2FMGs();
                }
                else
                {
                    success = lang.LoadNormalFmgs();
                }
                if (success)
                    fmgLangs.Add(lang.LanguageFolder, lang);
            }));
    }
    
    public void SaveFMGs()
    {
        foreach (FMGLanguage lang in fmgLangs.Values)
        {
            lang.SaveFMGs();
        }
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
    public FMGEntryGroup GenerateEntryGroup(int id, FMGInfo fmgInfo)
    {
        FMGEntryGroup eGroup = new() { ID = id };

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
    public FMGFileSet FileSet;

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
    public FmgFileCategory FileCategory;

    private string _patchPrefix = null;
    public string PatchPrefix
    {
        get
        {
            _patchPrefix ??= Name.Replace(FMGBank.RemovePatchStrings(Name), "");
            return _patchPrefix;
        }
    }

    public void AddParent(FMGInfo parent)
    {
        PatchParent = parent;
        parent.PatchChildren.Add(this);
    }

    /// <summary>
    ///     Returns a patched list of Entry & FMGInfo value pairs from this FMGInfo and its children.
    ///     If a PatchParent exists, it will be checked instead.
    /// </summary>
    public List<EntryFMGInfoPair> GetPatchedEntryFMGPairs(bool sort = true)
    {
        if (PatchParent != null && !CFG.Current.FMG_NoFmgPatching)
        {
            return PatchParent.GetPatchedEntryFMGPairs(sort);
        }

        List<EntryFMGInfoPair> list = new();
        foreach (FMG.Entry entry in Fmg.Entries)
        {
            list.Add(new EntryFMGInfoPair(this, entry));
        }

        if (!CFG.Current.FMG_NoFmgPatching)
        {
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
        if (PatchParent != null && !CFG.Current.FMG_NoFmgPatching)
        {
            return PatchParent.GetPatchedEntries(sort);
        }

        List<FMG.Entry> list = new();
        list.AddRange(Fmg.Entries);

        if (!CFG.Current.FMG_NoFmgPatching)
        {
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
        foreach (var info in FileSet.FmgInfos)
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
public class FMGEntryGroup
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
    public FMGEntryGroup DuplicateFMGEntries()
    {
        FMGEntryGroup newGroup = new();
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
    public FMGEntryGroup CloneEntryGroup()
    {
        FMGEntryGroup newGroup = new();
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
