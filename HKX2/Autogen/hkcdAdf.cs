using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdAdf
    {
        public float m_accuracy;
        public hkAabb m_domain;
        public Vector4 m_origin;
        public Vector4 m_scale;
        public float m_range;
        public List<uint> m_nodes;
        public List<ushort> m_voxels;
    }
}
