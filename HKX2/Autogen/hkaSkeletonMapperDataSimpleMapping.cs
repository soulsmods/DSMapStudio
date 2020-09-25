using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaSkeletonMapperDataSimpleMapping : IHavokObject
    {
        public short m_boneA;
        public short m_boneB;
        public Matrix4x4 m_aFromBTransform;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_boneA = br.ReadInt16();
            m_boneB = br.ReadInt16();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_aFromBTransform = des.ReadQSTransform(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt16(m_boneA);
            bw.WriteInt16(m_boneB);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
