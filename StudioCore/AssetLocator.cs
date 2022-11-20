using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using SoulsFormats;

namespace StudioCore
{
    /// <summary>
    /// Generic asset description for a generic game asset
    /// </summary>
    public class AssetDescription
    {
        /// <summary>
        /// Pretty UI friendly name for an asset. Usually the file name without an extention i.e. c1234
        /// </summary>
        public string AssetName = null;

        /// <summary>
        /// Absolute path of where the full asset is located. If this asset exists in a mod override directory,
        /// then this path points to that instead of the base game asset.
        /// </summary>
        public string AssetPath = null;

        public string AssetArchiveVirtualPath = null;

        /// <summary>
        /// Virtual friendly path for this asset to use with the resource manager
        /// </summary>
        public string AssetVirtualPath = null;

        /// <summary>
        /// Where applicable, the numeric asset ID. Usually applies to chrs, objs, and various map pieces
        /// </summary>
        public int AssetID;

        public override int GetHashCode()
        {
            if (AssetVirtualPath != null)
            {
                return AssetVirtualPath.GetHashCode();
            }
            else if (AssetPath != null)
            {
                return AssetPath.GetHashCode();
            }
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is AssetDescription ad)
            {
                if (AssetVirtualPath != null)
                {
                    return AssetVirtualPath.Equals(ad.AssetVirtualPath);
                }
                if (AssetPath != null)
                {
                    return AssetPath.Equals(ad.AssetPath);
                }
            }
            return base.Equals(obj);
        }
    }

    /// <summary>
    /// Exposes an interface to retrieve game assets from the various souls games. Also allows layering
    /// of an additional mod directory on top of the game assets.
    /// </summary>
    public class AssetLocator
    {

        public static readonly string GameExecutatbleFilter =
            "Windows executable (*.EXE) |*.EXE*|" +
            "Playstation executable (*.BIN) |*.BIN*|" +
            "All Files|*.*";

        public static readonly string JsonFilter =
            "Project file (project.json) |PROJECT.JSON";

        public static readonly string ERRegulationFilter =
            "Regulation file (regulation.bin) |REGULATION.BIN";
        
        public static readonly string ParamFilter =
            ERRegulationFilter + "|" +
            "Data file (Data0.bdt) |DATA0.BDT|" +
            "ParamBndDcx (gameparam.parambnd.dcx) |GAMEPARAM.PARAMBND.DCX|" +
            "ParamBnd (gameparam.parambnd) |GAMEPARAM.PARAMBND";

        public static readonly string ERParamUpgradeFilter =
            ERRegulationFilter + "|" +
            "All Files|*.*";
            
        
        public GameType Type { get; private set; } = GameType.Undefined;

        /// <summary>
        /// The game interroot where all the game assets are
        /// </summary>
        public string GameRootDirectory { get; private set; } = null;

        /// <summary>
        /// An optional override mod directory where modded files are stored
        /// </summary>
        public string GameModDirectory { get; private set; } = null;

        public AssetLocator()
        {
        }

        private List<string> FullMapList = null;

        public string GetAssetPath(string relpath)
        {
            if (GameModDirectory != null)
            {
                var modpath = $@"{GameModDirectory}\{relpath}";
                if (File.Exists(modpath))
                {
                    return modpath;
                }
            }
            return $@"{GameRootDirectory}\{relpath}";
        }

        public GameType GetGameTypeForExePath(string exePath)
        {
            GameType type = GameType.Undefined;
            if (exePath.ToLower().Contains("darksouls.exe"))
            {
                type = GameType.DarkSoulsPTDE;
            }
            else if (exePath.ToLower().Contains("darksoulsremastered.exe"))
            {
                type = GameType.DarkSoulsRemastered;
            }
            else if (exePath.ToLower().Contains("darksoulsii.exe"))
            {
                type = GameType.DarkSoulsIISOTFS;
            }
            else if (exePath.ToLower().Contains("darksoulsiii.exe"))
            {
                type = GameType.DarkSoulsIII;
            }
            else if (exePath.ToLower().Contains("eboot.bin"))
            {
                var path = Path.GetDirectoryName(exePath);
                if (Directory.Exists($@"{path}\dvdroot_ps4"))
                {
                    type = GameType.Bloodborne;
                }
                else
                {
                    type = GameType.DemonsSouls;
                }
            }
            else if (exePath.ToLower().Contains("sekiro.exe"))
            {
                type = GameType.Sekiro;
            }
            else if (exePath.ToLower().Contains("eldenring.exe"))
            {
                type = GameType.EldenRing;
            }
            return type;
        }

        public bool CheckFilesExpanded(string gamepath, GameType game)
        {
            if (game == GameType.DarkSoulsPTDE || game == GameType.DarkSoulsIII || game == GameType.Sekiro)
            {
                if (!Directory.Exists($@"{gamepath}\map"))
                {
                    return false;
                }
                if (!Directory.Exists($@"{gamepath}\obj"))
                {
                    return false;
                }
            }
            if (game == GameType.DarkSoulsIISOTFS)
            {
                if (!Directory.Exists($@"{gamepath}\map"))
                {
                    return false;
                }
                if (!Directory.Exists($@"{gamepath}\model\obj"))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Sets the game root directory by giving a path to the game exe/eboot.bin. Autodetects the game type.
        /// </summary>
        /// <param name="exePath">Path to an exe/eboot.bin</param>
        /// <returns>true if the game was autodetected</returns>
        public bool SetGameRootDirectoryByExePath(string exePath)
        {
            GameRootDirectory = Path.GetDirectoryName(exePath);
            if (exePath.ToLower().Contains("darksouls.exe"))
            {
                Type = GameType.DarkSoulsPTDE;
            }
            else if (exePath.ToLower().Contains("darksoulsremastered.exe"))
            {
                Type = GameType.DarkSoulsRemastered;
            }
            else if (exePath.ToLower().Contains("darksoulsii.exe"))
            {
                Type = GameType.DarkSoulsIISOTFS;
            }
            else if (exePath.ToLower().Contains("darksoulsiii.exe"))
            {
                Type = GameType.DarkSoulsIII;
            }
            else if (exePath.ToLower().Contains("eboot.bin"))
            {
                if (Directory.Exists($@"{GameRootDirectory}\dvdroot_ps4"))
                {
                    Type = GameType.Bloodborne;
                    GameRootDirectory = GameRootDirectory + $@"\dvdroot_ps4";
                }
                else
                {
                    Type = GameType.DemonsSouls;
                }
            }
            else if (exePath.ToLower().Contains("sekiro.exe"))
            {
                Type = GameType.Sekiro;
            }
            else if (exePath.ToLower().Contains("eldenring.exe"))
            {
                Type = GameType.EldenRing;
            }
            else
            {
                GameRootDirectory = null;
            }

            // Invalidate various caches
            FullMapList = null;
            GameModDirectory = null;

            return true;
        }

        public void SetModProjectDirectory(string dir)
        {
            GameModDirectory = dir;
        }

        public void SetFromProjectSettings(Editor.ProjectSettings settings, string moddir)
        {
            Type = settings.GameType;
            GameRootDirectory = settings.GameRoot;
            GameModDirectory = moddir;
            FullMapList = null;
        }

        public bool CreateRecoveryProject()
        {
            if (GameRootDirectory == null || GameModDirectory == null)
            {
                return false;
            }

            try
            {
                var time = DateTime.Now.ToString("dd-MM-yyyy-(hh-mm-ss)", CultureInfo.InvariantCulture);
                GameModDirectory = GameModDirectory + $@"\recovery\{time}";
                if (!Directory.Exists(GameModDirectory))
                    Directory.CreateDirectory(GameModDirectory);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool FileExists(string relpath)
        {
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{relpath}"))
            {
                return true;
            }
            else if (File.Exists($@"{GameRootDirectory}\{relpath}"))
            {
                return true;
            }
            return false;
        }

        public string GetOverridenFilePath(string relpath)
        {
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{relpath}"))
            {
                return $@"{GameModDirectory}\{relpath}";
            }
            else if (File.Exists($@"{GameRootDirectory}\{relpath}"))
            {
                return $@"{GameRootDirectory}\{relpath}";
            }
            return null;
        }

        /// <summary>
        /// Gets the full list of maps in the game (excluding chalice dungeons). Basically if there's an msb for it,
        /// it will be in this list.
        /// </summary>
        /// <returns></returns>
        public List<string> GetFullMapList()
        {
            if (GameRootDirectory == null)
            {
                return null;
            }

            if (FullMapList != null)
            {
                return FullMapList;
            }

            try
            {
                var mapSet = new HashSet<string>();

                // DS2 has its own structure for msbs, where they are all inside individual folders
                if (Type == GameType.DarkSoulsIISOTFS)
                {
                    var maps = Directory.GetFileSystemEntries(GameRootDirectory + @"\map", @"m*").ToList();
                    if (GameModDirectory != null)
                    {
                        if (Directory.Exists(GameModDirectory + @"\map"))
                        {
                            maps.AddRange(Directory.GetFileSystemEntries(GameModDirectory + @"\map", @"m*").ToList());
                        }
                    }

                    foreach (var map in maps)
                    {
                        mapSet.Add(Path.GetFileNameWithoutExtension($@"{map}.blah"));
                    }
                }
                else
                {
                    var msbFiles = Directory.GetFileSystemEntries(GameRootDirectory + @"\map\MapStudio\", @"*.msb")
                        .Select(Path.GetFileNameWithoutExtension).ToList();
                    msbFiles.AddRange(Directory
                        .GetFileSystemEntries(GameRootDirectory + @"\map\MapStudio\", @"*.msb.dcx")
                        .Select(Path.GetFileNameWithoutExtension).Select(Path.GetFileNameWithoutExtension).ToList());
                    if (GameModDirectory != null && Directory.Exists(GameModDirectory + @"\map\MapStudio\"))
                    {
                        msbFiles.AddRange(Directory
                            .GetFileSystemEntries(GameModDirectory + @"\map\MapStudio\", @"*.msb")
                            .Select(Path.GetFileNameWithoutExtension).ToList());
                        msbFiles.AddRange(Directory
                            .GetFileSystemEntries(GameModDirectory + @"\map\MapStudio\", @"*.msb.dcx")
                            .Select(Path.GetFileNameWithoutExtension).Select(Path.GetFileNameWithoutExtension)
                            .ToList());
                    }

                    foreach (var msb in msbFiles)
                    {
                        mapSet.Add(msb);
                    }
                }

                var mapRegex = new Regex(@"^m\d{2}_\d{2}_\d{2}_\d{2}$");
                var mapList = mapSet.Where(x => mapRegex.IsMatch(x)).ToList();
                mapList.Sort();
                FullMapList = mapList;
                return FullMapList;
            }
            catch (DirectoryNotFoundException e)
            {
                // Game is likely not UXM unpacked
                return new List<string>();
            }
        }

        public AssetDescription GetMapMSB(string mapid, bool writemode = false)
        {
            AssetDescription ad = new AssetDescription();
            ad.AssetPath = null;
            if (mapid.Length != 12)
            {
                return ad;
            }
            string preferredPath;
            string backupPath;
            // SOFTS
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                preferredPath = $@"map\{mapid}\{mapid}.msb";
                backupPath = $@"map\{mapid}\{mapid}.msb";
            }
            // BB chalice maps
            else if (Type == GameType.Bloodborne && mapid.StartsWith("m29"))
            {
                preferredPath = $@"\map\MapStudio\{mapid.Substring(0, 9)}_00\{mapid}.msb.dcx";
                backupPath = $@"\map\MapStudio\{mapid.Substring(0, 9)}_00\{mapid}.msb";
            }
            // DeS, DS1, DS1R
            else if (Type == GameType.DarkSoulsPTDE || Type == GameType.DarkSoulsRemastered || Type == GameType.DemonsSouls)
            {
                preferredPath = $@"\map\MapStudio\{mapid}.msb";
                backupPath = $@"\map\MapStudio\{mapid}.msb.dcx";
            }
            // BB, DS3, ER, SSDT
            else if (Type == GameType.Bloodborne || Type == GameType.DarkSoulsIII || Type == GameType.EldenRing || Type == GameType.Sekiro)
            {
                preferredPath = $@"\map\MapStudio\{mapid}.msb.dcx";
                backupPath = $@"\map\MapStudio\{mapid}.msb";
            }
            else
            {
                preferredPath = $@"\map\MapStudio\{mapid}.msb.dcx";
                backupPath = $@"\map\MapStudio\{mapid}.msb";
            }

            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{preferredPath}") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{preferredPath}";
            }
            else if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{backupPath}") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{backupPath}";
            }
            else if (File.Exists($@"{GameRootDirectory}\{preferredPath}"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{preferredPath}";
            }
            else if (File.Exists($@"{GameRootDirectory}\{backupPath}"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{backupPath}";
            }
            ad.AssetName = mapid;
            return ad;
        }


        public List<AssetDescription> GetMapBTLs(string mapid, bool writemode = false)
        {
            List<AssetDescription> adList = new();
            if (mapid.Length != 12)
            {
                return adList;
            }

            if (Type is GameType.DarkSoulsIISOTFS)
            {
                // DS2 BTL is located inside map's .gibdt file
                AssetDescription ad = new();
                var path = $@"model\map\g{mapid[1..]}.gibhd";

                if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}") || (writemode && GameModDirectory != null))
                {
                    ad.AssetPath = $@"{GameModDirectory}\{path}";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}"))
                {
                    ad.AssetPath = $@"{GameRootDirectory}\{path}";
                }
                if (ad.AssetPath != null)
                {
                    ad.AssetName = $@"g{mapid[1..]}";
                    ad.AssetVirtualPath = $@"{mapid}\light.btl.dcx";
                    adList.Add(ad);
                }

                AssetDescription ad2 = new();
                path = $@"model_lq\map\g{mapid[1..]}.gibhd";

                if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}") || (writemode && GameModDirectory != null))
                {
                    ad2.AssetPath = $@"{GameModDirectory}\{path}";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}"))
                {
                    ad2.AssetPath = $@"{GameRootDirectory}\{path}";
                }
                if (ad2.AssetPath != null)
                {
                    ad2.AssetName = $@"g{mapid[1..]}_lq";
                    ad2.AssetVirtualPath = $@"{mapid}\light.btl.dcx";
                    adList.Add(ad2);
                }
            }
            else if (Type is GameType.Bloodborne or GameType.DarkSoulsIII or GameType.Sekiro or GameType.EldenRing)
            {
                string path;
                if (Type is GameType.EldenRing)
                    path = $@"map\{mapid[..3]}\{mapid}";
                else
                    path = $@"map\{mapid}";

                List<string> files = new();
                try
                {
                    files = Directory.GetFiles($@"{GameRootDirectory}\{path}", "*.btl*").Where(f => !f.EndsWith(".bak")).ToList();
                    if (Directory.Exists($"{GameModDirectory}\\{path}"))
                    {
                        // Check for additional BTLs the user has created.
                        files.AddRange(Directory.GetFiles($@"{GameModDirectory}\{path}", "*.btl*").Where(f => !f.EndsWith(".bak")).ToList());
                        files = files.DistinctBy(f => f.Split("\\").Last()).ToList();
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    return adList;
                }

                foreach (var file in files)
                {
                    AssetDescription ad = new();
                    var fileName = file.Split("\\").Last();
                    if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}\{fileName}") || (writemode && GameModDirectory != null))
                    {
                        ad.AssetPath = $@"{GameModDirectory}\{path}\{fileName}";
                    }
                    else if (File.Exists($@"{GameRootDirectory}\{path}\{fileName}"))
                    {
                        ad.AssetPath = $@"{GameRootDirectory}\{path}\{fileName}";
                    }
                    if (ad.AssetPath != null)
                    {
                        ad.AssetName = fileName;
                        adList.Add(ad);
                    }
                }
            }
            return adList;
        }

        public AssetDescription GetMapNVA(string mapid, bool writemode = false)
        {
            AssetDescription ad = new AssetDescription();
            ad.AssetPath = null;
            if (mapid.Length != 12)
            {
                return ad;
            }
            // BB chalice maps
            else if (Type == GameType.Bloodborne && mapid.StartsWith("m29"))
            {
                var path = $@"\map\{mapid.Substring(0, 9)}_00\{mapid}";
                if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.nva.dcx") || (writemode && GameModDirectory != null && Type != GameType.DarkSoulsPTDE))
                {
                    ad.AssetPath = $@"{GameModDirectory}\{path}.nva.dcx";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}.nva.dcx"))
                {
                    ad.AssetPath = $@"{GameRootDirectory}\{path}.nva.dcx";
                }
            }
            else
            {
                var path = $@"\map\{mapid}\{mapid}";
                if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.nva.dcx") || (writemode && GameModDirectory != null && Type != GameType.DarkSoulsPTDE))
                {
                    ad.AssetPath = $@"{GameModDirectory}\{path}.nva.dcx";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}.nva.dcx"))
                {
                    ad.AssetPath = $@"{GameRootDirectory}\{path}.nva.dcx";
                }
                else if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.nva") || (writemode && GameModDirectory != null))
                {
                    ad.AssetPath = $@"{GameModDirectory}\{path}.nva";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}.nva"))
                {
                    ad.AssetPath = $@"{GameRootDirectory}\{path}.nva";
                }
            }
            ad.AssetName = mapid;
            return ad;
        }

        /// <summary>
        /// Get folders with msgbnds used in-game
        /// </summary>
        /// <returns>Dictionary with language name and path</returns>
        public Dictionary<string, string> GetMsgLanguages()
        {
            Dictionary<string, string> dict = new();
            List<string> folders = new();
            try
            {
                if (Type == GameType.DemonsSouls)
                {
                    folders = Directory.GetDirectories(GameRootDirectory + @"\msg").ToList();
                    // Japanese uses root directory
                    if (File.Exists(GameRootDirectory + @"\msg\menu.msgbnd.dcx") || File.Exists(GameRootDirectory + @"\msg\item.msgbnd.dcx"))
                        dict.Add("Japanese", "");
                }
                else if (Type == GameType.DarkSoulsIISOTFS)
                {
                    folders = Directory.GetDirectories(GameRootDirectory + @"\menu\text").ToList();
                }
                else
                {
                    // Exclude folders that don't have typical msgbnds
                    folders = Directory.GetDirectories(GameRootDirectory + @"\msg").Where(x => !"common,as,eu,jp,na,uk,japanese".Contains(x.Split("\\").Last())).ToList();
                }

                foreach (var path in folders)
                {
                    dict.Add(path.Split("\\").Last(), path);
                }
            }
            catch (Exception e) when (e is DirectoryNotFoundException or FileNotFoundException)
            {
            }
            return dict;
        }

        /// <summary>
        /// Get path of item.msgbnd (english by default)
        /// </summary>
        public AssetDescription GetItemMsgbnd(ref string langFolder, bool writemode = false)
        {
            return GetMsgbnd("item", ref langFolder, writemode);
        }
        /// <summary>
        /// Get path of menu.msgbnd (english by default)
        /// </summary>
        public AssetDescription GetMenuMsgbnd(ref string langFolder, bool writemode = false)
        {
            return GetMsgbnd("menu", ref langFolder, writemode);
        }
        public AssetDescription GetMsgbnd(string msgBndType, ref string langFolder, bool writemode = false)
        {
            AssetDescription ad = new();
            if (langFolder == "")
            {
                //pick default (english) path
                foreach (var lang in GetMsgLanguages())
                {
                    string folder = lang.Value.Split("\\").Last();
                    if (folder.Contains("eng", StringComparison.CurrentCultureIgnoreCase)) //I believe this is good enough.
                    {
                        langFolder = folder;
                        break;
                    }
                }
                if (langFolder == "")
                {
                    // Could not find default [english] text msgbnd.
                    return ad;
                }
            }

            string path = $@"msg\{langFolder}\{msgBndType}.msgbnd.dcx";
            if (Type == GameType.DemonsSouls)
            {
                path = $@"msg\{langFolder}\{msgBndType}.msgbnd.dcx";
                // Demon's Souls has msgbnds directly in the msg folder
                if (!File.Exists($@"{GameRootDirectory}\{path}"))
                {
                    path = $@"msg\{msgBndType}.msgbnd.dcx";
                }
            }
            else if (Type == GameType.DarkSoulsPTDE)
            {
                path = $@"msg\{langFolder}\{msgBndType}.msgbnd";
            }
            else if (Type == GameType.DarkSoulsRemastered)
            {
                path = $@"msg\{langFolder}\{msgBndType}.msgbnd.dcx";
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                // DS2 does not have an msgbnd but loose fmg files instead
                path = $@"menu\text\{langFolder}";
                AssetDescription ad2 = new AssetDescription();
                ad2.AssetPath = writemode ? path : $@"{GameRootDirectory}\{path}";
                //TODO: doesn't support project files
                return ad2;
            }
            else if (Type == GameType.DarkSoulsIII)
            {
                path = $@"msg\{langFolder}\{msgBndType}_dlc2.msgbnd.dcx";
            }

            if (writemode)
            {
                ad.AssetPath = path;
                return ad;
            }
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{path}";
            }
            else if (File.Exists($@"{GameRootDirectory}\{path}"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{path}";
            }
            return ad;
        }
        private string GetGameIDForDir()
        {
            switch (Type)
            {
                case GameType.DemonsSouls:
                    return "DES";
                case GameType.DarkSoulsPTDE:
                    return "DS1";
                case GameType.DarkSoulsRemastered:
                    return "DS1R";
                case GameType.DarkSoulsIISOTFS:
                    return "DS2S";
                case GameType.Bloodborne:
                    return "BB";
                case GameType.DarkSoulsIII:
                    return "DS3";
                case GameType.Sekiro:
                    return "SDT";
                case GameType.EldenRing:
                    return "ER";
                default:
                    throw new Exception("Game type not set");
            }
        }
    	public string GetAliasAssetsDir()
        {
            return  $@"Assets\Aliases\{GetGameIDForDir()}";
        }

        public string GetGameOffsetsAssetsDir()
        {
            return  $@"Assets\GameOffsets\{GetGameIDForDir()}";
        }

        public string GetParamAssetsDir()
        {
            return  $@"Assets\Paramdex\{GetGameIDForDir()}";
        }

        public string GetParamdefDir()
        {
            return $@"{GetParamAssetsDir()}\Defs";
        }
        
        public ulong[] GetParamdefPatches()
        {
            if (Directory.Exists($@"{GetParamAssetsDir()}\DefsPatch"))
            {
                var entries = Directory.GetFileSystemEntries($@"{GetParamAssetsDir()}\DefsPatch");
                return entries.Select(e => ulong.Parse(Path.GetFileNameWithoutExtension(e))).ToArray();
            }
            return new ulong[]  { };
        }
        
        public string GetParamdefPatchDir(ulong patch)
        {
            return $@"{GetParamAssetsDir()}\DefsPatch\{patch}";
        }

        public string GetParammetaDir()
        {
            return $@"{GetParamAssetsDir()}\Meta";
        }

        public string GetParamNamesDir()
        {
            return $@"{GetParamAssetsDir()}\Names";
        }

        public PARAMDEF GetParamdefForParam(string paramType)
        {
            PARAMDEF pd = PARAMDEF.XmlDeserialize($@"{GetParamdefDir()}\{paramType}.xml");
            ParamEditor.ParamMetaData meta = ParamEditor.ParamMetaData.XmlDeserialize($@"{GetParammetaDir()}\{paramType}.xml", pd);
            return pd;
        }

        public AssetDescription GetDS2GeneratorParam(string mapid, bool writemode=false)
        {
            AssetDescription ad = new AssetDescription();
            var path = $@"Param\generatorparam_{mapid}";
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.param") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{path}.param";
            }
            else if (File.Exists($@"{GameRootDirectory}\{path}.param"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{path}.param";
            }
            ad.AssetName = mapid + "_generators";
            return ad;
        }

        public AssetDescription GetDS2GeneratorLocationParam(string mapid, bool writemode = false)
        {
            AssetDescription ad = new AssetDescription();
            var path = $@"Param\generatorlocation_{mapid}";
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.param") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{path}.param";
            }
            else if (File.Exists($@"{GameRootDirectory}\{path}.param"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{path}.param";
            }
            ad.AssetName = mapid + "_generator_locations";
            return ad;
        }

        public AssetDescription GetDS2GeneratorRegistParam(string mapid, bool writemode = false)
        {
            AssetDescription ad = new AssetDescription();
            var path = $@"Param\generatorregistparam_{mapid}";
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.param") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{path}.param";
            }
            else if (File.Exists($@"{GameRootDirectory}\{path}.param"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{path}.param";
            }
            ad.AssetName = mapid + "_generator_registrations";
            return ad;
        }

        public AssetDescription GetDS2EventParam(string mapid, bool writemode = false)
        {
            AssetDescription ad = new AssetDescription();
            var path = $@"Param\eventparam_{mapid}";
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.param") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{path}.param";
            }
            else if (File.Exists($@"{GameRootDirectory}\{path}.param"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{path}.param";
            }
            ad.AssetName = mapid + "_event_params";
            return ad;
        }

        public AssetDescription GetDS2EventLocationParam(string mapid, bool writemode = false)
        {
            AssetDescription ad = new AssetDescription();
            var path = $@"Param\eventlocation_{mapid}";
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.param") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{path}.param";
            }
            else if (File.Exists($@"{GameRootDirectory}\{path}.param"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{path}.param";
            }
            ad.AssetName = mapid + "_event_locations";
            return ad;
        }

        public AssetDescription GetDS2ObjInstanceParam(string mapid, bool writemode = false)
        {
            AssetDescription ad = new AssetDescription();
            var path = $@"Param\mapobjectinstanceparam_{mapid}";
            if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.param") || (writemode && GameModDirectory != null))
            {
                ad.AssetPath = $@"{GameModDirectory}\{path}.param";
            }
            else if (File.Exists($@"{GameRootDirectory}\{path}.param"))
            {
                ad.AssetPath = $@"{GameRootDirectory}\{path}.param";
            }
            ad.AssetName = mapid + "_object_instance_params";
            return ad;
        }

        public List<AssetDescription> GetMapModels(string mapid)
        {
            var ret = new List<AssetDescription>();
            if (Type == GameType.DarkSoulsIII || Type == GameType.Sekiro)
            {
                if (!Directory.Exists(GameRootDirectory + $@"\map\{mapid}\"))
                    return ret;
                var mapfiles = Directory.GetFileSystemEntries(GameRootDirectory + $@"\map\{mapid}\", @"*.mapbnd.dcx").ToList();
                foreach (var f in mapfiles)
                {
                    var ad = new AssetDescription();
                    ad.AssetPath = f;
                    var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f));
                    ad.AssetName = name;
                    ad.AssetArchiveVirtualPath = $@"map/{mapid}/model/{name}";
                    ad.AssetVirtualPath = $@"map/{mapid}/model/{name}/{name}.flver";
                    ret.Add(ad);
                }
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                var ad = new AssetDescription();
                var name = mapid;
                ad.AssetName = name;
                ad.AssetArchiveVirtualPath = $@"map/{mapid}/model";
                ret.Add(ad);
            }
            else
            {
                if (!Directory.Exists(GameRootDirectory + $@"\map\{mapid}\"))
                    return ret;
                var ext = Type == GameType.DarkSoulsPTDE ? @"*.flver" : @"*.flver.dcx";
                var mapfiles = Directory.GetFileSystemEntries(GameRootDirectory + $@"\map\{mapid}\", ext).ToList();
                foreach (var f in mapfiles)
                {
                    var ad = new AssetDescription();
                    ad.AssetPath = f;
                    var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f));
                    ad.AssetName = name;
                    // ad.AssetArchiveVirtualPath = $@"map/{mapid}/model/{name}";
                    ad.AssetVirtualPath = $@"map/{mapid}/model/{name}/{name}.flver";
                    ret.Add(ad);
                }
            }
            return ret;
        }

        public string MapModelNameToAssetName(string mapid, string modelname)
        {
            if (Type == GameType.DarkSoulsPTDE || Type == GameType.DarkSoulsRemastered)
            {
                return $@"{modelname}A{mapid.Substring(1, 2)}";
            }
            else if (Type == GameType.DemonsSouls)
            {
                return $@"{modelname}";
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                return modelname;
            }
            return $@"{mapid}_{modelname.Substring(1)}";
        }

        /// <summary>
        /// Gets the adjusted map ID that contains all the map assets
        /// </summary>
        /// <param name="mapid">The msb map ID to adjust</param>
        /// <returns>The map ID for the purpose of asset storage</returns>
        public string GetAssetMapID(string mapid)
        {
            var amapid = mapid.Substring(0, 6) + "_00_00";
            if (Type == GameType.EldenRing)
            {
                // Elden Ring all maps have their own assets
                amapid = mapid;
            }
            // Special case for chalice dungeon assets
            if (mapid.StartsWith("m29"))
            {
                amapid = "m29_00_00_00";
            }

            return amapid;
        }
        
        public AssetDescription GetMapModel(string mapid, string model)
        {
            var ret = new AssetDescription();
            if (Type == GameType.DarkSoulsPTDE || Type == GameType.Bloodborne || Type == GameType.DemonsSouls)
            {
                ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.flver");
            }
            else if (Type == GameType.DarkSoulsRemastered)
            {
                ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.flver.dcx");
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetPath = GetAssetPath($@"model\map\{mapid}.mapbhd");
            }
            else
            {
                ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.mapbnd.dcx");
            }
            ret.AssetName = model;
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetArchiveVirtualPath = $@"map/{mapid}/model";
                ret.AssetVirtualPath = $@"map/{mapid}/model/{model}.flv.dcx";
            }
            else
            {
                if (Type != GameType.DarkSoulsPTDE && Type != GameType.DarkSoulsRemastered && Type != GameType.Bloodborne && Type != GameType.DemonsSouls)
                {
                    ret.AssetArchiveVirtualPath = $@"map/{mapid}/model/{model}";
                }
                ret.AssetVirtualPath = $@"map/{mapid}/model/{model}/{model}.flver";
            }
            return ret;
        }

        public AssetDescription GetMapCollisionModel(string mapid, string model, bool hi=true)
        {
            var ret = new AssetDescription();
            if (Type == GameType.DarkSoulsPTDE || Type == GameType.DemonsSouls)
            {
                if (hi)
                {
                    ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.hkx");
                    ret.AssetName = model;
                    ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/{model}.hkx";
                }
                else
                {
                    ret.AssetPath = GetAssetPath($@"map\{mapid}\l{model.Substring(1)}.hkx");
                    ret.AssetName = model;
                    ret.AssetVirtualPath = $@"map/{mapid}/hit/lo/l{model.Substring(1)}.hkx";
                }
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetPath = GetAssetPath($@"model\map\h{mapid.Substring(1)}.hkxbhd");
                ret.AssetName = model;
                ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/{model}.hkx.dcx";
                ret.AssetArchiveVirtualPath = $@"map/{mapid}/hit/hi";
            }
            else if (Type == GameType.DarkSoulsIII || Type == GameType.Bloodborne)
            {
                if (hi)
                {
                    ret.AssetPath = GetAssetPath($@"map\{mapid}\h{mapid.Substring(1)}.hkxbhd");
                    ret.AssetName = model;
                    ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/h{model.Substring(1)}.hkx.dcx";
                    ret.AssetArchiveVirtualPath = $@"map/{mapid}/hit/hi";
                }
                else
                {
                    ret.AssetPath = GetAssetPath($@"map\{mapid}\l{mapid.Substring(1)}.hkxbhd");
                    ret.AssetName = model;
                    ret.AssetVirtualPath = $@"map/{mapid}/hit/lo/l{model.Substring(1)}.hkx.dcx";
                    ret.AssetArchiveVirtualPath = $@"map/{mapid}/hit/lo";
                }
            }
            else
            {
                return GetNullAsset();
            }
            return ret;
        }

        public List<AssetDescription> GetMapTextures(string mapid)
        {
            List<AssetDescription> ads = new List<AssetDescription>();

            if (Type == GameType.DarkSoulsIISOTFS)
            {
                var t = new AssetDescription();
                t.AssetPath = GetAssetPath($@"model\map\t{mapid.Substring(1)}.tpfbhd");
                t.AssetArchiveVirtualPath = $@"map/tex/{mapid}/tex";
                ads.Add(t);
            }
            else if (Type == GameType.DarkSoulsPTDE)
            {
                // TODO
            }
            else if (Type == GameType.EldenRing)
            {
                // TODO ER
            }
            else if (Type == GameType.DemonsSouls)
            {
                var mid = mapid.Substring(0, 3);
                var paths = Directory.GetFileSystemEntries($@"{GameRootDirectory}\map\{mid}\", "*.tpf.dcx");
                foreach (var path in paths)
                {
                    var ad = new AssetDescription();
                    ad.AssetPath = path;
                    var tid = Path.GetFileNameWithoutExtension(path).Substring(4, 4);
                    ad.AssetVirtualPath = $@"map/tex/{mid}/{tid}";
                    ads.Add(ad);
                }
            }
            else
            {
                // Clean this up. Even if it's common code having something like "!=Sekiro" can lead to future issues
                var mid = mapid.Substring(0, 3);

                var t0000 = new AssetDescription();
                t0000.AssetPath = GetAssetPath($@"map\{mid}\{mid}_0000.tpfbhd");
                t0000.AssetArchiveVirtualPath = $@"map/tex/{mid}/0000";
                ads.Add(t0000);

                var t0001 = new AssetDescription();
                t0001.AssetPath = GetAssetPath($@"map\{mid}\{mid}_0001.tpfbhd");
                t0001.AssetArchiveVirtualPath = $@"map/tex/{mid}/0001";
                ads.Add(t0001);

                var t0002 = new AssetDescription();
                t0002.AssetPath = GetAssetPath($@"map\{mid}\{mid}_0002.tpfbhd");
                t0002.AssetArchiveVirtualPath = $@"map/tex/{mid}/0002";
                ads.Add(t0002);

                var t0003 = new AssetDescription();
                t0003.AssetPath = GetAssetPath($@"map\{mid}\{mid}_0003.tpfbhd");
                t0003.AssetArchiveVirtualPath = $@"map/tex/{mid}/0003";
                ads.Add(t0003);

                if (Type == GameType.DarkSoulsRemastered)
                {
                    var env = new AssetDescription();
                    env.AssetPath = GetAssetPath($@"map\{mid}\GI_EnvM_{mid}.tpfbhd");
                    env.AssetArchiveVirtualPath = $@"map/tex/{mid}/env";
                    ads.Add(env);
                }
                else if (Type == GameType.Bloodborne || Type == GameType.DarkSoulsIII)
                {
                    var env = new AssetDescription();
                    env.AssetPath = GetAssetPath($@"map\{mid}\{mid}_envmap.tpf.dcx");
                    env.AssetVirtualPath = $@"map/tex/{mid}/env";
                    ads.Add(env);
                }
                else if (Type == GameType.Sekiro)
                {
                    //TODO SDT
                }
            }

            return ads;
        }

        public List<string> GetEnvMapTextureNames(string mapid)
        {
            var l = new List<string>();
            if (Type == GameType.DarkSoulsIII)
            {
                var mid = mapid.Substring(0, 3);
                if (File.Exists(GetAssetPath($@"map\{mid}\{mid}_envmap.tpf.dcx")))
                {
                    var t = TPF.Read(GetAssetPath($@"map\{mid}\{mid}_envmap.tpf.dcx"));
                    foreach (var tex in t.Textures)
                    {
                        l.Add(tex.Name);
                    }
                }
            }
            return l;
        }

        public AssetDescription GetChrTextures(string chrid)
        {
            AssetDescription ad = new AssetDescription();
            ad.AssetArchiveVirtualPath = null;
            ad.AssetPath = null;
            if (Type == GameType.EldenRing)
            {
                // Maybe add an option down the line to load lower quality
                string path = GetOverridenFilePath($@"chr\{chrid}_h.texbnd.dcx");
                if (path != null)
                {
                    ad.AssetPath = path;
                    ad.AssetArchiveVirtualPath = $@"chr/{chrid}/tex";
                }
            }
            if (Type == GameType.DarkSoulsIII || Type == GameType.Sekiro)
            {
                string path = GetOverridenFilePath($@"chr\{chrid}.texbnd.dcx");
                if (path != null)
                {
                    ad.AssetPath = path;
                    ad.AssetArchiveVirtualPath = $@"chr/{chrid}/tex";
                }
            }
            if (Type == GameType.Bloodborne)
            {
                string path = GetOverridenFilePath($@"chr\{chrid}_2.tpf.dcx");
                if (path != null)
                {
                    ad.AssetPath = path;
                    ad.AssetVirtualPath = $@"chr/{chrid}/tex";
                }
            }
            return ad;
        }

        public AssetDescription GetMapNVMModel(string mapid, string model)
        {
            var ret = new AssetDescription();
            if (Type == GameType.DarkSoulsPTDE || Type == GameType.DarkSoulsRemastered || Type == GameType.DemonsSouls)
            {
                ret.AssetPath = GetAssetPath($@"map\{mapid}\{model}.nvm");
                ret.AssetName = model;
                ret.AssetArchiveVirtualPath = $@"map/{mapid}/nav";
                ret.AssetVirtualPath = $@"map/{mapid}/nav/{model}.nvm";
            }
            else
            {
                return GetNullAsset();
            }
            return ret;
        }

        public AssetDescription GetHavokNavmeshes(string mapid)
        {
            var ret = new AssetDescription();
            ret.AssetPath = GetAssetPath($@"map\{mapid}\{mapid}.nvmhktbnd.dcx");
            ret.AssetName = mapid;
            ret.AssetArchiveVirtualPath = $@"map/{mapid}/nav";
            return ret;
        }

        public AssetDescription GetHavokNavmeshModel(string mapid, string model)
        {
            var ret = new AssetDescription();
            ret.AssetPath = GetAssetPath($@"map\{mapid}\{mapid}.nvmhktbnd.dcx");
            ret.AssetName = model;
            ret.AssetArchiveVirtualPath = $@"map/{mapid}/nav";
            ret.AssetVirtualPath = $@"map/{mapid}/nav/{model}.hkx";

            return ret;
        }

        public List<string> GetChrModels()
        {
            try
            {
                var chrs = new HashSet<string>();
                var ret = new List<string>();

                string modelDir = $@"\chr";
                string modelExt = $@".chrbnd.dcx";
                if (Type == GameType.DarkSoulsPTDE)
                {
                    modelExt = ".chrbnd";
                }
                else if (Type == GameType.DarkSoulsIISOTFS)
                {
                    modelDir = $@"\model\chr";
                    modelExt = ".bnd";
                }

                if (Type == GameType.DemonsSouls)
                {
                    var chrdirs = Directory.GetDirectories(GameRootDirectory + modelDir);
                    foreach (var f in chrdirs)
                    {
                        var name = Path.GetFileNameWithoutExtension(f + ".dummy");
                        if (name.StartsWith("c"))
                        {
                            ret.Add(name);
                        }
                    }

                    return ret;
                }

                var chrfiles = Directory.GetFileSystemEntries(GameRootDirectory + modelDir, $@"*{modelExt}").ToList();
                foreach (var f in chrfiles)
                {
                    var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f));
                    ret.Add(name);
                    chrs.Add(name);
                }

                if (GameModDirectory != null && Directory.Exists(GameModDirectory + modelDir))
                {
                    chrfiles = Directory.GetFileSystemEntries(GameModDirectory + modelDir, $@"*{modelExt}").ToList();
                    foreach (var f in chrfiles)
                    {
                        var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f));
                        if (!chrs.Contains(name))
                        {
                            ret.Add(name);
                            chrs.Add(name);
                        }
                    }
                }

                return ret;
            }
            catch (DirectoryNotFoundException e)
            {
                // Game likely isn't UXM unpacked
                return new List<string>();
            }
        }

        public AssetDescription GetChrModel(string chr)
        {
            var ret = new AssetDescription();
            ret.AssetName = chr;
            ret.AssetArchiveVirtualPath = $@"chr/{chr}/model";
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetVirtualPath = $@"chr/{chr}/model/{chr}.flv";
            }
            else
            {
                ret.AssetVirtualPath = $@"chr/{chr}/model/{chr}.flver";
            }
            return ret;
        }

        public List<string> GetObjModels()
        {
            try
            {
                var objs = new HashSet<string>();
                var ret = new List<string>();

                string modelDir = $@"\obj";
                string modelExt = $@".objbnd.dcx";
                if (Type == GameType.DarkSoulsPTDE)
                {
                    modelExt = ".objbnd";
                }
                else if (Type == GameType.DarkSoulsIISOTFS)
                {
                    modelDir = $@"\model\obj";
                    modelExt = ".bnd";
                }
                else if (Type == GameType.EldenRing)
                {
                    // AEGs are objs in my heart :(
                    modelDir = @"\asset\aeg";
                    modelExt = ".geombnd.dcx";
                }

                // Directories to search for obj models
                List<string> searchDirs = new List<string>();
                if (Type == GameType.EldenRing)
                {
                    searchDirs = Directory.GetFileSystemEntries(GameRootDirectory + modelDir, $@"aeg*").ToList();
                }
                else
                {
                    searchDirs.Add(GameRootDirectory + modelDir);
                }

                foreach (var searchDir in searchDirs)
                {
                    var objfiles = Directory.GetFileSystemEntries(searchDir, $@"*{modelExt}").ToList();
                    foreach (var f in objfiles)
                    {
                        var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f));
                        ret.Add(name);
                        objs.Add(name);
                    }

                    if (GameModDirectory != null && Directory.Exists(searchDir))
                    {
                        objfiles = Directory.GetFileSystemEntries(searchDir, $@"*{modelExt}").ToList();
                        foreach (var f in objfiles)
                        {
                            var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f));
                            if (!objs.Contains(name))
                            {
                                ret.Add(name);
                                objs.Add(name);
                            }
                        }
                    }
                }

                return ret;
            }
            catch (DirectoryNotFoundException e)
            {
                // Game likely isn't UXM unpacked
                return new List<string>();
            }
        }

        public AssetDescription GetObjModel(string obj)
        {
            var ret = new AssetDescription();
            ret.AssetName = obj;
            ret.AssetArchiveVirtualPath = $@"obj/{obj}/model";
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetVirtualPath = $@"obj/{obj}/model/{obj}.flv";
            }
            else if (Type == GameType.EldenRing)
            {
                ret.AssetVirtualPath = $@"obj/{obj}/model/{obj.ToUpper()}.flver";
            }
            else
            {
                ret.AssetVirtualPath = $@"obj/{obj}/model/{obj}.flver";
            }
            return ret;
        }

        public AssetDescription GetNullAsset()
        {
            var ret = new AssetDescription();
            ret.AssetPath = "null";
            ret.AssetName = "null";
            ret.AssetArchiveVirtualPath = "null";
            ret.AssetVirtualPath = "null";
            return ret;
        }

        /// <summary>
        /// Converts a virtual path to an actual filesystem path. Only resolves virtual paths up to the bnd level,
        /// which the remaining string is output for additional handling
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public string VirtualToRealPath(string virtualPath, out string bndpath)
        {
            var pathElements = virtualPath.Split('/');
            var mapRegex = new Regex(@"^m\d{2}_\d{2}_\d{2}_\d{2}$");
            string ret = "";

            // Parse the virtual path with a DFA and convert it to a game path
            int i = 0;
            if (pathElements[i].Equals("map"))
            {
                i++;
                if (pathElements[i].Equals("tex"))
                {
                    i++;
                    if (Type == GameType.DarkSoulsIISOTFS)
                    {
                        var mid = pathElements[i];
                        i++;
                        var id = pathElements[i];
                        if (id == "tex")
                        {
                            bndpath = "";
                            return GetAssetPath($@"model\map\t{mid.Substring(1)}.tpfbhd");
                        }
                    }
                    else if (Type == GameType.DemonsSouls)
                    {
                        var mid = pathElements[i];
                        i++;
                        bndpath = "";
                        return GetAssetPath($@"map\{mid}\{mid}_{pathElements[i]}.tpf.dcx");
                    }
                    else
                    {
                        var mid = pathElements[i];
                        i++;
                        bndpath = "";
                        if (pathElements[i] == "env")
                        {
                            if (Type == GameType.DarkSoulsRemastered)
                            {
                                return GetAssetPath($@"map\{mid}\GI_EnvM_{mid}.tpf.dcx");
                            }
                            return GetAssetPath($@"map\{mid}\{mid}_envmap.tpf.dcx");
                        }
                        return GetAssetPath($@"map\{mid}\{mid}_{pathElements[i]}.tpfbhd");
                    }
                }
                else if (mapRegex.IsMatch(pathElements[i]))
                {
                    var mapid = pathElements[i];
                    i++;
                    if (pathElements[i].Equals("model"))
                    {
                        i++;
                        bndpath = "";
                        if (Type == GameType.DarkSoulsPTDE)
                        {
                            return GetAssetPath($@"map\{mapid}\{pathElements[i]}.flver");
                        }
                        else if (Type == GameType.DarkSoulsRemastered)
                        {
                            return GetAssetPath($@"map\{mapid}\{pathElements[i]}.flver.dcx");
                        }
                        else if (Type == GameType.DarkSoulsIISOTFS)
                        {
                            return GetAssetPath($@"model\map\{mapid}.mapbhd");
                        }
                        else if (Type == GameType.Bloodborne || Type == GameType.DemonsSouls)
                        {
                            return GetAssetPath($@"map\{mapid}\{pathElements[i]}.flver.dcx");
                        }
                        else if (Type == GameType.EldenRing)
                        {
                            return GetAssetPath($@"map\{mapid.Substring(0, 3)}\{mapid}\{pathElements[i]}.mapbnd.dcx");
                        }
                        return GetAssetPath($@"map\{mapid}\{pathElements[i]}.mapbnd.dcx");
                    }
                    else if (pathElements[i].Equals("hit"))
                    {
                        i++;
                        var hittype = pathElements[i];
                        i++;
                        if (Type == GameType.DarkSoulsPTDE || Type == GameType.DemonsSouls)
                        {
                            bndpath = "";
                            return GetAssetPath($@"map\{mapid}\{pathElements[i]}");
                        }
                        else if (Type == GameType.DarkSoulsIISOTFS)
                        {
                            bndpath = "";
                            return GetAssetPath($@"model\map\h{mapid.Substring(1)}.hkxbhd");
                        }
                        else if (Type == GameType.DarkSoulsIII || Type == GameType.Bloodborne)
                        {
                            bndpath = "";
                            if (hittype == "lo")
                            {
                                return GetAssetPath($@"map\{mapid}\l{mapid.Substring(1)}.hkxbhd");
                            }
                            return GetAssetPath($@"map\{mapid}\h{mapid.Substring(1)}.hkxbhd");
                        }
                        bndpath = "";
                        return null;
                    }
                    else if (pathElements[i].Equals("nav"))
                    {
                        i++;
                        if (Type == GameType.DarkSoulsPTDE || Type == GameType.DemonsSouls || Type == GameType.DarkSoulsRemastered)
                        {
                            if (i < pathElements.Length)
                            {
                                bndpath = $@"{pathElements[i]}";
                            }
                            else
                            {
                                bndpath = "";
                            }
                            if (Type == GameType.DarkSoulsRemastered)
                            {
                                return GetAssetPath($@"map\{mapid}\{mapid}.nvmbnd.dcx");
                            }
                            return GetAssetPath($@"map\{mapid}\{mapid}.nvmbnd");
                        }
                        else if (Type == GameType.DarkSoulsIII)
                        {
                            bndpath = "";
                            return GetAssetPath($@"map\{mapid}\{ mapid}.nvmhktbnd.dcx");
                        }
                        bndpath = "";
                        return null;
                    }
                }
            }
            else if (pathElements[i].Equals("chr"))
            {
                i++;
                var chrid = pathElements[i];
                i++;
                if (pathElements[i].Equals("model"))
                {
                    bndpath = "";
                    if (Type == GameType.DarkSoulsPTDE)
                    {
                        return GetOverridenFilePath($@"chr\{chrid}.chrbnd");
                    }
                    else if (Type == GameType.DarkSoulsIISOTFS)
                    {
                        return GetOverridenFilePath($@"model\chr\{chrid}.bnd");
                    }
                    else if (Type == GameType.DemonsSouls)
                    {
                        return GetOverridenFilePath($@"chr\{chrid}\{chrid}.chrbnd.dcx");
                    }
                    return GetOverridenFilePath($@"chr\{chrid}.chrbnd.dcx");
                }
                else if (pathElements[i].Equals("tex"))
                {
                    bndpath = "";
                    if (Type == GameType.EldenRing)
                    {
                        return GetOverridenFilePath($@"chr\{chrid}_h.texbnd.dcx");
                    }
                    if (Type == GameType.DarkSoulsIII || Type == GameType.Sekiro)
                    {
                        return GetOverridenFilePath($@"chr\{chrid}.texbnd.dcx");
                    }
                    else if (Type == GameType.Bloodborne)
                    {
                        return GetOverridenFilePath($@"chr\{chrid}_2.tpf.dcx");
                    }
                }
            }
            else if (pathElements[i].Equals("obj"))
            {
                i++;
                var objid = pathElements[i];
                i++;
                if (pathElements[i].Equals("model"))
                {
                    bndpath = "";
                    if (Type == GameType.DarkSoulsPTDE)
                    {
                        return GetOverridenFilePath($@"obj\{objid}.objbnd");
                    }
                    else if (Type == GameType.DarkSoulsIISOTFS)
                    {
                        return GetOverridenFilePath($@"model\obj\{objid}.bnd");
                    }
                    else if (Type == GameType.EldenRing)
                    {
                        // Derive subfolder path from model name (all vanilla AEG are within subfolders)
                        if (objid.Length >= 6)
                            return GetOverridenFilePath($@"asset\aeg\{objid.Substring(0, 6)}\{objid}.geombnd.dcx");
                        else
                            return null;
                    }
                    return GetOverridenFilePath($@"obj\{objid}.objbnd.dcx");
                }
            }

            bndpath = virtualPath;
            return null;
        }

        public string GetBinderVirtualPath(string virtualPathToBinder, string binderFilePath)
        {
            var filename = Path.GetFileNameWithoutExtension($@"{binderFilePath}.blah");
            if (filename.Length > 0)
            {
                filename = $@"{virtualPathToBinder}/{filename}";
            }
            else
            {
                filename = virtualPathToBinder;
            }
            return filename;
        }
    }
}
