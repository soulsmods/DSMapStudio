using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpConstraintCinfo : IHavokObject
    {
        public hkpConstraintData m_constraintData;
        public uint m_bodyA;
        public uint m_bodyB;
        public byte m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_constraintData = des.ReadClassPointer<hkpConstraintData>(br);
            m_bodyA = br.ReadUInt32();
            m_bodyB = br.ReadUInt32();
            m_flags = br.ReadByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt32(m_bodyA);
            bw.WriteUInt32(m_bodyB);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
