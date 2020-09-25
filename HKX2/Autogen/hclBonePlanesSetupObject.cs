using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBonePlanesSetupObject : hclConstraintSetSetupObject
    {
        public string m_name;
        public hclSimulationSetupMesh m_simulationMesh;
        public hclTransformSetSetupObject m_transformSetSetup;
        public List<hclBonePlanesSetupObjectPerParticlePlane> m_perParticlePlanes;
        public List<hclBonePlanesSetupObjectGlobalPlane> m_globalPlanes;
        public List<hclBonePlanesSetupObjectPerParticleAngle> m_perParticleAngle;
        public bool m_angleSpecifiedInDegrees;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_simulationMesh = des.ReadClassPointer<hclSimulationSetupMesh>(br);
            m_transformSetSetup = des.ReadClassPointer<hclTransformSetSetupObject>(br);
            m_perParticlePlanes = des.ReadClassArray<hclBonePlanesSetupObjectPerParticlePlane>(br);
            m_globalPlanes = des.ReadClassArray<hclBonePlanesSetupObjectGlobalPlane>(br);
            m_perParticleAngle = des.ReadClassArray<hclBonePlanesSetupObjectPerParticleAngle>(br);
            m_angleSpecifiedInDegrees = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteBoolean(m_angleSpecifiedInDegrees);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
