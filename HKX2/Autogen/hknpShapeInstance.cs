using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpShapeInstance
    {
        public enum Flags
        {
            HAS_TRANSLATION = 2,
            HAS_ROTATION = 4,
            HAS_SCALE = 8,
            FLIP_ORIENTATION = 16,
            SCALE_SURFACE = 32,
            IS_ENABLED = 64,
            DEFAULT_FLAGS = 64,
        }
        
        public Matrix4x4 m_transform;
        public Vector4 m_scale;
        public hknpShape m_shape;
        public ushort m_shapeTag;
        public ushort m_destructionTag;
        public byte m_padding;
    }
}
