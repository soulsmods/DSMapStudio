using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclVolumeConstraintSetupObject : hclConstraintSetSetupObject
    {
        public string m_name;
        public hclSimulationSetupMesh m_simulationMesh;
        public hclVertexSelectionInput m_applyToParticles;
        public hclVertexFloatInput m_stiffness;
        public hclVertexSelectionInput m_influenceParticles;
        public hclVertexFloatInput m_particleWeights;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_simulationMesh = des.ReadClassPointer<hclSimulationSetupMesh>(br);
            m_applyToParticles = new hclVertexSelectionInput();
            m_applyToParticles.Read(des, br);
            m_stiffness = new hclVertexFloatInput();
            m_stiffness.Read(des, br);
            m_influenceParticles = new hclVertexSelectionInput();
            m_influenceParticles.Read(des, br);
            m_particleWeights = new hclVertexFloatInput();
            m_particleWeights.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            m_applyToParticles.Write(bw);
            m_stiffness.Write(bw);
            m_influenceParticles.Write(bw);
            m_particleWeights.Write(bw);
        }
    }
}
