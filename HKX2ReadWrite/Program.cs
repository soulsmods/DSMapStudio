using SoulsFormats;
using System;
using System.IO;
using HKX2;

namespace HKX2ReadWrite
{
    class Program
    {
        static void Main(string[] args)
        {
            var hkxpath = args[0];
            using (FileStream stream = File.OpenRead(hkxpath))
            {
                BinaryReaderEx br = new BinaryReaderEx(false, stream);
                var des = new HKX2.PackFileDeserializer();
                var root = des.Deserialize(br);

                using (FileStream s2 = File.Create(hkxpath + ".out"))
                {
                    BinaryWriterEx bw = new BinaryWriterEx(false, s2);
                    var s = new HKX2.PackFileSerializer();
                    s.Serialize(root, bw);
                }
            }
        }
    }
}
