using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiPathfindingUtilFindGraphPathInput : IHavokObject
    {
        public List<uint> m_startNodeKeys;
        public List<float> m_initialCosts;
        public List<uint> m_goalNodeKeys;
        public List<float> m_finalCosts;
        public int m_maxNumberOfIterations;
        public hkaiAgentTraversalInfo m_agentInfo;
        public hkaiGraphPathSearchParameters m_searchParameters;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_startNodeKeys = des.ReadUInt32Array(br);
            m_initialCosts = des.ReadSingleArray(br);
            m_goalNodeKeys = des.ReadUInt32Array(br);
            m_finalCosts = des.ReadSingleArray(br);
            m_maxNumberOfIterations = br.ReadInt32();
            m_agentInfo = new hkaiAgentTraversalInfo();
            m_agentInfo.Read(des, br);
            br.AssertUInt32(0);
            m_searchParameters = new hkaiGraphPathSearchParameters();
            m_searchParameters.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_maxNumberOfIterations);
            m_agentInfo.Write(bw);
            bw.WriteUInt32(0);
            m_searchParameters.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
