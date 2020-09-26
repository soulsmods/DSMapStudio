using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdPlanarCsgOperand : hkReferencedObject
    {
        public hkcdPlanarGeometry m_geometry;
        public hkcdPlanarGeometry m_danglingGeometry;
        public hkcdPlanarSolid m_solid;
        public List<hkcdPlanarCsgOperandGeomSource> m_geomSources;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_geometry = des.ReadClassPointer<hkcdPlanarGeometry>(br);
            m_danglingGeometry = des.ReadClassPointer<hkcdPlanarGeometry>(br);
            m_solid = des.ReadClassPointer<hkcdPlanarSolid>(br);
            m_geomSources = des.ReadClassArray<hkcdPlanarCsgOperandGeomSource>(br);
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
