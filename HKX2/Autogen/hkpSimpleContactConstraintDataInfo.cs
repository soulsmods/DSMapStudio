using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSimpleContactConstraintDataInfo : IHavokObject
    {
        public ushort m_flags;
        public ushort m_biNormalAxis;
        public short m_rollingFrictionMultiplier;
        public short m_internalData1;
        public short m_rhsRolling;
        public float m_contactRadius;
        public float m_data;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_flags = br.ReadUInt16();
            m_biNormalAxis = br.ReadUInt16();
            m_rollingFrictionMultiplier = br.ReadInt16();
            m_internalData1 = br.ReadInt16();
            m_rhsRolling = br.ReadInt16();
            br.AssertUInt16(0);
            m_contactRadius = br.ReadSingle();
            m_data = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_flags);
            bw.WriteUInt16(m_biNormalAxis);
            bw.WriteInt16(m_rollingFrictionMultiplier);
            bw.WriteInt16(m_internalData1);
            bw.WriteInt16(m_rhsRolling);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_contactRadius);
            bw.WriteSingle(m_data);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
