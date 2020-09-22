using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbHandle : hkReferencedObject
    {
        public hkLocalFrame m_frame;
        public hkReferencedObject m_rigidBody;
        public hkbCharacter m_character;
        public short m_animationBoneIndex;
    }
}
