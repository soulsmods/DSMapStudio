using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdPlanarGeometry : hkcdPlanarEntity
    {
        public hkcdPlanarGeometryPlanesCollection m_planes;
        public hkcdPlanarGeometryPolygonCollection m_polys;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_planes = des.ReadClassPointer<hkcdPlanarGeometryPlanesCollection>(br);
            m_polys = des.ReadClassPointer<hkcdPlanarGeometryPolygonCollection>(br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
