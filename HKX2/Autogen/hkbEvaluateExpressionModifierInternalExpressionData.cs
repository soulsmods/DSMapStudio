using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEvaluateExpressionModifierInternalExpressionData : IHavokObject
    {
        public bool m_raisedEvent;
        public bool m_wasTrueInPreviousFrame;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_raisedEvent = br.ReadBoolean();
            m_wasTrueInPreviousFrame = br.ReadBoolean();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteBoolean(m_raisedEvent);
            bw.WriteBoolean(m_wasTrueInPreviousFrame);
        }
    }
}
