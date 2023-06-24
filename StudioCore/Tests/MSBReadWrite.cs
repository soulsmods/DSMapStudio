using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SoulsFormats;

namespace StudioCore.Tests
{
    public static class MSBReadWrite
    {
        public static bool Run(AssetLocator locator)
        {
            var msbs = locator.GetFullMapList();
            foreach (var msb in msbs)
            {
                var path = locator.GetMapMSB(msb);
                var bytes = File.ReadAllBytes(path.AssetPath);
                var decompressed = DCX.Decompress(bytes);
                MSBE m = MSBE.Read(decompressed);
                var written = m.Write(DCX.Type.None);
                if (!decompressed.Span.SequenceEqual(written))
                {
                    var basepath = Path.GetDirectoryName(path.AssetPath);
                    if (!Directory.Exists($@"{basepath}\mismatches"))
                    {
                        Directory.CreateDirectory($@"{basepath}\mismatches");
                    }
                    Console.WriteLine($@"Mismatch: {msb}");
                    File.WriteAllBytes($@"{basepath}\mismatches\{Path.GetFileNameWithoutExtension(path.AssetPath)}", written);
                }
            }
            return true;
        }
    }
}
