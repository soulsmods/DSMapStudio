using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiPathfindingUtilFindPathInput : hkReferencedObject
    {
        public Vector4 m_startPoint;
        public List<Vector4> m_goalPoints;
        public uint m_startFaceKey;
        public List<uint> m_goalFaceKeys;
        public int m_maxNumberOfIterations;
        public hkaiAgentTraversalInfo m_agentInfo;
        public hkaiNavMeshPathSearchParameters m_searchParameters;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_startPoint = des.ReadVector4(br);
            m_goalPoints = des.ReadVector4Array(br);
            m_startFaceKey = br.ReadUInt32();
            br.AssertUInt32(0);
            m_goalFaceKeys = des.ReadUInt32Array(br);
            m_maxNumberOfIterations = br.ReadInt32();
            m_agentInfo = new hkaiAgentTraversalInfo();
            m_agentInfo.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_searchParameters = new hkaiNavMeshPathSearchParameters();
            m_searchParameters.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_startFaceKey);
            bw.WriteUInt32(0);
            bw.WriteInt32(m_maxNumberOfIterations);
            m_agentInfo.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            m_searchParameters.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
