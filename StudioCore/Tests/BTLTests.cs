using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SoulsFormats;
using System.Diagnostics;

namespace StudioCore.Tests
{
    public static class BTLReadWrite
    {
        public static bool Run(AssetLocator locator)
        {
            var msbs = locator.GetFullMapList();
            List<string> floats = new();
            List<string> noWrite = new();
            List<string> ver = new();
            foreach (var msb in msbs)
            {
                var btls = locator.GetMapBTLs(msb);

                foreach (var file in btls)
                {
                    BTL btl;
                    /*
                    if (locator.Type == GameType.DarkSoulsIISOTFS)
                    {
                        var bdt = BXF4.Read(file.AssetPath, file.AssetPath[..^3] + "bdt");
                        var bdtFile = bdt.Files.Find(f => f.Name.EndsWith("light.btl.dcx"));
                        if (bdtFile == null)
                        {
                            continue;
                        }
                        btl = BTL.Read(bdtFile.Bytes);
                    }
                    else
                    {
                        btl = BTL.Read(file.AssetPath);
                    }

                    foreach (var l in btl.Lights)
                    {
                        floats.Add(l.Rotation.Z.ToString());
                    }
                    ver.Add(btl.Version.ToString());
                    */

                    var bytes = File.ReadAllBytes(file.AssetPath);
                    var decompressed = DCX.Decompress(bytes);

                    btl = BTL.Read(decompressed);

                    var written = btl.Write(DCX.Type.None);
                    if (!decompressed.SequenceEqual(written))
                    {
                        noWrite.Add(file.AssetName);

                        var basepath = "Tests";
                        if (!Directory.Exists($@"{basepath}\mismatches"))
                        {
                            Directory.CreateDirectory($@"{basepath}\mismatches");
                        }
                        Debug.WriteLine($"Mismatch: {file.AssetName}");
                        File.WriteAllBytes($@"Tests\\mismatches\{Path.GetFileNameWithoutExtension(file.AssetName)}", written);
                    }
                }
            }
            var floatsD = floats.Distinct();
            var noWriteD = noWrite.Distinct();
            var verD = ver.Distinct();

            File.WriteAllLines("Tests\\BTL Zrot.txt", floatsD);
            File.WriteAllLines("Tests\\BTL Write Failure.txt", noWriteD);
            File.WriteAllLines("Tests\\BTL versions.txt", verD);

            return true;
        }
    }
}
