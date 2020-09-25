using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaSkeletonPartition : IHavokObject
    {
        public string m_name;
        public short m_startBoneIndex;
        public short m_numBones;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_startBoneIndex = br.ReadInt16();
            m_numBones = br.ReadInt16();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt16(m_startBoneIndex);
            bw.WriteInt16(m_numBones);
            bw.WriteUInt32(0);
        }
    }
}
