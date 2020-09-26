using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpMoppBvTreeShape : hkMoppBvTreeShapeBase
    {
        public hkpSingleShapeContainer m_child;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_child = new hkpSingleShapeContainer();
            m_child.Read(des, br);
            br.ReadUInt64();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_child.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
