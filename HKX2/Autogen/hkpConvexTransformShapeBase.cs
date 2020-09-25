using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConvexTransformShapeBase : hkpConvexShape
    {
        public hkpSingleShapeContainer m_childShape;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childShape = new hkpSingleShapeContainer();
            m_childShape.Read(des, br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_childShape.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
