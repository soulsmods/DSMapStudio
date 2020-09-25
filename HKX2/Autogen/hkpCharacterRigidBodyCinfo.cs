using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCharacterRigidBodyCinfo : hkpCharacterControllerCinfo
    {
        public uint m_collisionFilterInfo;
        public hkpShape m_shape;
        public Vector4 m_position;
        public Quaternion m_rotation;
        public float m_mass;
        public float m_friction;
        public float m_maxLinearVelocity;
        public float m_allowedPenetrationDepth;
        public Vector4 m_up;
        public float m_maxSlope;
        public float m_maxForce;
        public float m_unweldingHeightOffsetFactor;
        public float m_maxSpeedForSimplexSolver;
        public float m_supportDistance;
        public float m_hardSupportDistance;
        public int m_vdbColor;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_collisionFilterInfo = br.ReadUInt32();
            br.AssertUInt32(0);
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_position = des.ReadVector4(br);
            m_rotation = des.ReadQuaternion(br);
            m_mass = br.ReadSingle();
            m_friction = br.ReadSingle();
            m_maxLinearVelocity = br.ReadSingle();
            m_allowedPenetrationDepth = br.ReadSingle();
            m_up = des.ReadVector4(br);
            m_maxSlope = br.ReadSingle();
            m_maxForce = br.ReadSingle();
            m_unweldingHeightOffsetFactor = br.ReadSingle();
            m_maxSpeedForSimplexSolver = br.ReadSingle();
            m_supportDistance = br.ReadSingle();
            m_hardSupportDistance = br.ReadSingle();
            m_vdbColor = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteSingle(m_mass);
            bw.WriteSingle(m_friction);
            bw.WriteSingle(m_maxLinearVelocity);
            bw.WriteSingle(m_allowedPenetrationDepth);
            bw.WriteSingle(m_maxSlope);
            bw.WriteSingle(m_maxForce);
            bw.WriteSingle(m_unweldingHeightOffsetFactor);
            bw.WriteSingle(m_maxSpeedForSimplexSolver);
            bw.WriteSingle(m_supportDistance);
            bw.WriteSingle(m_hardSupportDistance);
            bw.WriteInt32(m_vdbColor);
            bw.WriteUInt32(0);
        }
    }
}
