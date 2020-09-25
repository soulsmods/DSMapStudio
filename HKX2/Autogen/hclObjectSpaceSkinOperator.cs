using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceSkinOperator : hclOperator
    {
        public List<Matrix4x4> m_boneFromSkinMeshTransforms;
        public List<ushort> m_transformSubset;
        public uint m_outputBufferIndex;
        public uint m_transformSetIndex;
        public hclObjectSpaceDeformer m_objectSpaceDeformer;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_boneFromSkinMeshTransforms = des.ReadMatrix4Array(br);
            m_transformSubset = des.ReadUInt16Array(br);
            m_outputBufferIndex = br.ReadUInt32();
            m_transformSetIndex = br.ReadUInt32();
            m_objectSpaceDeformer = new hclObjectSpaceDeformer();
            m_objectSpaceDeformer.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_outputBufferIndex);
            bw.WriteUInt32(m_transformSetIndex);
            m_objectSpaceDeformer.Write(bw);
        }
    }
}
