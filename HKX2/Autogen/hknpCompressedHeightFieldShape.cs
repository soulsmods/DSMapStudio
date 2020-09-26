using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompressedHeightFieldShape : hknpHeightFieldShape
    {
        public List<ushort> m_storage;
        public List<ushort> m_shapeTags;
        public bool m_triangleFlip;
        public float m_offset;
        public float m_scale;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_storage = des.ReadUInt16Array(br);
            m_shapeTags = des.ReadUInt16Array(br);
            m_triangleFlip = br.ReadBoolean();
            br.ReadUInt16();
            br.ReadByte();
            m_offset = br.ReadSingle();
            m_scale = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_triangleFlip);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_offset);
            bw.WriteSingle(m_scale);
            bw.WriteUInt32(0);
        }
    }
}
