using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpStaticCompoundShape : hknpCompoundShape
    {
        public hknpStaticCompoundShapeData m_boundingVolumeData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_boundingVolumeData = des.ReadClassPointer<hknpStaticCompoundShapeData>(br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
