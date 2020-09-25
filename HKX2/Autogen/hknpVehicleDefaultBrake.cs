using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleDefaultBrake : hknpVehicleBrake
    {
        public List<hknpVehicleDefaultBrakeWheelBrakingProperties> m_wheelBrakingProperties;
        public float m_wheelsMinTimeToBlock;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wheelBrakingProperties = des.ReadClassArray<hknpVehicleDefaultBrakeWheelBrakingProperties>(br);
            m_wheelsMinTimeToBlock = br.ReadSingle();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_wheelsMinTimeToBlock);
            bw.WriteUInt32(0);
        }
    }
}
