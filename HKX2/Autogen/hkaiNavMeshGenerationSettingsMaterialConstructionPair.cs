using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiNavMeshGenerationSettingsMaterialConstructionPair : IHavokObject
    {
        public int m_materialIndex;
        public uint m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_materialIndex = br.ReadInt32();
            m_flags = br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_materialIndex);
        }
    }
}
