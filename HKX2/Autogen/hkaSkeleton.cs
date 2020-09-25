using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaSkeleton : hkReferencedObject
    {
        public string m_name;
        public List<short> m_parentIndices;
        public List<hkaBone> m_bones;
        public List<Matrix4x4> m_referencePose;
        public List<float> m_referenceFloats;
        public List<string> m_floatSlots;
        public List<hkaSkeletonLocalFrameOnBone> m_localFrames;
        public List<hkaSkeletonPartition> m_partitions;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_parentIndices = des.ReadInt16Array(br);
            m_bones = des.ReadClassArray<hkaBone>(br);
            m_referencePose = des.ReadQSTransformArray(br);
            m_referenceFloats = des.ReadSingleArray(br);
            m_floatSlots = des.ReadStringPointerArray(br);
            m_localFrames = des.ReadClassArray<hkaSkeletonLocalFrameOnBone>(br);
            m_partitions = des.ReadClassArray<hkaSkeletonPartition>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
