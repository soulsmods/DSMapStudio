using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbFootIkControlsModifier : hkbModifier
    {
        public hkbFootIkControlData m_controlData;
        public List<hkbFootIkControlsModifierLeg> m_legs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_controlData = new hkbFootIkControlData();
            m_controlData.Read(des, br);
            m_legs = des.ReadClassArray<hkbFootIkControlsModifierLeg>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            m_controlData.Write(bw);
        }
    }
}
