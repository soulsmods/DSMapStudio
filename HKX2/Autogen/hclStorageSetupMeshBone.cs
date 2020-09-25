using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStorageSetupMeshBone : IHavokObject
    {
        public string m_name;
        public Matrix4x4 m_boneFromSkin;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            br.AssertUInt64(0);
            m_boneFromSkin = des.ReadMatrix4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
        }
    }
}
