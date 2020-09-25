using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpWheelFrictionConstraintAtom : hkpConstraintAtom
    {
        public byte m_isEnabled;
        public byte m_forwardAxis;
        public byte m_sideAxis;
        public float m_maxFrictionForce;
        public float m_torque;
        public float m_radius;
        public float m_frictionImpulse;
        public float m_slipImpulse;
        public hkpWheelFrictionConstraintAtomAxle m_axle;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isEnabled = br.ReadByte();
            m_forwardAxis = br.ReadByte();
            m_sideAxis = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_maxFrictionForce = br.ReadSingle();
            m_torque = br.ReadSingle();
            m_radius = br.ReadSingle();
            m_frictionImpulse = br.ReadSingle();
            br.AssertUInt32(0);
            m_slipImpulse = br.ReadSingle();
            br.AssertUInt64(0);
            m_axle = des.ReadClassPointer<hkpWheelFrictionConstraintAtomAxle>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_isEnabled);
            bw.WriteByte(m_forwardAxis);
            bw.WriteByte(m_sideAxis);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_maxFrictionForce);
            bw.WriteSingle(m_torque);
            bw.WriteSingle(m_radius);
            bw.WriteSingle(m_frictionImpulse);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_slipImpulse);
            bw.WriteUInt64(0);
            // Implement Write
        }
    }
}
