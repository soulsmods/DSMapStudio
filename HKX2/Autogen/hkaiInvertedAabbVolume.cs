using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiInvertedAabbVolume : hkaiVolume
    {
        public hkAabb m_aabb;
        public hkGeometry m_geometry;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_aabb = new hkAabb();
            m_aabb.Read(des, br);
            m_geometry = new hkGeometry();
            m_geometry.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_aabb.Write(bw);
            m_geometry.Write(bw);
        }
    }
}
