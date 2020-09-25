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
        
        public byte m_indices;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_indices = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_indices);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
