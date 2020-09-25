using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBonePlanesSetupObjectGlobalPlane : IHavokObject
    {
        public string m_transformName;
        public hclVertexSelectionInput m_particles;
        public Vector4 m_planeEquationBoneSpace;
        public hclVertexFloatInput m_allowedPenetration;
        public hclVertexFloatInput m_stiffness;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transformName = des.ReadStringPointer(br);
            m_particles = new hclVertexSelectionInput();
            m_particles.Read(des, br);
            br.AssertUInt64(0);
            m_planeEquationBoneSpace = des.ReadVector4(br);
            m_allowedPenetration = new hclVertexFloatInput();
            m_allowedPenetration.Read(des, br);
            m_stiffness = new hclVertexFloatInput();
            m_stiffness.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_particles.Write(bw);
            bw.WriteUInt64(0);
            m_allowedPenetration.Write(bw);
            m_stiffness.Write(bw);
        }
    }
}
