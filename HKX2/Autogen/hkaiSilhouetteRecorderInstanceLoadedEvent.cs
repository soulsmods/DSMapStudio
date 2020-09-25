using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiSilhouetteRecorderInstanceLoadedEvent : hkaiSilhouetteRecorderReplayEvent
    {
        public hkaiNavMeshInstance m_instance;
        public hkaiNavMeshQueryMediator m_mediator;
        public hkaiDirectedGraphInstance m_graph;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_instance = des.ReadClassPointer<hkaiNavMeshInstance>(br);
            m_mediator = des.ReadClassPointer<hkaiNavMeshQueryMediator>(br);
            m_graph = des.ReadClassPointer<hkaiDirectedGraphInstance>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
        }
    }
}
