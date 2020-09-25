using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiStaticTreeNavMeshQueryMediator : hkaiNavMeshQueryMediator
    {
        public hkcdStaticAabbTree m_tree;
        public hkaiNavMesh m_navMesh;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_tree = des.ReadClassPointer<hkcdStaticAabbTree>(br);
            m_navMesh = des.ReadClassPointer<hkaiNavMesh>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
