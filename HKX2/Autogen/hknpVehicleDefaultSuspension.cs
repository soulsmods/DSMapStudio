using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleDefaultSuspension : hknpVehicleSuspension
    {
        public List<hknpVehicleDefaultSuspensionWheelSpringSuspensionParameters> m_wheelSpringParams;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wheelSpringParams = des.ReadClassArray<hknpVehicleDefaultSuspensionWheelSpringSuspensionParameters>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
