using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclMeshBoneDeformSetupObject : hclOperatorSetupObject
    {
        public string m_name;
        public hclBufferSetupObject m_inputBufferSetupObject;
        public hclTriangleSelectionInput m_inputTriangleSelection;
        public hclTransformSetSetupObject m_outputTransformSetSetupObject;
        public List<string> m_deformedBones;
        public uint m_maxTrianglesPerBone;
        public float m_minimumTriangleWeight;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_inputBufferSetupObject = des.ReadClassPointer<hclBufferSetupObject>(br);
            m_inputTriangleSelection = new hclTriangleSelectionInput();
            m_inputTriangleSelection.Read(des, br);
            m_outputTransformSetSetupObject = des.ReadClassPointer<hclTransformSetSetupObject>(br);
            m_deformedBones = des.ReadStringPointerArray(br);
            m_maxTrianglesPerBone = br.ReadUInt32();
            m_minimumTriangleWeight = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            m_inputTriangleSelection.Write(bw);
            // Implement Write
            bw.WriteUInt32(m_maxTrianglesPerBone);
            bw.WriteSingle(m_minimumTriangleWeight);
        }
    }
}
