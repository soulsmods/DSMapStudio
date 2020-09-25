using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBoolVariableSequencedData : hkbSequencedData
    {
        public List<hkbBoolVariableSequencedDataSample> m_samples;
        public int m_variableIndex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_samples = des.ReadClassArray<hkbBoolVariableSequencedDataSample>(br);
            m_variableIndex = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_variableIndex);
            bw.WriteUInt32(0);
        }
    }
}
