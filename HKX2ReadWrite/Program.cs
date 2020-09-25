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
                des.Deserialize(br);
            }
        }
    }
}
