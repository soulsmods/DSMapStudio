using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpBridgeConstraintAtom : hkpConstraintAtom
    {
        public int m_numSolverResults;
        public hkpConstraintData m_constraintData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt16();
            m_numSolverResults = br.ReadInt32();
            m_constraintData = des.ReadClassPointer<hkpConstraintData>(br);
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt16(0);
            bw.WriteInt32(m_numSolverResults);
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
