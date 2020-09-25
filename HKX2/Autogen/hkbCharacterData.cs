using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterData : hkReferencedObject
    {
        public hkbCharacterControllerSetup m_characterControllerSetup;
        public Vector4 m_modelUpMS;
        public Vector4 m_modelForwardMS;
        public Vector4 m_modelRightMS;
        public List<hkbVariableInfo> m_characterPropertyInfos;
        public List<int> m_numBonesPerLod;
        public hkbVariableValueSet m_characterPropertyValues;
        public hkbFootIkDriverInfo m_footIkDriverInfo;
        public hkbHandIkDriverInfo m_handIkDriverInfo;
        public hkReferencedObject m_aiControlDriverInfo;
        public hkbCharacterStringData m_stringData;
        public hkbMirroredSkeletonInfo m_mirroredSkeletonInfo;
        public List<short> m_boneAttachmentBoneIndices;
        public List<Matrix4x4> m_boneAttachmentTransforms;
        public float m_scale;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterControllerSetup = new hkbCharacterControllerSetup();
            m_characterControllerSetup.Read(des, br);
            br.AssertUInt64(0);
            m_modelUpMS = des.ReadVector4(br);
            m_modelForwardMS = des.ReadVector4(br);
            m_modelRightMS = des.ReadVector4(br);
            m_characterPropertyInfos = des.ReadClassArray<hkbVariableInfo>(br);
            m_numBonesPerLod = des.ReadInt32Array(br);
            m_characterPropertyValues = des.ReadClassPointer<hkbVariableValueSet>(br);
            m_footIkDriverInfo = des.ReadClassPointer<hkbFootIkDriverInfo>(br);
            m_handIkDriverInfo = des.ReadClassPointer<hkbHandIkDriverInfo>(br);
            m_aiControlDriverInfo = des.ReadClassPointer<hkReferencedObject>(br);
            m_stringData = des.ReadClassPointer<hkbCharacterStringData>(br);
            m_mirroredSkeletonInfo = des.ReadClassPointer<hkbMirroredSkeletonInfo>(br);
            m_boneAttachmentBoneIndices = des.ReadInt16Array(br);
            m_boneAttachmentTransforms = des.ReadMatrix4Array(br);
            m_scale = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_characterControllerSetup.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteSingle(m_scale);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
