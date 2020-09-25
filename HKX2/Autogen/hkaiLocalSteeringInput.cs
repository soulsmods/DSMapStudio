using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiLocalSteeringInput : IHavokObject
    {
        public Vector4 m_currentPosition;
        public Vector4 m_currentForward;
        public Vector4 m_currentUp;
        public Vector4 m_currentVelocity;
        public Vector4 m_desiredVelocity;
        public Vector4 m_localGoalPlane;
        public float m_distToLocalGoal;
        public bool m_applyKinematicConstraints;
        public bool m_applyAvoidanceSteering;
        public bool m_enableLocalSteering;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_currentPosition = des.ReadVector4(br);
            m_currentForward = des.ReadVector4(br);
            m_currentUp = des.ReadVector4(br);
            m_currentVelocity = des.ReadVector4(br);
            m_desiredVelocity = des.ReadVector4(br);
            m_localGoalPlane = des.ReadVector4(br);
            m_distToLocalGoal = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_applyKinematicConstraints = br.ReadBoolean();
            m_applyAvoidanceSteering = br.ReadBoolean();
            m_enableLocalSteering = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_distToLocalGoal);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteBoolean(m_applyKinematicConstraints);
            bw.WriteBoolean(m_applyAvoidanceSteering);
            bw.WriteBoolean(m_enableLocalSteering);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
