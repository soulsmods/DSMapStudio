using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiDynamicNavMeshQueryMediator : hkaiNavMeshQueryMediator
    {
        public hkaiStreamingCollection m_collection;
        public hkcdDynamicAabbTree m_aabbTree;
        public hkaiNavMeshCutter m_cutter;
        public float m_cutAabbTolerance;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_collection = des.ReadClassPointer<hkaiStreamingCollection>(br);
            m_aabbTree = des.ReadClassPointer<hkcdDynamicAabbTree>(br);
            m_cutter = des.ReadClassPointer<hkaiNavMeshCutter>(br);
            m_cutAabbTolerance = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteSingle(m_cutAabbTolerance);
            bw.WriteUInt32(0);
        }
    }
}
