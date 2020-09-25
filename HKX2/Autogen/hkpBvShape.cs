using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBvShape : hkpShape
    {
        public hkpShape m_boundingVolumeShape;
        public hkpSingleShapeContainer m_childShape;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_boundingVolumeShape = des.ReadClassPointer<hkpShape>(br);
            m_childShape = new hkpSingleShapeContainer();
            m_childShape.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            m_childShape.Write(bw);
        }
    }
}
