using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpLinearParametricCurve : hkpParametricCurve
    {
        public float m_smoothingFactor;
        public bool m_closedLoop;
        public Vector4 m_dirNotParallelToTangentAlongWholePath;
        public List<Vector4> m_points;
        public List<float> m_distance;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_smoothingFactor = br.ReadSingle();
            m_closedLoop = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_dirNotParallelToTangentAlongWholePath = des.ReadVector4(br);
            m_points = des.ReadVector4Array(br);
            m_distance = des.ReadSingleArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_smoothingFactor);
            bw.WriteBoolean(m_closedLoop);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
