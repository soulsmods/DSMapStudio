using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbKeyframeBonesModifierKeyframeInfo : IHavokObject
    {
        public Vector4 m_keyframedPosition;
        public Quaternion m_keyframedRotation;
        public short m_boneIndex;
        public bool m_isValid;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_keyframedPosition = des.ReadVector4(br);
            m_keyframedRotation = des.ReadQuaternion(br);
            m_boneIndex = br.ReadInt16();
            m_isValid = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt16(m_boneIndex);
            bw.WriteBoolean(m_isValid);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
