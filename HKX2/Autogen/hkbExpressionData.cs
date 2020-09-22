using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ExpressionEventMode
    {
        EVENT_MODE_SEND_ONCE = 0,
        EVENT_MODE_SEND_ON_TRUE = 1,
        EVENT_MODE_SEND_ON_FALSE_TO_TRUE = 2,
        EVENT_MODE_SEND_EVERY_FRAME_ONCE_TRUE = 3,
    }
    
    public class hkbExpressionData
    {
        public string m_expression;
        public int m_assignmentVariableIndex;
        public int m_assignmentEventIndex;
        public ExpressionEventMode m_eventMode;
    }
}
