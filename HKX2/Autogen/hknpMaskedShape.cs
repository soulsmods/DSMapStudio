using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMaskedShape : hknpDecoratorShape
    {
        public hknpShapeKeyMask m_mask;
        public hknpMaskedShapeMaskWrapper m_maskWrapper;
        public int m_maskSize;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_mask = des.ReadClassPointer<hknpShapeKeyMask>(br);
            m_maskWrapper = new hknpMaskedShapeMaskWrapper();
            m_maskWrapper.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_maskSize = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            m_maskWrapper.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteInt32(m_maskSize);
            bw.WriteUInt32(0);
        }
    }
}
