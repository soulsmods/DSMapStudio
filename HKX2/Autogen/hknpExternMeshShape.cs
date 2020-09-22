using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum RebuildFlagsEnum
    {
        REBUILD_USE_DOUBLE_BUFFERING = 1,
        REBUILD_REFIT_ONLY = 2,
    }
    
    public class hknpExternMeshShape : hknpCompositeShape
    {
        public hknpExternMeshShapeGeometry m_geometry;
        public hknpExternMeshShapeData m_boundingVolumeData;
    }
}
