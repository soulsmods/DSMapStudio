using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiStreamingSet : IHavokObject
    {
        public uint m_thisUid;
        public uint m_oppositeUid;
        public List<hkaiStreamingSetNavMeshConnection> m_meshConnections;
        public List<hkaiStreamingSetGraphConnection> m_graphConnections;
        public List<hkaiStreamingSetVolumeConnection> m_volumeConnections;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_thisUid = br.ReadUInt32();
            m_oppositeUid = br.ReadUInt32();
            m_meshConnections = des.ReadClassArray<hkaiStreamingSetNavMeshConnection>(br);
            m_graphConnections = des.ReadClassArray<hkaiStreamingSetGraphConnection>(br);
            m_volumeConnections = des.ReadClassArray<hkaiStreamingSetVolumeConnection>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_thisUid);
            bw.WriteUInt32(m_oppositeUid);
        }
    }
}
