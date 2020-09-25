using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGetHandleOnBoneModifier : hkbModifier
    {
        public hkbHandle m_handleOut;
        public string m_localFrameName;
        public short m_ragdollBoneIndex;
        public short m_animationBoneIndex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_handleOut = des.ReadClassPointer<hkbHandle>(br);
            m_localFrameName = des.ReadStringPointer(br);
            m_ragdollBoneIndex = br.ReadInt16();
            m_animationBoneIndex = br.ReadInt16();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteInt16(m_ragdollBoneIndex);
            bw.WriteInt16(m_animationBoneIndex);
            bw.WriteUInt32(0);
        }
    }
}
