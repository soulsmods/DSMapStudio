using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpConvexPolytopeShapeFace : IHavokObject
    {
        public ushort m_firstIndex;
        public byte m_numIndices;
        public byte m_minHalfAngle;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_firstIndex = br.ReadUInt16();
            m_numIndices = br.ReadByte();
            m_minHalfAngle = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_firstIndex);
            bw.WriteByte(m_numIndices);
            bw.WriteByte(m_minHalfAngle);
        }
    }
}
