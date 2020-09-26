using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum VertexFloatDimensions
    {
        FLOAT = 0,
        DISTANCE = 1,
        ANGLE = 2,
    }
    
    public class hkxVertexFloatDataChannel : hkReferencedObject
    {
        public List<float> m_perVertexFloats;
        public VertexFloatDimensions m_dimensions;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_perVertexFloats = des.ReadSingleArray(br);
            m_dimensions = (VertexFloatDimensions)br.ReadByte();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
