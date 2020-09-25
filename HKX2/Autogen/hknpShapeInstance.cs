using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpShapeInstance : IHavokObject
    {
        public enum Flags
        {
            HAS_TRANSLATION = 2,
            HAS_ROTATION = 4,
            HAS_SCALE = 8,
            FLIP_ORIENTATION = 16,
            SCALE_SURFACE = 32,
            IS_ENABLED = 64,
            DEFAULT_FLAGS = 64,
        }
        
        public Matrix4x4 m_transform;
        public Vector4 m_scale;
        public hknpShape m_shape;
        public ushort m_shapeTag;
        public ushort m_destructionTag;
        public byte m_padding;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transform = des.ReadTransform(br);
            m_scale = des.ReadVector4(br);
            m_shape = des.ReadClassPointer<hknpShape>(br);
            m_shapeTag = br.ReadUInt16();
            m_destructionTag = br.ReadUInt16();
            m_padding = br.ReadByte();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt16(m_shapeTag);
            bw.WriteUInt16(m_destructionTag);
            bw.WriteByte(m_padding);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
