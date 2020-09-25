using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMonitorStreamStringMap : IHavokObject
    {
        public List<hkMonitorStreamStringMapStringMap> m_map;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_map = des.ReadClassArray<hkMonitorStreamStringMapStringMap>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
