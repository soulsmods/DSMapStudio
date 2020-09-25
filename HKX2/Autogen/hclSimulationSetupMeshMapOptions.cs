using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimulationSetupMeshMapOptions : IHavokObject
    {
        public bool m_collapseVertices;
        public float m_collapseThreshold;
        public hclVertexSelectionInput m_vertexSelection;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_collapseVertices = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_collapseThreshold = br.ReadSingle();
            m_vertexSelection = new hclVertexSelectionInput();
            m_vertexSelection.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteBoolean(m_collapseVertices);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_collapseThreshold);
            m_vertexSelection.Write(bw);
        }
    }
}
