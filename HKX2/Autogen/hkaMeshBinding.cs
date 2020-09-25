using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaMeshBinding : hkReferencedObject
    {
        public string m_originalSkeletonName;
        public string m_name;
        public hkaSkeleton m_skeleton;
        public List<hkaMeshBindingMapping> m_mappings;
        public List<Matrix4x4> m_boneFromSkinMeshTransforms;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_originalSkeletonName = des.ReadStringPointer(br);
            m_name = des.ReadStringPointer(br);
            m_skeleton = des.ReadClassPointer<hkaSkeleton>(br);
            m_mappings = des.ReadClassArray<hkaMeshBindingMapping>(br);
            m_boneFromSkinMeshTransforms = des.ReadTransformArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
        }
    }
}
