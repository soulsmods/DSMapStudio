using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclMeshMeshDeformSetupObject : hclOperatorSetupObject
    {
        public string m_name;
        public hclBufferSetupObject m_inputBufferSetupObject;
        public hclTriangleSelectionInput m_inputTriangleSelection;
        public hclBufferSetupObject m_outputBufferSetupObject;
        public hclVertexSelectionInput m_outputVertexSelection;
        public hclVertexFloatInput m_influenceRadiusPerVertex;
        public ScaleNormalBehaviour m_scaleNormalBehaviour;
        public uint m_maxTrianglesPerVertex;
        public float m_minimumTriangleWeight;
        public bool m_deformNormals;
        public bool m_deformTangents;
        public bool m_deformBiTangents;
        public bool m_useMeshTopology;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_inputBufferSetupObject = des.ReadClassPointer<hclBufferSetupObject>(br);
            m_inputTriangleSelection = new hclTriangleSelectionInput();
            m_inputTriangleSelection.Read(des, br);
            m_outputBufferSetupObject = des.ReadClassPointer<hclBufferSetupObject>(br);
            m_outputVertexSelection = new hclVertexSelectionInput();
            m_outputVertexSelection.Read(des, br);
            m_influenceRadiusPerVertex = new hclVertexFloatInput();
            m_influenceRadiusPerVertex.Read(des, br);
            m_scaleNormalBehaviour = (ScaleNormalBehaviour)br.ReadUInt32();
            m_maxTrianglesPerVertex = br.ReadUInt32();
            m_minimumTriangleWeight = br.ReadSingle();
            m_deformNormals = br.ReadBoolean();
            m_deformTangents = br.ReadBoolean();
            m_deformBiTangents = br.ReadBoolean();
            m_useMeshTopology = br.ReadBoolean();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            m_inputTriangleSelection.Write(bw);
            // Implement Write
            m_outputVertexSelection.Write(bw);
            m_influenceRadiusPerVertex.Write(bw);
            bw.WriteUInt32(m_maxTrianglesPerVertex);
            bw.WriteSingle(m_minimumTriangleWeight);
            bw.WriteBoolean(m_deformNormals);
            bw.WriteBoolean(m_deformTangents);
            bw.WriteBoolean(m_deformBiTangents);
            bw.WriteBoolean(m_useMeshTopology);
        }
    }
}
