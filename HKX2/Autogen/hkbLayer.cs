using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbLayer : hkbBindable
    {
        public hkbGenerator m_generator;
        public float m_weight;
        public hkbBoneWeightArray m_boneWeights;
        public float m_fadeInDuration;
        public float m_fadeOutDuration;
        public int m_onEventId;
        public int m_offEventId;
        public bool m_onByDefault;
        public bool m_useMotion;
        public bool m_forceFullFadeDurations;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
            m_weight = br.ReadSingle();
            br.ReadUInt32();
            m_boneWeights = des.ReadClassPointer<hkbBoneWeightArray>(br);
            m_fadeInDuration = br.ReadSingle();
            m_fadeOutDuration = br.ReadSingle();
            m_onEventId = br.ReadInt32();
            m_offEventId = br.ReadInt32();
            m_onByDefault = br.ReadBoolean();
            m_useMotion = br.ReadBoolean();
            m_forceFullFadeDurations = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteSingle(m_weight);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteSingle(m_fadeInDuration);
            bw.WriteSingle(m_fadeOutDuration);
            bw.WriteInt32(m_onEventId);
            bw.WriteInt32(m_offEventId);
            bw.WriteBoolean(m_onByDefault);
            bw.WriteBoolean(m_useMotion);
            bw.WriteBoolean(m_forceFullFadeDurations);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
