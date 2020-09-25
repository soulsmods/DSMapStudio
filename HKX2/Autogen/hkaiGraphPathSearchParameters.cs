using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiGraphPathSearchParameters : IHavokObject
    {
        public float m_heuristicWeight;
        public bool m_useHierarchicalHeuristic;
        public hkaiSearchParametersBufferSizes m_bufferSizes;
        public hkaiSearchParametersBufferSizes m_hierarchyBufferSizes;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_heuristicWeight = br.ReadSingle();
            m_useHierarchicalHeuristic = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_bufferSizes = new hkaiSearchParametersBufferSizes();
            m_bufferSizes.Read(des, br);
            m_hierarchyBufferSizes = new hkaiSearchParametersBufferSizes();
            m_hierarchyBufferSizes.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_heuristicWeight);
            bw.WriteBoolean(m_useHierarchicalHeuristic);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            m_bufferSizes.Write(bw);
            m_hierarchyBufferSizes.Write(bw);
        }
    }
}
