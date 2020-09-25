using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiCuttingGeometryInfo : hkReferencedObject
    {
        public hkGeometry m_geometry;
        public hkBitField m_cuttingTriangles;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_geometry = new hkGeometry();
            m_geometry.Read(des, br);
            m_cuttingTriangles = new hkBitField();
            m_cuttingTriangles.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_geometry.Write(bw);
            m_cuttingTriangles.Write(bw);
        }
    }
}
