using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    public class MtdBank
    {
        private static AssetLocator AssetLocator = null;

        private static Dictionary<string, MTD> _mtds = null;

        public static IReadOnlyDictionary<string, MTD> Mtds
        {
            get
            {
                return _mtds;
            }
        }

        public static void ReloadMtds()
        {
            IBinder mtdBinder = null;
            if (AssetLocator.Type == GameType.DarkSoulsIII)
            {
                mtdBinder = BND4.Read($@"{AssetLocator.GameRootDirectory}\mtd\allmaterialbnd.mtdbnd.dcx");
            }

            if (mtdBinder == null)
            {
                return;
            }

            _mtds = new Dictionary<string, MTD>();
            foreach (var f in mtdBinder.Files)
            {
                _mtds.Add(Path.GetFileNameWithoutExtension(f.Name), MTD.Read(f.Bytes));
            }
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
