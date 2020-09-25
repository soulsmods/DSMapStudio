using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpMultiRayShape : hkpShape
    {
        public List<hkpMultiRayShapeRay> m_rays;
        public float m_rayPenetrationDistance;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rays = des.ReadClassArray<hkpMultiRayShapeRay>(br);
            m_rayPenetrationDistance = br.ReadSingle();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_rayPenetrationDistance);
            bw.WriteUInt32(0);
        }
    }
}
