using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompositeShape : hknpShape
    {
        public hknpSparseCompactMapunsignedshort m_edgeWeldingMap;
        public uint m_shapeTagCodecInfo;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_edgeWeldingMap = new hknpSparseCompactMapunsignedshort();
            m_edgeWeldingMap.Read(des, br);
            m_shapeTagCodecInfo = br.ReadUInt32();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_edgeWeldingMap.Write(bw);
            bw.WriteUInt32(m_shapeTagCodecInfo);
            bw.WriteUInt32(0);
        }
    }
}
