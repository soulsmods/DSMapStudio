using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdShape : hkReferencedObject
    {
        public ShapeDispatchTypeEnum m_dispatchType;
        public byte m_bitsPerKey;
        public ShapeInfoCodecTypeEnum m_shapeInfoCodecType;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertByte(0);
            m_dispatchType = (ShapeDispatchTypeEnum)br.ReadByte();
            m_bitsPerKey = br.ReadByte();
            m_shapeInfoCodecType = (ShapeInfoCodecTypeEnum)br.ReadByte();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(0);
            bw.WriteByte(m_bitsPerKey);
            bw.WriteUInt32(0);
        }
    }
}
