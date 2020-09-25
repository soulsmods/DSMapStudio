using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiPhysics2012ShapeVolume : hkaiVolume
    {
        public hkpShape m_shape;
        public Matrix4x4 m_shapeTransform;
        public hkGeometry m_geometry;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_shapeTransform = des.ReadTransform(br);
            m_geometry = new hkGeometry();
            m_geometry.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
            m_geometry.Write(bw);
        }
    }
}
