using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbpCheckBalanceModifier : hkbModifier
    {
        public short m_ragdollLeftFootBoneIndex;
        public short m_ragdollRightFootBoneIndex;
        public float m_balanceOnAnklesFraction;
        public hkbEvent m_eventToSendWhenOffBalance;
        public float m_offBalanceEventThreshold;
        public int m_worldUpAxisIndex;
        public float m_comBiasX;
        public bool m_extractRagdollPose;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_ragdollLeftFootBoneIndex = br.ReadInt16();
            m_ragdollRightFootBoneIndex = br.ReadInt16();
            m_balanceOnAnklesFraction = br.ReadSingle();
            m_eventToSendWhenOffBalance = new hkbEvent();
            m_eventToSendWhenOffBalance.Read(des, br);
            m_offBalanceEventThreshold = br.ReadSingle();
            m_worldUpAxisIndex = br.ReadInt32();
            m_comBiasX = br.ReadSingle();
            m_extractRagdollPose = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt16(m_ragdollLeftFootBoneIndex);
            bw.WriteInt16(m_ragdollRightFootBoneIndex);
            bw.WriteSingle(m_balanceOnAnklesFraction);
            m_eventToSendWhenOffBalance.Write(bw);
            bw.WriteSingle(m_offBalanceEventThreshold);
            bw.WriteInt32(m_worldUpAxisIndex);
            bw.WriteSingle(m_comBiasX);
            bw.WriteBoolean(m_extractRagdollPose);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
