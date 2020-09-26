using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBonePlanesConstraintSetBonePlane : IHavokObject
    {
        public Vector4 m_planeEquationBone;
        public ushort m_particleIndex;
        public ushort m_transformIndex;
        public float m_stiffness;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_planeEquationBone = des.ReadVector4(br);
            m_particleIndex = br.ReadUInt16();
            m_transformIndex = br.ReadUInt16();
            m_stiffness = br.ReadSingle();
            br.ReadUInt64();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_particleIndex);
            bw.WriteUInt16(m_transformIndex);
            bw.WriteSingle(m_stiffness);
            bw.WriteUInt64(0);
        }
    }
}
