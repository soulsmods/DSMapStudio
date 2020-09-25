using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpStiffSpringChainData : hkpConstraintChainData
    {
        public hkpBridgeAtoms m_atoms;
        public List<hkpStiffSpringChainDataConstraintInfo> m_infos;
        public float m_tau;
        public float m_damping;
        public float m_cfm;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_atoms = new hkpBridgeAtoms();
            m_atoms.Read(des, br);
            m_infos = des.ReadClassArray<hkpStiffSpringChainDataConstraintInfo>(br);
            m_tau = br.ReadSingle();
            m_damping = br.ReadSingle();
            m_cfm = br.ReadSingle();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            m_atoms.Write(bw);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_damping);
            bw.WriteSingle(m_cfm);
            bw.WriteUInt32(0);
        }
    }
}
