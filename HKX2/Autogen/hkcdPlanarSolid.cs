using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum NodeTypes
    {
        NODE_TYPE_INTERNAL = 0,
        NODE_TYPE_IN = 1,
        NODE_TYPE_OUT = 2,
        NODE_TYPE_UNKNOWN = 3,
        NODE_TYPE_INVALID = 4,
        NODE_TYPE_FREE = 15,
    }
    
    public class hkcdPlanarSolid : hkcdPlanarEntity
    {
        public hkcdPlanarSolidNodeStorage m_nodes;
        public hkcdPlanarGeometryPlanesCollection m_planes;
        public uint m_rootNodeId;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nodes = des.ReadClassPointer<hkcdPlanarSolidNodeStorage>(br);
            m_planes = des.ReadClassPointer<hkcdPlanarGeometryPlanesCollection>(br);
            m_rootNodeId = br.ReadUInt32();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteUInt32(m_rootNodeId);
            bw.WriteUInt32(0);
        }
    }
}
