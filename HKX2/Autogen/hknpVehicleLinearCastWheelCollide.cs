using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleLinearCastWheelCollide : hknpVehicleWheelCollide
    {
        public List<hknpVehicleLinearCastWheelCollideWheelState> m_wheelStates;
        public float m_maxExtraPenetration;
        public float m_startPointTolerance;
        public uint m_chassisBody;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wheelStates = des.ReadClassArray<hknpVehicleLinearCastWheelCollideWheelState>(br);
            m_maxExtraPenetration = br.ReadSingle();
            m_startPointTolerance = br.ReadSingle();
            m_chassisBody = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_maxExtraPenetration);
            bw.WriteSingle(m_startPointTolerance);
            bw.WriteUInt32(m_chassisBody);
            bw.WriteUInt32(0);
        }
    }
}
