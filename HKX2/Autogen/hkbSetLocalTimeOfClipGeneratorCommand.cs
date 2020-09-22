using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSetLocalTimeOfClipGeneratorCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public float m_localTime;
        public ushort m_nodeId;
    }
}
