using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbPinBonesGenerator : hkbGenerator
    {
        public hkbGenerator m_referenceFrameGenerator;
        public hkbGenerator m_pinnedGenerator;
        public hkbBoneIndexArray m_boneIndices;
        public float m_fraction;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_referenceFrameGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_pinnedGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_boneIndices = des.ReadClassPointer<hkbBoneIndexArray>(br);
            m_fraction = br.ReadSingle();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteSingle(m_fraction);
            bw.WriteUInt32(0);
        }
    }
}
