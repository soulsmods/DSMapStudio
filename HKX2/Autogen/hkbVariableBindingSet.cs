using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbVariableBindingSet : hkReferencedObject
    {
        public List<hkbVariableBindingSetBinding> m_bindings;
        public int m_indexOfBindingToEnable;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bindings = des.ReadClassArray<hkbVariableBindingSetBinding>(br);
            m_indexOfBindingToEnable = br.ReadInt32();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_indexOfBindingToEnable);
            bw.WriteUInt32(0);
        }
    }
}
