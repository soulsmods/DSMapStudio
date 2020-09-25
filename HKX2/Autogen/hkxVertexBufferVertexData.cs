using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxVertexBufferVertexData : IHavokObject
    {
        public List<uint> m_vectorData;
        public List<uint> m_floatData;
        public List<uint> m_uint32Data;
        public List<ushort> m_uint16Data;
        public List<byte> m_uint8Data;
        public uint m_numVerts;
        public uint m_vectorStride;
        public uint m_floatStride;
        public uint m_uint32Stride;
        public uint m_uint16Stride;
        public uint m_uint8Stride;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_vectorData = des.ReadUInt32Array(br);
            m_floatData = des.ReadUInt32Array(br);
            m_uint32Data = des.ReadUInt32Array(br);
            m_uint16Data = des.ReadUInt16Array(br);
            m_uint8Data = des.ReadByteArray(br);
            m_numVerts = br.ReadUInt32();
            m_vectorStride = br.ReadUInt32();
            m_floatStride = br.ReadUInt32();
            m_uint32Stride = br.ReadUInt32();
            m_uint16Stride = br.ReadUInt32();
            m_uint8Stride = br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_numVerts);
            bw.WriteUInt32(m_vectorStride);
            bw.WriteUInt32(m_floatStride);
            bw.WriteUInt32(m_uint32Stride);
            bw.WriteUInt32(m_uint16Stride);
            bw.WriteUInt32(m_uint8Stride);
        }
    }
}
