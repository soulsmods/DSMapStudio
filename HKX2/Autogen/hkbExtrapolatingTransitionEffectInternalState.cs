using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbExtrapolatingTransitionEffectInternalState : hkReferencedObject
    {
        public hkbGeneratorSyncInfo m_fromGeneratorSyncInfo;
        public hkbGeneratorPartitionInfo m_fromGeneratorPartitionInfo;
        public Matrix4x4 m_worldFromModel;
        public Matrix4x4 m_motion;
        public List<Matrix4x4> m_pose;
        public List<Matrix4x4> m_additivePose;
        public List<float> m_boneWeights;
        public float m_toGeneratorDuration;
        public bool m_isFromGeneratorActive;
        public bool m_gotPose;
        public bool m_gotAdditivePose;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_fromGeneratorSyncInfo = new hkbGeneratorSyncInfo();
            m_fromGeneratorSyncInfo.Read(des, br);
            m_fromGeneratorPartitionInfo = new hkbGeneratorPartitionInfo();
            m_fromGeneratorPartitionInfo.Read(des, br);
            br.AssertUInt32(0);
            m_worldFromModel = des.ReadQSTransform(br);
            m_motion = des.ReadQSTransform(br);
            m_pose = des.ReadQSTransformArray(br);
            m_additivePose = des.ReadQSTransformArray(br);
            m_boneWeights = des.ReadSingleArray(br);
            m_toGeneratorDuration = br.ReadSingle();
            m_isFromGeneratorActive = br.ReadBoolean();
            m_gotPose = br.ReadBoolean();
            m_gotAdditivePose = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_fromGeneratorSyncInfo.Write(bw);
            m_fromGeneratorPartitionInfo.Write(bw);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_toGeneratorDuration);
            bw.WriteBoolean(m_isFromGeneratorActive);
            bw.WriteBoolean(m_gotPose);
            bw.WriteBoolean(m_gotAdditivePose);
            bw.WriteUInt64(0);
            bw.WriteByte(0);
        }
    }
}
