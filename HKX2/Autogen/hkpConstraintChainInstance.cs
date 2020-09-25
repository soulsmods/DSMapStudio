using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConstraintChainInstance : hkpConstraintInstance
    {
        public List<hkpEntity> m_chainedEntities;
        public hkpConstraintChainInstanceAction m_action;
        public ulong m_chainConnectedness;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_chainedEntities = des.ReadClassPointerArray<hkpEntity>(br);
            m_action = des.ReadClassPointer<hkpConstraintChainInstanceAction>(br);
            m_chainConnectedness = br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteUInt64(m_chainConnectedness);
        }
    }
}
