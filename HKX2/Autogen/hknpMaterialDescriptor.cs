using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMaterialDescriptor : IHavokObject
    {
        public string m_name;
        public hknpRefMaterial m_material;
        public ushort m_materialId;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_material = des.ReadClassPointer<hknpRefMaterial>(br);
            m_materialId = br.ReadUInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt16(m_materialId);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
