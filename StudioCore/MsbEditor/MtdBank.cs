﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SoulsFormats;
using StudioCore.Editor;

namespace StudioCore.MsbEditor
{
    public class MtdBank
    {
        private static AssetLocator AssetLocator = null;

        private static Dictionary<string, MTD> _mtds = null;
        private static Dictionary<string, MATBIN> _matbins = null;

        public static bool IsMatbin { get; private set; }

        public static IReadOnlyDictionary<string, MTD> Mtds
        {
            get
            {
                return _mtds;
            }
        }

        public static IReadOnlyDictionary<string, MATBIN> Matbins
        {
            get
            {
                return _matbins;
            }
        }

        public static void ReloadMtds()
        {

            TaskManager.Run("MB:LoadMtds", true, false, false, () =>
            {
                try
                {
                    IBinder mtdBinder = null;
                    if (AssetLocator.Type == GameType.DarkSoulsIII || AssetLocator.Type == GameType.Sekiro)
                    {
                        mtdBinder = BND4.Read(AssetLocator.GetAssetPath($@"mtd\allmaterialbnd.mtdbnd.dcx"));
                        IsMatbin = false;
                    }
                    else if (AssetLocator.Type == GameType.EldenRing)
                    {
                        mtdBinder = BND4.Read(AssetLocator.GetAssetPath($@"material\allmaterial.matbinbnd.dcx"));
                        IsMatbin = true;
                    }

                    if (mtdBinder == null)
                    {
                        return;
                    }

                    if (IsMatbin)
                    {
                        _matbins = new Dictionary<string, MATBIN>();
                        foreach (var f in mtdBinder.Files)
                        {
                            var matname = Path.GetFileNameWithoutExtension(f.Name);
                            // Because *certain* mods contain duplicate entries for the same material
                            if (!_matbins.ContainsKey(matname))
                            {
                                _matbins.Add(matname, MATBIN.Read(f.Bytes));
                            }
                        }
                    }
                    else
                    {
                        _mtds = new Dictionary<string, MTD>();
                        foreach (var f in mtdBinder.Files)
                        {
                            var mtdname = Path.GetFileNameWithoutExtension(f.Name);
                            // Because *certain* mods contain duplicate entries for the same material
                            if (!_mtds.ContainsKey(mtdname))
                            {
                                _mtds.Add(mtdname, MTD.Read(f.Bytes));
                            }
                        }
                    }
                }
                catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
                {
                    _mtds = new Dictionary<string, MTD>();
                    _matbins = new Dictionary<string, MATBIN>();
                }
            });
        }

        public static void LoadMtds(AssetLocator l)
        {
            AssetLocator = l;

            if (AssetLocator.Type == GameType.Undefined)
            {
                return;
            }

            ReloadMtds();
        }
    }
}
