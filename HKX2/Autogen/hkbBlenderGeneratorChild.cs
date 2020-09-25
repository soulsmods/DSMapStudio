using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBlenderGeneratorChild : hkbBindable
    {
        public hkbGenerator m_generator;
        public hkbBoneWeightArray m_boneWeights;
        public float m_weight;
        public float m_worldFromModelWeight;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
            m_boneWeights = des.ReadClassPointer<hkbBoneWeightArray>(br);
            m_weight = br.ReadSingle();
            m_worldFromModelWeight = br.ReadSingle();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteSingle(m_weight);
            bw.WriteSingle(m_worldFromModelWeight);
            bw.WriteUInt64(0);
        }
    }
}
