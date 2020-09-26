using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompressedMeshShape : hknpCompositeShape
    {
        public hknpCompressedMeshShapeData m_data;
        public hkBitField m_quadIsFlat;
        public hkBitField m_triangleIsInterior;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_data = des.ReadClassPointer<hknpCompressedMeshShapeData>(br);
            m_quadIsFlat = new hkBitField();
            m_quadIsFlat.Read(des, br);
            m_triangleIsInterior = new hkBitField();
            m_triangleIsInterior.Read(des, br);
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            m_quadIsFlat.Write(bw);
            m_triangleIsInterior.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
