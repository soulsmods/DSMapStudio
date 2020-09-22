using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSimpleLocalFrame : hkLocalFrame
    {
        public Matrix4x4 m_transform;
        public List<hkLocalFrame> m_children;
        public hkLocalFrame m_parentFrame;
        public hkLocalFrameGroup m_group;
        public string m_name;
    }
}
