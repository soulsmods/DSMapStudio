using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiHierarchyUtilsClusterSettings : IHavokObject
    {
        public int m_desiredFacesPerCluster;
        public hkaiNavMeshPathSearchParameters m_searchParameters;
        public hkaiAgentTraversalInfo m_agentInfo;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_desiredFacesPerCluster = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_searchParameters = new hkaiNavMeshPathSearchParameters();
            m_searchParameters.Read(des, br);
            m_agentInfo = new hkaiAgentTraversalInfo();
            m_agentInfo.Read(des, br);
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_desiredFacesPerCluster);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            m_searchParameters.Write(bw);
            m_agentInfo.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
