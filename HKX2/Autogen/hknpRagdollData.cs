using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpRagdollData : hknpPhysicsSystemData
    {
        public hkaSkeleton m_skeleton;
        public List<int> m_boneToBodyMap;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_skeleton = des.ReadClassPointer<hkaSkeleton>(br);
            m_boneToBodyMap = des.ReadInt32Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
