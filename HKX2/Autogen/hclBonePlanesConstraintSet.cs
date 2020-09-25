using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBonePlanesConstraintSet : hclConstraintSet
    {
        public List<hclBonePlanesConstraintSetBonePlane> m_bonePlanes;
        public uint m_transformSetIndex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bonePlanes = des.ReadClassArray<hclBonePlanesConstraintSetBonePlane>(br);
            m_transformSetIndex = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_transformSetIndex);
            bw.WriteUInt32(0);
        }
    }
}
