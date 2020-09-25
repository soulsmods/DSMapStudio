using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBreakableShape : hkReferencedObject
    {
        public hkcdShape m_physicsShape;
        public hkpBreakableMaterial m_material;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_physicsShape = des.ReadClassPointer<hkcdShape>(br);
            m_material = des.ReadClassPointer<hkpBreakableMaterial>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
