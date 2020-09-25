using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbComputeWorldFromModelModifier : hkbModifier
    {
        public short m_poseMatchingRootBoneIndex;
        public short m_poseMatchingOtherBoneIndex;
        public short m_poseMatchingAnotherBoneIndex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_poseMatchingRootBoneIndex = br.ReadInt16();
            m_poseMatchingOtherBoneIndex = br.ReadInt16();
            m_poseMatchingAnotherBoneIndex = br.ReadInt16();
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt16(m_poseMatchingRootBoneIndex);
            bw.WriteInt16(m_poseMatchingOtherBoneIndex);
            bw.WriteInt16(m_poseMatchingAnotherBoneIndex);
            bw.WriteUInt16(0);
        }
    }
}
