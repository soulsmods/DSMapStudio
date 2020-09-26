using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpConvexShape : hknpShape
    {
        public List<Vector4> m_vertices;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            // Read TYPE_RELARRAY
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Read TYPE_RELARRAY
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
