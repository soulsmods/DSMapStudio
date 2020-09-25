using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkPackedVector8 : IHavokObject
    {
        public sbyte m_values;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_values = br.ReadSByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSByte(m_values);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
