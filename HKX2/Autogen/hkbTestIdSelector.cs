using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbTestIdSelector : hkbCustomIdSelector
    {
        public int m_int;
        public float m_real;
        public string m_string;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_int = br.ReadInt32();
            m_real = br.ReadSingle();
            m_string = des.ReadStringPointer(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_int);
            bw.WriteSingle(m_real);
        }
    }
}
