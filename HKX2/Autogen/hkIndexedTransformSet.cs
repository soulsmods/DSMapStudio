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
    }
}
