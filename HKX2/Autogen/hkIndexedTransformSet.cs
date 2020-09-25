using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkIndexedTransformSet : hkReferencedObject
    {
        public List<Matrix4x4> m_matrices;
        public List<Matrix4x4> m_inverseMatrices;
        public List<short> m_matricesOrder;
        public List<string> m_matricesNames;
        public List<hkMeshBoneIndexMapping> m_indexMappings;
        public bool m_allMatricesAreAffine;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_matrices = des.ReadMatrix4Array(br);
            m_inverseMatrices = des.ReadMatrix4Array(br);
            m_matricesOrder = des.ReadInt16Array(br);
            m_matricesNames = des.ReadStringPointerArray(br);
            m_indexMappings = des.ReadClassArray<hkMeshBoneIndexMapping>(br);
            m_allMatricesAreAffine = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_allMatricesAreAffine);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
