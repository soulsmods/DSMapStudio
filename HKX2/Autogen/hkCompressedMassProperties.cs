using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkCompressedMassProperties : IHavokObject
    {
        public short m_centerOfMass;
        public short m_inertia;
        public short m_majorAxisSpace;
        public float m_mass;
        public float m_volume;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_centerOfMass = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_inertia = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_majorAxisSpace = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_mass = br.ReadSingle();
            m_volume = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt16(m_centerOfMass);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteInt16(m_inertia);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteInt16(m_majorAxisSpace);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_mass);
            bw.WriteSingle(m_volume);
        }
    }
}
