using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBreakableBody : hkReferencedObject
    {
        public hkpBreakableBodyController m_controller;
        public hkpBreakableShape m_breakableShape;
        public byte m_bodyTypeAndFlags;
        public short m_constraintStrength;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_controller = des.ReadClassPointer<hkpBreakableBodyController>(br);
            m_breakableShape = des.ReadClassPointer<hkpBreakableShape>(br);
            m_bodyTypeAndFlags = br.ReadByte();
            br.AssertByte(0);
            m_constraintStrength = br.ReadInt16();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteByte(m_bodyTypeAndFlags);
            bw.WriteByte(0);
            bw.WriteInt16(m_constraintStrength);
            bw.WriteUInt32(0);
        }
    }
}
