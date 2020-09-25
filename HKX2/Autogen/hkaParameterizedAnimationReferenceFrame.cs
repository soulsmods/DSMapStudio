using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ParameterType
    {
        UNKNOWN = 0,
        LINEAR_SPEED = 1,
        LINEAR_DIRECTION = 2,
        TURN_SPEED = 3,
    }
    
    public class hkaParameterizedAnimationReferenceFrame : hkaDefaultAnimatedReferenceFrame
    {
        public List<float> m_parameterValues;
        public List<int> m_parameterTypes;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_parameterValues = des.ReadSingleArray(br);
            m_parameterTypes = des.ReadInt32Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
