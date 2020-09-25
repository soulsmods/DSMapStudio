using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpTyremarksInfo : hkReferencedObject
    {
        public float m_minTyremarkEnergy;
        public float m_maxTyremarkEnergy;
        public List<hknpTyremarksWheel> m_tyremarksWheel;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_minTyremarkEnergy = br.ReadSingle();
            m_maxTyremarkEnergy = br.ReadSingle();
            m_tyremarksWheel = des.ReadClassPointerArray<hknpTyremarksWheel>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_minTyremarkEnergy);
            bw.WriteSingle(m_maxTyremarkEnergy);
        }
    }
}
