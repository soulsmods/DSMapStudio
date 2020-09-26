using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpAngConstraintAtom : hkpConstraintAtom
    {
        public byte m_firstConstrainedAxis;
        public byte m_numConstrainedAxes;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_firstConstrainedAxis = br.ReadByte();
            m_numConstrainedAxes = br.ReadByte();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_firstConstrainedAxis);
            bw.WriteByte(m_numConstrainedAxes);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
