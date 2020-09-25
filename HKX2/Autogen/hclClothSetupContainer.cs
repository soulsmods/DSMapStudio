using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclClothSetupContainer : hkReferencedObject
    {
        public List<hclClothSetupObject> m_clothSetupDatas;
        public List<hclNamedSetupMesh> m_namedSetupMeshWrappers;
        public List<hclNamedTransformSetSetupObject> m_namedTransformSetWrappers;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_clothSetupDatas = des.ReadClassPointerArray<hclClothSetupObject>(br);
            m_namedSetupMeshWrappers = des.ReadClassPointerArray<hclNamedSetupMesh>(br);
            m_namedTransformSetWrappers = des.ReadClassPointerArray<hclNamedTransformSetSetupObject>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
