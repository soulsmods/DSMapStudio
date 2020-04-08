using System;
using System.Collections.Generic;
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
            return type;
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

        public void SetFromProjectSettings(MsbEditor.ProjectSettings settings, string moddir)
        {
            Type = settings.GameType;
            GameRootDirectory = settings.GameRoot;
            GameModDirectory = moddir;
            FullMapList = null;
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
                msbFiles.AddRange(Directory.GetFileSystemEntries(GameRootDirectory + @"\map\MapStudio\", @"*.msb.dcx")
                    .Select(Path.GetFileNameWithoutExtension).Select(Path.GetFileNameWithoutExtension).ToList());
                if (GameModDirectory != null && Directory.Exists(GameModDirectory + @"\map\MapStudio\"))
                {
                    msbFiles.AddRange(Directory.GetFileSystemEntries(GameModDirectory + @"\map\MapStudio\", @"*.msb")
                    .Select(Path.GetFileNameWithoutExtension).ToList());
                    msbFiles.AddRange(Directory.GetFileSystemEntries(GameModDirectory + @"\map\MapStudio\", @"*.msb.dcx")
                        .Select(Path.GetFileNameWithoutExtension).Select(Path.GetFileNameWithoutExtension).ToList());
                }
                foreach (var msb in msbFiles)
                {
                    mapSet.Add(msb);
                }
            }
            var mapRegex = new Regex(@"^m\d{2}_\d{2}_\d{2}_\d{2}$");
            FullMapList = mapSet.Where(x => mapRegex.IsMatch(x)).ToList();
            FullMapList.Sort();
            return FullMapList;
        }

        public AssetDescription GetMapMSB(string mapid, bool writemode = false)
        {
            AssetDescription ad = new AssetDescription();
            ad.AssetPath = null;
            if (mapid.Length != 12)
            {
                return ad;
            }
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                var path = $@"map\{mapid}\{mapid}";
                if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.msb") || (writemode && GameModDirectory != null))
                {
                    ad.AssetPath = $@"{GameModDirectory}\{path}.msb";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}.msb"))
                {
                    ad.AssetPath = $@"{GameRootDirectory}\{path}.msb";
                }
            }
            // BB chalice maps
            else if (Type == GameType.Bloodborne && mapid.StartsWith("m29"))
            {
                var path = $@"\map\MapStudio\{mapid.Substring(0, 9)}_00\{mapid}";
                if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.msb.dcx") || (writemode && GameModDirectory != null && Type != GameType.DarkSoulsPTDE))
                {
                    ad.AssetPath = $@"{GameModDirectory}\{path}.msb.dcx";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}.msb.dcx"))
                {
                    ad.AssetPath = $@"{GameRootDirectory}\{path}.msb.dcx";
                }
            }
            else
            {
                var path = $@"\map\MapStudio\{mapid}";
                if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.msb.dcx") || (writemode && GameModDirectory != null && Type != GameType.DarkSoulsPTDE))
                {
                    ad.AssetPath = $@"{GameModDirectory}\{path}.msb.dcx";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}.msb.dcx"))
                {
                    ad.AssetPath = $@"{GameRootDirectory}\{path}.msb.dcx";
                }
                else if (GameModDirectory != null && File.Exists($@"{GameModDirectory}\{path}.msb") || (writemode && GameModDirectory != null))
                {
                    ad.AssetPath = $@"{GameModDirectory}\{path}.msb";
                }
                else if (File.Exists($@"{GameRootDirectory}\{path}.msb"))
                {
                    ad.AssetPath = $@"{GameRootDirectory}\{path}.msb";
                }
            }
            ad.AssetName = mapid;
            return ad;
        }

        public AssetDescription GetEnglishItemMsgbnd(bool writemode = false)
        {
            string path = $@"msg\engus\item.msgbnd.dcx";
            if (Type == GameType.DarkSoulsPTDE)
            {
                path = $@"msg\ENGLISH\item.msgbnd";
            }
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                // DS2 does not have an msgbnd but loose fmg files instead
                path = $@"menu\text\english";
                AssetDescription ad2 = new AssetDescription();
                ad2.AssetPath = writemode ? path : $@"{GameRootDirectory}\{path}";
                return ad2;
            }
            if (Type == GameType.DarkSoulsIII)
            {
                path = $@"msg\engus\item_dlc2.msgbnd.dcx";
            }
            AssetDescription ad = new AssetDescription();
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

        public string GetParamdefDir()
        {
            string game;
            switch (Type)
            {
                case GameType.DemonsSouls:
                    game = "DES";
                    break;
                case GameType.DarkSoulsPTDE:
                    game = "DS1";
                    break;
                case GameType.DarkSoulsRemastered:
                    game = "DS1R";
                    break;
                case GameType.DarkSoulsIISOTFS:
                    game = "DS2S";
                    break;
                case GameType.Bloodborne:
                    game = "BB";
                    break;
                case GameType.DarkSoulsIII:
                    game = "DS3";
                    break;
                case GameType.Sekiro:
                    game = "SDT";
                    break;
                default:
                    throw new Exception("Game type not set");
            }
            return $@"Assets\Paramdex\{game}\Defs";
        }

        public PARAMDEF GetParamdefForParam(string paramType)
        {
            string game;
            switch (Type)
            {
                case GameType.DemonsSouls:
                    game = "DES";
                    break;
                case GameType.DarkSoulsPTDE:
                    game = "DS1";
                    break;
                case GameType.DarkSoulsRemastered:
                    game = "DS1R";
                    break;
                case GameType.DarkSoulsIISOTFS:
                    game = "DS2S";
                    break;
                case GameType.Bloodborne:
                    game = "BB";
                    break;
                case GameType.DarkSoulsIII:
                    game = "DS3";
                    break;
                case GameType.Sekiro:
                    game = "SDT";
                    break;
                default:
                    throw new Exception("Game type not set");
            }

            return PARAMDEF.XmlDeserialize($@"Assets\Paramdex\{game}\Defs\{paramType}.xml");
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

        public List<AssetDescription> GetMapModels(string mapid)
        {
            var ret = new List<AssetDescription>();
            if (Type == GameType.DarkSoulsIII || Type == GameType.Sekiro)
            {
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
                var ext = Type == GameType.DarkSoulsPTDE ? @"*.flver" : @"*.flver.dcx";
                var mapfiles = Directory.GetFileSystemEntries(GameRootDirectory + $@"\map\{mapid}\", ext).ToList();
                foreach (var f in mapfiles)
                {
                    var ad = new AssetDescription();
                    ad.AssetPath = f;
                    var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f));
                    ad.AssetName = name;
                    //ad.AssetArchiveVirtualPath = $@"map/{mapid}/model/{name}";
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
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                return modelname;
            }
            return $@"{mapid}_{modelname.Substring(1)}";
        }

        public AssetDescription GetMapModel(string mapid, string model)
        {
            var ret = new AssetDescription();
            if (Type == GameType.DarkSoulsPTDE || Type == GameType.Bloodborne)
            {
                ret.AssetPath = $@"{GameRootDirectory}\map\{mapid}\{model}.flver";
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetPath = $@"{GameRootDirectory}\model\map\{mapid}.mapbhd";
            }
            else
            {
                ret.AssetPath = $@"{GameRootDirectory}\map\{mapid}\{model}.mapbnd.dcx";
            }
            ret.AssetName = model;
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetArchiveVirtualPath = $@"map/{mapid}/model";
                ret.AssetVirtualPath = $@"map/{mapid}/model/{model}.flv.dcx";
            }
            else
            {
                if (Type != GameType.DarkSoulsPTDE && Type != GameType.Bloodborne)
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
            if (Type == GameType.DarkSoulsPTDE)
            {
                if (hi)
                {
                    ret.AssetPath = $@"{GameRootDirectory}\map\{mapid}\{model}.hkx";
                    ret.AssetName = model;
                    ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/{model}.hkx";
                }
                else
                {
                    ret.AssetPath = $@"{GameRootDirectory}\map\{mapid}\l{model.Substring(1)}.hkx";
                    ret.AssetName = model;
                    ret.AssetVirtualPath = $@"map/{mapid}/hit/lo/l{model.Substring(1)}.hkx";
                }
            }
            else if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetPath = $@"{GameRootDirectory}\model\map\h{mapid.Substring(1)}.hkxbhd";
                ret.AssetName = model;
                ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/{model}.hkx.dcx";
                ret.AssetArchiveVirtualPath = $@"map/{mapid}/hit/hi";
            }
            else if (Type == GameType.DarkSoulsIII || Type == GameType.Bloodborne)
            {
                if (hi)
                {
                    ret.AssetPath = $@"{GameRootDirectory}\map\{mapid}\h{mapid.Substring(1)}.hkxbhd";
                    ret.AssetName = model;
                    ret.AssetVirtualPath = $@"map/{mapid}/hit/hi/h{model.Substring(1)}.hkx.dcx";
                    ret.AssetArchiveVirtualPath = $@"map/{mapid}/hit/hi";
                }
                else
                {
                    ret.AssetPath = $@"{GameRootDirectory}\map\{mapid}\l{mapid.Substring(1)}.hkxbhd";
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
                t.AssetPath = $@"{GameRootDirectory}\model\map\t{mapid.Substring(1)}.tpfbhd";
                t.AssetArchiveVirtualPath = $@"map/tex/{mapid}/tex";
                ads.Add(t);
            }
            else
            {
                var mid = mapid.Substring(0, 3);

                var t0000 = new AssetDescription();
                t0000.AssetPath = $@"{GameRootDirectory}\map\{mid}\{mid}_0000.tpfbhd";
                t0000.AssetArchiveVirtualPath = $@"map/tex/{mid}/0000";
                ads.Add(t0000);

                var t0001 = new AssetDescription();
                t0001.AssetPath = $@"{GameRootDirectory}\map\{mid}\{mid}_0001.tpfbhd";
                t0001.AssetArchiveVirtualPath = $@"map/tex/{mid}/0001";
                ads.Add(t0001);

                var t0002 = new AssetDescription();
                t0002.AssetPath = $@"{GameRootDirectory}\map\{mid}\{mid}_0002.tpfbhd";
                t0002.AssetArchiveVirtualPath = $@"map/tex/{mid}/0002";
                ads.Add(t0002);

                var t0003 = new AssetDescription();
                t0003.AssetPath = $@"{GameRootDirectory}\map\{mid}\{mid}_0003.tpfbhd";
                t0003.AssetArchiveVirtualPath = $@"map/tex/{mid}/0003";
                ads.Add(t0003);

                var env = new AssetDescription();
                env.AssetPath = $@"{GameRootDirectory}\map\{mid}\{mid}_envmap.tpf.dcx";
                env.AssetVirtualPath = $@"map/tex/{mid}/env";
                ads.Add(env);
            }

            return ads;
        }

        public List<string> GetEnvMapTextureNames(string mapid)
        {
            var l = new List<string>();
            if (Type == GameType.DarkSoulsIII)
            {
                var mid = mapid.Substring(0, 3);
                var t = TPF.Read($@"{GameRootDirectory}\map\{mid}\{mid}_envmap.tpf.dcx");
                foreach (var tex in t.Textures)
                {
                    l.Add(tex.Name);
                }
            }
            return l;
        }

        public AssetDescription GetChrTextures(string chrid)
        {
            AssetDescription ad = new AssetDescription();
            ad.AssetArchiveVirtualPath = null;
            ad.AssetPath = null;
            if (Type == GameType.DarkSoulsIII)
            {
                string path = $@"{GameRootDirectory}\chr\{chrid}.texbnd.dcx";
                if (File.Exists(path))
                {
                    ad.AssetPath = path;
                    ad.AssetArchiveVirtualPath = $@"chr/{chrid}/tex";
                }
            }
            if (Type == GameType.Bloodborne)
            {
                string path = $@"{GameRootDirectory}\chr\{chrid}_2.tpf.dcx";
                if (File.Exists(path))
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
            if (Type == GameType.DarkSoulsPTDE)
            {
                ret.AssetPath = $@"{GameRootDirectory}\map\{mapid}\{model}.nvm";
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

        public AssetDescription GetObjModel(string obj)
        {
            var ret = new AssetDescription();
            ret.AssetName = obj;
            ret.AssetArchiveVirtualPath = $@"obj/{obj}/model";
            if (Type == GameType.DarkSoulsIISOTFS)
            {
                ret.AssetVirtualPath = $@"obj/{obj}/model/{obj}.flv";
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
                            return $@"{GameRootDirectory}\model\map\t{mid.Substring(1)}.tpfbhd";
                        }
                    }
                    else
                    {
                        var mid = pathElements[i];
                        i++;
                        bndpath = "";
                        if (pathElements[i] == "env")
                        {
                            return $@"{GameRootDirectory}\map\{mid}\{mid}_envmap.tpf.dcx";
                        }
                        return $@"{GameRootDirectory}\map\{mid}\{mid}_{pathElements[i]}.tpfbhd";
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
                            return $@"{GameRootDirectory}\map\{mapid}\{pathElements[i]}.flver";
                        }
                        else if (Type == GameType.DarkSoulsIISOTFS)
                        {
                            return $@"{GameRootDirectory}\model\map\{mapid}.mapbhd";
                        }
                        else if (Type == GameType.Bloodborne)
                        {
                            return $@"{GameRootDirectory}\map\{mapid}\{pathElements[i]}.flver.dcx";
                        }
                        return $@"{GameRootDirectory}\map\{mapid}\{pathElements[i]}.mapbnd.dcx";
                    }
                    else if (pathElements[i].Equals("hit"))
                    {
                        i++;
                        var hittype = pathElements[i];
                        i++;
                        if (Type == GameType.DarkSoulsPTDE)
                        {
                            bndpath = "";
                            return $@"{GameRootDirectory}\map\{mapid}\{pathElements[i]}";
                        }
                        else if (Type == GameType.DarkSoulsIISOTFS)
                        {
                            bndpath = "";
                            return $@"{GameRootDirectory}\model\map\h{mapid.Substring(1)}.hkxbhd";
                        }
                        else if (Type == GameType.DarkSoulsIII || Type == GameType.Bloodborne)
                        {
                            bndpath = "";
                            if (hittype == "lo")
                            {
                                return $@"{GameRootDirectory}\map\{mapid}\l{mapid.Substring(1)}.hkxbhd";
                            }
                            return $@"{GameRootDirectory}\map\{mapid}\h{mapid.Substring(1)}.hkxbhd";
                        }
                        bndpath = "";
                        return null;
                    }
                    else if (pathElements[i].Equals("nav"))
                    {
                        i++;
                        if (Type == GameType.DarkSoulsPTDE)
                        {
                            if (i < pathElements.Length)
                            {
                                bndpath = $@"{pathElements[i]}";
                            }
                            else
                            {
                                bndpath = "";
                            }
                            return $@"{GameRootDirectory}\map\{mapid}\{mapid}.nvmbnd";
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
                        return $@"{GameRootDirectory}\chr\{chrid}.chrbnd";
                    }
                    else if (Type == GameType.DarkSoulsIISOTFS)
                    {
                        return $@"{GameRootDirectory}\model\chr\{chrid}.bnd";
                    }
                    return $@"{GameRootDirectory}\chr\{chrid}.chrbnd.dcx";
                }
                else if (pathElements[i].Equals("tex"))
                {
                    bndpath = "";
                    if (Type == GameType.DarkSoulsIII)
                    {
                        return $@"{GameRootDirectory}\chr\{chrid}.texbnd.dcx";
                    }
                    else if (Type == GameType.Bloodborne)
                    {
                        return $@"{GameRootDirectory}\chr\{chrid}_2.tpf.dcx";
                    }
                }
            }
            else if (pathElements[i].Equals("obj"))
            {
                i++;
                var chrid = pathElements[i];
                i++;
                if (pathElements[i].Equals("model"))
                {
                    bndpath = "";
                    if (Type == GameType.DarkSoulsPTDE)
                    {
                        return $@"{GameRootDirectory}\obj\{chrid}.objbnd";
                    }
                    else if (Type == GameType.DarkSoulsIISOTFS)
                    {
                        return $@"{GameRootDirectory}\model\obj\{chrid}.bnd";
                    }
                    return $@"{GameRootDirectory}\obj\{chrid}.objbnd.dcx";
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
