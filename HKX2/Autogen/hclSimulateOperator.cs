using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimulateOperator : hclOperator
    {
        public uint m_simClothIndex;
        public uint m_subSteps;
        public int m_numberOfSolveIterations;
        public List<int> m_constraintExecution;
        public bool m_adaptConstraintStiffness;
    }
}
