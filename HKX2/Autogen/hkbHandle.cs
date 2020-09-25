using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbHandle : hkReferencedObject
    {
        public hkLocalFrame m_frame;
        public hkReferencedObject m_rigidBody;
        public hkbCharacter m_character;
        public short m_animationBoneIndex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_frame = des.ReadClassPointer<hkLocalFrame>(br);
            m_rigidBody = des.ReadClassPointer<hkReferencedObject>(br);
            m_character = des.ReadClassPointer<hkbCharacter>(br);
            m_animationBoneIndex = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteInt16(m_animationBoneIndex);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
