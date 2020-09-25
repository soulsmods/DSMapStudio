using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleSuspension : hkReferencedObject
    {
        public List<hknpVehicleSuspensionSuspensionWheelParameters> m_wheelParams;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wheelParams = des.ReadClassArray<hknpVehicleSuspensionSuspensionWheelParameters>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
