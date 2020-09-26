using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkColor : IHavokObject
    {
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(0);
        }
    }
}
