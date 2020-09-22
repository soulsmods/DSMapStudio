using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpConstraint
    {
        public enum FlagsEnum
        {
            NO_FLAGS = 0,
            IS_EXPORTABLE = 1,
            IS_IMMEDIATE = 2,
            IS_DISABLED = 4,
            IS_DESTRUCTION_INTERNAL = 8,
            AUTO_REMOVE_ON_DESTRUCTION_RESET = 16,
            AUTO_REMOVE_ON_DESTRUCTION = 32,
            RAISE_CONSTRAINT_FORCE_EVENTS = 64,
            RAISE_CONSTRAINT_FORCE_EXCEEDED_EVENTS = 128,
        }
        
        public uint m_bodyIdA;
        public uint m_bodyIdB;
        public hkpConstraintData m_data;
        public uint m_id;
        public uint m_flags;
        public ConstraintType m_type;
        public ushort m_sizeOfAtoms;
        public ushort m_sizeOfSchemas;
        public byte m_numSolverResults;
        public byte m_numSolverElemTemps;
        public ushort m_runtimeSize;
        public ulong m_userData;
    }
}
