using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBreakableMultiMaterial : hkpBreakableMaterial
    {
        public List<hkpBreakableMaterial> m_subMaterials;
        public hkpBreakableMultiMaterialInverseMapping m_inverseMapping;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_subMaterials = des.ReadClassPointerArray<hkpBreakableMaterial>(br);
            m_inverseMapping = des.ReadClassPointer<hkpBreakableMultiMaterialInverseMapping>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
