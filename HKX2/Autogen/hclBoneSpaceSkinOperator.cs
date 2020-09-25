using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceSkinOperator : hclOperator
    {
        public List<ushort> m_transformSubset;
        public uint m_outputBufferIndex;
        public uint m_transformSetIndex;
        public hclBoneSpaceDeformer m_boneSpaceDeformer;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transformSubset = des.ReadUInt16Array(br);
            m_outputBufferIndex = br.ReadUInt32();
            m_transformSetIndex = br.ReadUInt32();
            m_boneSpaceDeformer = new hclBoneSpaceDeformer();
            m_boneSpaceDeformer.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_outputBufferIndex);
            bw.WriteUInt32(m_transformSetIndex);
            m_boneSpaceDeformer.Write(bw);
        }
    }
}
