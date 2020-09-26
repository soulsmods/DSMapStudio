using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPairCollisionFilterMapPairFilterKeyOverrideType : IHavokObject
    {
        public int m_numElems;
        public int m_hashMod;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.ReadUInt64();
            m_numElems = br.ReadInt32();
            m_hashMod = br.ReadInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteInt32(m_numElems);
            bw.WriteInt32(m_hashMod);
        }
    }
}
