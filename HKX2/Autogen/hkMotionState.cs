using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMotionState : IHavokObject
    {
        public Matrix4x4 m_transform;
        public Vector4 m_sweptTransform;
        public Vector4 m_deltaAngle;
        public float m_objectRadius;
        public short m_linearDamping;
        public short m_angularDamping;
        public short m_timeFactor;
        public hkUFloat8 m_maxLinearVelocity;
        public hkUFloat8 m_maxAngularVelocity;
        public byte m_deactivationClass;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transform = des.ReadTransform(br);
            m_sweptTransform = des.ReadVector4(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_deltaAngle = des.ReadVector4(br);
            m_objectRadius = br.ReadSingle();
            m_linearDamping = br.ReadInt16();
            m_angularDamping = br.ReadInt16();
            m_timeFactor = br.ReadInt16();
            m_maxLinearVelocity = new hkUFloat8();
            m_maxLinearVelocity.Read(des, br);
            m_maxAngularVelocity = new hkUFloat8();
            m_maxAngularVelocity.Read(des, br);
            m_deactivationClass = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_objectRadius);
            bw.WriteInt16(m_linearDamping);
            bw.WriteInt16(m_angularDamping);
            bw.WriteInt16(m_timeFactor);
            m_maxLinearVelocity.Write(bw);
            m_maxAngularVelocity.Write(bw);
            bw.WriteByte(m_deactivationClass);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
