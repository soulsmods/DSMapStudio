using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaInterleavedUncompressedAnimation : hkaAnimation
    {
        public List<Matrix4x4> m_transforms;
        public List<float> m_floats;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transforms = des.ReadQSTransformArray(br);
            m_floats = des.ReadSingleArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
