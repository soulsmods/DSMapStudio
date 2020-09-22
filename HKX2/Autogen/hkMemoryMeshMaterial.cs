using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryMeshMaterial : hkMeshMaterial
    {
        public string m_materialName;
        public List<hkMeshTexture> m_textures;
        public Vector4 m_diffuseColor;
        public Vector4 m_ambientColor;
        public Vector4 m_specularColor;
        public Vector4 m_emissiveColor;
        public ulong m_userData;
        public float m_tesselationFactor;
        public float m_displacementAmount;
    }
}
