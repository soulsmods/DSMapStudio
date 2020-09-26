using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiPersistentFaceKey : IHavokObject
    {
        public uint m_key;
        public short m_offset;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_key = br.ReadUInt32();
            m_offset = br.ReadInt16();
            br.ReadUInt16();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_key);
            bw.WriteInt16(m_offset);
            bw.WriteUInt16(0);
        }
    }
}
