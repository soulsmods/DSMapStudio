using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSkinnedRefMeshShape : hkMeshShape
    {
        public hkSkinnedMeshShape m_skinnedMeshShape;
        public List<short> m_bones;
        public List<Vector4> m_localFromRootTransforms;
        public string m_name;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_skinnedMeshShape = des.ReadClassPointer<hkSkinnedMeshShape>(br);
            m_bones = des.ReadInt16Array(br);
            m_localFromRootTransforms = des.ReadVector4Array(br);
            m_name = des.ReadStringPointer(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
