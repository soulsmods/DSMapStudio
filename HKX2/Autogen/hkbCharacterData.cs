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
    }
}
