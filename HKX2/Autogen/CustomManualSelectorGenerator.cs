using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum OffsetType
    {
        WeaponCategoryRight = 13,
        WeaponCategoryLeft = 14,
        IdleCategory = 11,
        OffsetNone = 0,
        AnimIdOffset = 15,
        WeaponCategoryHandStyle = 16,
    }
    
    public enum AnimeEndEventType
    {
        FireNextStateEvent = 0,
        FireStateEndEvent = 1,
        FireIdleEvent = 2,
        None = 3,
    }
    
    public enum ReplanningAI
    {
        Enable = 0,
        Disable = 1,
    }
    
    public enum ChangeTypeOfSelectedIndexAfterActivate
    {
        NONE = 0,
        SELF_TRANSITION = 1,
        UPDATE = 2,
    }
    
    public class CustomManualSelectorGenerator : hkbGenerator
    {
        public List<hkbGenerator> m_generators;
        public OffsetType m_offsetType;
        public int m_animId;
        public AnimeEndEventType m_animeEndEventType;
        public bool m_enableScript;
        public bool m_enableTae;
        public ChangeTypeOfSelectedIndexAfterActivate m_changeTypeOfSelection;
        public hkbTransitionEffect m_generatorChangedTransitionEffect;
        public int m_checkAnimEndSlotNo;
        public ReplanningAI m_replanningAI;
    }
}
