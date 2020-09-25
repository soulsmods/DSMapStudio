using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiStreamingSetVolumeConnection : IHavokObject
    {
        public int m_cellIndex;
        public int m_oppositeCellIndex;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_cellIndex = br.ReadInt32();
            m_oppositeCellIndex = br.ReadInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_cellIndex);
            bw.WriteInt32(m_oppositeCellIndex);
        }
    }
}
