using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterSkinInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public List<ulong> m_deformableSkins;
        public List<ulong> m_rigidSkins;
    }
}
