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
    }
}
