using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpBinaryAction : hknpAction
    {
        public uint m_bodyIdA;
        public uint m_bodyIdB;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bodyIdA = br.ReadUInt32();
            m_bodyIdB = br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_bodyIdA);
            bw.WriteUInt32(m_bodyIdB);
        }
    }
}
