using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticMeshTreeBasePrimitiveDataRunBaseunsignedint : IHavokObject
    {
        public uint m_value;
        public byte m_index;
        public byte m_count;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_value = br.ReadUInt32();
            m_index = br.ReadByte();
            m_count = br.ReadByte();
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_value);
            bw.WriteByte(m_index);
            bw.WriteByte(m_count);
            bw.WriteUInt16(0);
        }
    }
}
