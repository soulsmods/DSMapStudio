using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CallbackType
    {
        CALLBACK_MOTOR_TYPE_HAVOK_DEMO_SPRING_DAMPER = 0,
        CALLBACK_MOTOR_TYPE_USER_0 = 1,
        CALLBACK_MOTOR_TYPE_USER_1 = 2,
        CALLBACK_MOTOR_TYPE_USER_2 = 3,
        CALLBACK_MOTOR_TYPE_USER_3 = 4,
    }
    
    public class hkpCallbackConstraintMotor : hkpLimitedForceConstraintMotor
    {
        public CallbackType m_callbackType;
        public ulong m_userData0;
        public ulong m_userData1;
        public ulong m_userData2;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_callbackType = (CallbackType)br.ReadUInt32();
            br.AssertUInt32(0);
            m_userData0 = br.ReadUInt64();
            m_userData1 = br.ReadUInt64();
            m_userData2 = br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt64(m_userData0);
            bw.WriteUInt64(m_userData1);
            bw.WriteUInt64(m_userData2);
        }
    }
}
