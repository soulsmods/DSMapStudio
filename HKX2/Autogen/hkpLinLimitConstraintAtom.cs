using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinLimitConstraintAtom : hkpConstraintAtom
    {
        public byte m_axisIndex;
        public float m_min;
        public float m_max;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_axisIndex = br.ReadByte();
            br.ReadByte();
            m_min = br.ReadSingle();
            m_max = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_axisIndex);
            bw.WriteByte(0);
            bw.WriteSingle(m_min);
            bw.WriteSingle(m_max);
            bw.WriteUInt32(0);
        }
    }
}
