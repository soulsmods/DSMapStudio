using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCustomTestGenerator : hkbCustomTestGeneratorAnnotatedTypes
    {
        public bool m_protectedHiddenMember;
        public bool m_privateHiddenMember;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_protectedHiddenMember = br.ReadBoolean();
            m_privateHiddenMember = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_protectedHiddenMember);
            bw.WriteBoolean(m_privateHiddenMember);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
