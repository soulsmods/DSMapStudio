using SoulsFormats;
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
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_geometry = des.ReadClassPointer<hknpExternMeshShapeGeometry>(br);
            m_boundingVolumeData = des.ReadClassPointer<hknpExternMeshShapeData>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
