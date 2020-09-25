using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaQuantizedAnimation : hkaAnimation
    {
        public List<byte> m_data;
        public uint m_endian;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_data = des.ReadByteArray(br);
            m_endian = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_endian);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
