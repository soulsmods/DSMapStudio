using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxNodeAnnotationData : IHavokObject
    {
        public float m_time;
        public string m_description;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            br.AssertUInt32(0);
            m_description = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            bw.WriteUInt32(0);
        }
    }
}
