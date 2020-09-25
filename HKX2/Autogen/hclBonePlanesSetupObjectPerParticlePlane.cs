using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBonePlanesSetupObjectPerParticlePlane : IHavokObject
    {
        public string m_transformName;
        public hclVertexSelectionInput m_particles;
        public Vector4 m_directionBoneSpace;
        public hclVertexFloatInput m_allowedDistance;
        public hclVertexFloatInput m_stiffness;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transformName = des.ReadStringPointer(br);
            m_particles = new hclVertexSelectionInput();
            m_particles.Read(des, br);
            br.AssertUInt64(0);
            m_directionBoneSpace = des.ReadVector4(br);
            m_allowedDistance = new hclVertexFloatInput();
            m_allowedDistance.Read(des, br);
            m_stiffness = new hclVertexFloatInput();
            m_stiffness.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_particles.Write(bw);
            bw.WriteUInt64(0);
            m_allowedDistance.Write(bw);
            m_stiffness.Write(bw);
        }
    }
}
