using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CharacterControlCommand
    {
        COMMAND_HIDE = 0,
        COMMAND_SHOW = 1,
    }
    
    public class hkbCharacterControlCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public CharacterControlCommand m_command;
        public int m_padding;
    }
}
