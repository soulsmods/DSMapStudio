using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMeshBoneIndexMapping : IHavokObject
    {
        public List<short> m_mapping;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_mapping = des.ReadInt16Array(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
