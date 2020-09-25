using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbExtractRagdollPoseModifier : hkbModifier
    {
        public short m_poseMatchingBone0;
        public short m_poseMatchingBone1;
        public short m_poseMatchingBone2;
        public bool m_enableComputeWorldFromModel;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_poseMatchingBone0 = br.ReadInt16();
            m_poseMatchingBone1 = br.ReadInt16();
            m_poseMatchingBone2 = br.ReadInt16();
            m_enableComputeWorldFromModel = br.ReadBoolean();
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt16(m_poseMatchingBone0);
            bw.WriteInt16(m_poseMatchingBone1);
            bw.WriteInt16(m_poseMatchingBone2);
            bw.WriteBoolean(m_enableComputeWorldFromModel);
            bw.WriteByte(0);
        }
    }
}
