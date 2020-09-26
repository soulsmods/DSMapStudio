using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticMeshTreeBasePrimitive : IHavokObject
    {
        public enum Type
        {
            INVALID = 0,
            TRIANGLE = 1,
            QUAD = 2,
            CUSTOM = 3,
            NUM_TYPES = 4,
        }
        
        public byte m_indices_0;
        public byte m_indices_1;
        public byte m_indices_2;
        public byte m_indices_3;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_indices_0 = br.ReadByte();
            m_indices_1 = br.ReadByte();
            m_indices_2 = br.ReadByte();
            m_indices_3 = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_indices_0);
            bw.WriteByte(m_indices_1);
            bw.WriteByte(m_indices_2);
            bw.WriteByte(m_indices_3);
        }
    }
}
