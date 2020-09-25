using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticMeshTreeBasePrimitiveDataRunBasehknpCompressedMeshShapeTreeDataRunData : IHavokObject
    {
        public hknpCompressedMeshShapeTreeDataRunData m_value;
        public byte m_index;
        public byte m_count;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_value = new hknpCompressedMeshShapeTreeDataRunData();
            m_value.Read(des, br);
            m_index = br.ReadByte();
            m_count = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_value.Write(bw);
            bw.WriteByte(m_index);
            bw.WriteByte(m_count);
        }
    }
}
