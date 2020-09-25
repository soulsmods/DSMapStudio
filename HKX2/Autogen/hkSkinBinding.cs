using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSkinBinding : hkMeshShape
    {
        public hkMeshShape m_skin;
        public List<Matrix4x4> m_worldFromBoneTransforms;
        public List<string> m_boneNames;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_skin = des.ReadClassPointer<hkMeshShape>(br);
            m_worldFromBoneTransforms = des.ReadMatrix4Array(br);
            m_boneNames = des.ReadStringPointerArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
