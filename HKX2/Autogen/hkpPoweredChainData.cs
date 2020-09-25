using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPoweredChainData : hkpConstraintChainData
    {
        public hkpBridgeAtoms m_atoms;
        public List<hkpPoweredChainDataConstraintInfo> m_infos;
        public float m_tau;
        public float m_damping;
        public float m_cfmLinAdd;
        public float m_cfmLinMul;
        public float m_cfmAngAdd;
        public float m_cfmAngMul;
        public float m_maxErrorDistance;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_atoms = new hkpBridgeAtoms();
            m_atoms.Read(des, br);
            m_infos = des.ReadClassArray<hkpPoweredChainDataConstraintInfo>(br);
            m_tau = br.ReadSingle();
            m_damping = br.ReadSingle();
            m_cfmLinAdd = br.ReadSingle();
            m_cfmLinMul = br.ReadSingle();
            m_cfmAngAdd = br.ReadSingle();
            m_cfmAngMul = br.ReadSingle();
            m_maxErrorDistance = br.ReadSingle();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            m_atoms.Write(bw);
            bw.WriteSingle(m_tau);
            bw.WriteSingle(m_damping);
            bw.WriteSingle(m_cfmLinAdd);
            bw.WriteSingle(m_cfmLinMul);
            bw.WriteSingle(m_cfmAngAdd);
            bw.WriteSingle(m_cfmAngMul);
            bw.WriteSingle(m_maxErrorDistance);
            bw.WriteUInt32(0);
        }
    }
}
