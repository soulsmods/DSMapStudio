using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkFreeListArrayhknpMaterialhknpMaterialId8hknpMaterialFreeListArrayOperations : IHavokObject
    {
        public List<hknpMaterial> m_elements;
        public int m_firstFree;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_elements = des.ReadClassArray<hknpMaterial>(br);
            m_firstFree = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_firstFree);
            bw.WriteUInt32(0);
        }
    }
}
