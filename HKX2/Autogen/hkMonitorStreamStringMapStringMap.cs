using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMonitorStreamStringMapStringMap : IHavokObject
    {
        public ulong m_id;
        public string m_string;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_id = br.ReadUInt64();
            m_string = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(m_id);
        }
    }
}
