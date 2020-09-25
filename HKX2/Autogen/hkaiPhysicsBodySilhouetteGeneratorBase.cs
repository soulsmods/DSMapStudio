using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiPhysicsBodySilhouetteGeneratorBase : hkaiPointCloudSilhouetteGenerator
    {
        public Vector4 m_linearVelocityAndThreshold;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_linearVelocityAndThreshold = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
