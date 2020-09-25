using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiStreamingCollectionInstanceInfo : IHavokObject
    {
        public hkaiNavMeshInstance m_instancePtr;
        public hkaiNavVolumeInstance m_volumeInstancePtr;
        public hkaiDirectedGraphInstance m_clusterGraphInstance;
        public hkaiNavMeshQueryMediator m_mediator;
        public hkaiNavVolumeMediator m_volumeMediator;
        public uint m_treeNode;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_instancePtr = des.ReadClassPointer<hkaiNavMeshInstance>(br);
            m_volumeInstancePtr = des.ReadClassPointer<hkaiNavVolumeInstance>(br);
            m_clusterGraphInstance = des.ReadClassPointer<hkaiDirectedGraphInstance>(br);
            m_mediator = des.ReadClassPointer<hkaiNavMeshQueryMediator>(br);
            m_volumeMediator = des.ReadClassPointer<hkaiNavVolumeMediator>(br);
            m_treeNode = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteUInt32(m_treeNode);
            bw.WriteUInt32(0);
        }
    }
}
