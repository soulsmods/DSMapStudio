using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaSkeletonLocalFrameOnBone : IHavokObject
    {
        public hkLocalFrame m_localFrame;
        public short m_boneIndex;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_localFrame = des.ReadClassPointer<hkLocalFrame>(br);
            m_boneIndex = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteInt16(m_boneIndex);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
