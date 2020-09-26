using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbPoweredRagdollControlsModifier : hkbModifier
    {
        public hkbPoweredRagdollControlData m_controlData;
        public hkbBoneIndexArray m_bones;
        public hkbWorldFromModelModeData m_worldFromModelModeData;
        public hkbBoneWeightArray m_boneWeights;
        public float m_animationBlendFraction;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_controlData = new hkbPoweredRagdollControlData();
            m_controlData.Read(des, br);
            m_bones = des.ReadClassPointer<hkbBoneIndexArray>(br);
            m_worldFromModelModeData = new hkbWorldFromModelModeData();
            m_worldFromModelModeData.Read(des, br);
            m_boneWeights = des.ReadClassPointer<hkbBoneWeightArray>(br);
            m_animationBlendFraction = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            m_controlData.Write(bw);
            // Implement Write
            m_worldFromModelModeData.Write(bw);
            // Implement Write
            bw.WriteSingle(m_animationBlendFraction);
            bw.WriteUInt32(0);
        }
    }
}
