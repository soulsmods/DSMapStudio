using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConvexPieceMeshShape : hkpShapeCollection
    {
        public hkpConvexPieceStreamData m_convexPieceStream;
        public hkpShapeCollection m_displayMesh;
        public float m_radius;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_convexPieceStream = des.ReadClassPointer<hkpConvexPieceStreamData>(br);
            m_displayMesh = des.ReadClassPointer<hkpShapeCollection>(br);
            m_radius = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteSingle(m_radius);
            bw.WriteUInt32(0);
        }
    }
}
