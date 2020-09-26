using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaAnnotationTrackAnnotation : IHavokObject
    {
        public float m_time;
        public string m_text;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_time = br.ReadSingle();
            br.ReadUInt32();
            m_text = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_time);
            bw.WriteUInt32(0);
        }
    }
}
