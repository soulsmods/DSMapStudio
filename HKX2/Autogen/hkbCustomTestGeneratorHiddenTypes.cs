using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCustomTestGeneratorHiddenTypes : hkbReferencePoseGenerator
    {
        public bool m_inheritedHiddenMember;
        public bool m_protectedInheritedHiddenMember;
        public bool m_privateInheritedHiddenMember;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_inheritedHiddenMember = br.ReadBoolean();
            m_protectedInheritedHiddenMember = br.ReadBoolean();
            m_privateInheritedHiddenMember = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_inheritedHiddenMember);
            bw.WriteBoolean(m_protectedInheritedHiddenMember);
            bw.WriteBoolean(m_privateInheritedHiddenMember);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
