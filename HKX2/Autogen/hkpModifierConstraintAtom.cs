using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpModifierConstraintAtom : hkpConstraintAtom
    {
        public ushort m_modifierAtomSize;
        public ushort m_childSize;
        public hkpConstraintAtom m_child;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
            m_modifierAtomSize = br.ReadUInt16();
            m_childSize = br.ReadUInt16();
            br.ReadUInt32();
            m_child = des.ReadClassPointer<hkpConstraintAtom>(br);
            br.ReadUInt64();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteUInt16(m_modifierAtomSize);
            bw.WriteUInt16(m_childSize);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
