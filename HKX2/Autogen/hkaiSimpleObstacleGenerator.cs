using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiSimpleObstacleGenerator : hkaiObstacleGenerator
    {
        public hkAabb m_localAabb;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localAabb = new hkAabb();
            m_localAabb.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_localAabb.Write(bw);
        }
    }
}
