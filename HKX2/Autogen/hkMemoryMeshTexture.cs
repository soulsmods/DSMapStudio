using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryMeshTexture : hkMeshTexture
    {
        public string m_filename;
        public List<byte> m_data;
        public Format m_format;
        public bool m_hasMipMaps;
        public FilterMode m_filterMode;
        public TextureUsageType m_usageHint;
        public int m_textureCoordChannel;
    }
}
