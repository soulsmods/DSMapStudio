using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticTreeCodec3Axis : IHavokObject
    {
        public byte m_xyz;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_xyz = br.ReadByte();
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_xyz);
            bw.WriteUInt16(0);
        }
    }
}
