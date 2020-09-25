using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBreakableConstraintData : hkpWrappedConstraintData
    {
        public hkpBridgeAtoms m_atoms;
        public float m_solverResultLimit;
        public bool m_removeWhenBroken;
        public bool m_revertBackVelocityOnBreak;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_atoms = new hkpBridgeAtoms();
            m_atoms.Read(des, br);
            br.AssertUInt32(0);
            m_solverResultLimit = br.ReadSingle();
            m_removeWhenBroken = br.ReadBoolean();
            m_revertBackVelocityOnBreak = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_atoms.Write(bw);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_solverResultLimit);
            bw.WriteBoolean(m_removeWhenBroken);
            bw.WriteBoolean(m_revertBackVelocityOnBreak);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
