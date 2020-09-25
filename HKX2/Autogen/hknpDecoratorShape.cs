using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpDecoratorShape : hknpShape
    {
        public hknpShape m_coreShape;
        public int m_coreShapeSize;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_coreShape = des.ReadClassPointer<hknpShape>(br);
            m_coreShapeSize = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteInt32(m_coreShapeSize);
            bw.WriteUInt32(0);
        }
    }
}
