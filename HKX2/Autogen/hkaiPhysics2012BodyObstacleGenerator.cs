using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiPhysics2012BodyObstacleGenerator : hkaiObstacleGenerator
    {
        public float m_velocityThreshold;
        public hkpRigidBody m_rigidBody;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_velocityThreshold = br.ReadSingle();
            br.AssertUInt32(0);
            m_rigidBody = des.ReadClassPointer<hkpRigidBody>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_velocityThreshold);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
