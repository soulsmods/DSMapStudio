using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbHandIkDriverInfoHand : IHavokObject
    {
        public Vector4 m_elbowAxisLS;
        public Vector4 m_backHandNormalLS;
        public Vector4 m_handOffsetLS;
        public Quaternion m_handOrienationOffsetLS;
        public float m_maxElbowAngleDegrees;
        public float m_minElbowAngleDegrees;
        public short m_shoulderIndex;
        public short m_shoulderSiblingIndex;
        public short m_elbowIndex;
        public short m_elbowSiblingIndex;
        public short m_wristIndex;
        public bool m_enforceEndPosition;
        public bool m_enforceEndRotation;
        public string m_localFrameName;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_elbowAxisLS = des.ReadVector4(br);
            m_backHandNormalLS = des.ReadVector4(br);
            m_handOffsetLS = des.ReadVector4(br);
            m_handOrienationOffsetLS = des.ReadQuaternion(br);
            m_maxElbowAngleDegrees = br.ReadSingle();
            m_minElbowAngleDegrees = br.ReadSingle();
            m_shoulderIndex = br.ReadInt16();
            m_shoulderSiblingIndex = br.ReadInt16();
            m_elbowIndex = br.ReadInt16();
            m_elbowSiblingIndex = br.ReadInt16();
            m_wristIndex = br.ReadInt16();
            m_enforceEndPosition = br.ReadBoolean();
            m_enforceEndRotation = br.ReadBoolean();
            br.AssertUInt32(0);
            m_localFrameName = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_maxElbowAngleDegrees);
            bw.WriteSingle(m_minElbowAngleDegrees);
            bw.WriteInt16(m_shoulderIndex);
            bw.WriteInt16(m_shoulderSiblingIndex);
            bw.WriteInt16(m_elbowIndex);
            bw.WriteInt16(m_elbowSiblingIndex);
            bw.WriteInt16(m_wristIndex);
            bw.WriteBoolean(m_enforceEndPosition);
            bw.WriteBoolean(m_enforceEndRotation);
            bw.WriteUInt32(0);
        }
    }
}
