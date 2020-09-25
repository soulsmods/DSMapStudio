using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStretchLinkSetupObject : hclConstraintSetSetupObject
    {
        public string m_name;
        public hclSimulationSetupMesh m_simulationMesh;
        public hclVertexSelectionInput m_movableParticlesSelection;
        public hclVertexSelectionInput m_fixedParticlesSelection;
        public hclVertexFloatInput m_rigidFactor;
        public hclVertexFloatInput m_stiffness;
        public Vector4 m_stretchDirection;
        public bool m_useStretchDirection;
        public bool m_useMeshTopology;
        public bool m_allowDynamicLinks;
        public bool m_useTopologicalStretchDistance;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_simulationMesh = des.ReadClassPointer<hclSimulationSetupMesh>(br);
            m_movableParticlesSelection = new hclVertexSelectionInput();
            m_movableParticlesSelection.Read(des, br);
            m_fixedParticlesSelection = new hclVertexSelectionInput();
            m_fixedParticlesSelection.Read(des, br);
            m_rigidFactor = new hclVertexFloatInput();
            m_rigidFactor.Read(des, br);
            m_stiffness = new hclVertexFloatInput();
            m_stiffness.Read(des, br);
            m_stretchDirection = des.ReadVector4(br);
            m_useStretchDirection = br.ReadBoolean();
            m_useMeshTopology = br.ReadBoolean();
            m_allowDynamicLinks = br.ReadBoolean();
            m_useTopologicalStretchDistance = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            m_movableParticlesSelection.Write(bw);
            m_fixedParticlesSelection.Write(bw);
            m_rigidFactor.Write(bw);
            m_stiffness.Write(bw);
            bw.WriteBoolean(m_useStretchDirection);
            bw.WriteBoolean(m_useMeshTopology);
            bw.WriteBoolean(m_allowDynamicLinks);
            bw.WriteBoolean(m_useTopologicalStretchDistance);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
