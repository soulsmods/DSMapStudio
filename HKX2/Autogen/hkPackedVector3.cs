using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkPackedVector3 : IHavokObject
    {
        public short m_values;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_values = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt16(m_values);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
