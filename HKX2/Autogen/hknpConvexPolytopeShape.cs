using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpConvexPolytopeShape : hknpConvexShape
    {
        public List<Vector4> m_planes;
        public List<hknpConvexPolytopeShapeFace> m_faces;
        public List<byte> m_indices;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            // Read TYPE_RELARRAY
            // Read TYPE_RELARRAY
            // Read TYPE_RELARRAY
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Read TYPE_RELARRAY
            // Read TYPE_RELARRAY
            // Read TYPE_RELARRAY
            bw.WriteUInt32(0);
        }
    }
}
