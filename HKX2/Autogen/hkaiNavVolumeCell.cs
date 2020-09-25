using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiNavVolumeCell : IHavokObject
    {
        public ushort m_min;
        public short m_numEdges;
        public ushort m_max;
        public int m_startEdgeIndex;
        public int m_data;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_min = br.ReadUInt16();
            br.AssertUInt32(0);
            m_numEdges = br.ReadInt16();
            m_max = br.ReadUInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_startEdgeIndex = br.ReadInt32();
            m_data = br.ReadInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_min);
            bw.WriteUInt32(0);
            bw.WriteInt16(m_numEdges);
            bw.WriteUInt16(m_max);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteInt32(m_startEdgeIndex);
            bw.WriteInt32(m_data);
        }
    }
}
