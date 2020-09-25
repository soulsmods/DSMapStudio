using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkRefCountedPropertiesEntry : IHavokObject
    {
        public hkReferencedObject m_object;
        public ushort m_key;
        public ushort m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_object = des.ReadClassPointer<hkReferencedObject>(br);
            m_key = br.ReadUInt16();
            m_flags = br.ReadUInt16();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt16(m_key);
            bw.WriteUInt16(m_flags);
            bw.WriteUInt32(0);
        }
    }
}
