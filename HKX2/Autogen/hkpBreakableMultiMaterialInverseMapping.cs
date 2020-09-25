using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBreakableMultiMaterialInverseMapping : hkReferencedObject
    {
        public List<hkpBreakableMultiMaterialInverseMappingDescriptor> m_descriptors;
        public List<uint> m_subShapeIds;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_descriptors = des.ReadClassArray<hkpBreakableMultiMaterialInverseMappingDescriptor>(br);
            m_subShapeIds = des.ReadUInt32Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
