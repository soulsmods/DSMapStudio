using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbStateMachineStateInfo : hkbBindable
    {
        public List<hkbStateListener> m_listeners;
        public hkbStateMachineEventPropertyArray m_enterNotifyEvents;
        public hkbStateMachineEventPropertyArray m_exitNotifyEvents;
        public hkbStateMachineTransitionInfoArray m_transitions;
        public hkbGenerator m_generator;
        public string m_name;
        public int m_stateId;
        public float m_probability;
        public bool m_enable;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_listeners = des.ReadClassPointerArray<hkbStateListener>(br);
            m_enterNotifyEvents = des.ReadClassPointer<hkbStateMachineEventPropertyArray>(br);
            m_exitNotifyEvents = des.ReadClassPointer<hkbStateMachineEventPropertyArray>(br);
            m_transitions = des.ReadClassPointer<hkbStateMachineTransitionInfoArray>(br);
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
            m_name = des.ReadStringPointer(br);
            m_stateId = br.ReadInt32();
            m_probability = br.ReadSingle();
            m_enable = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteInt32(m_stateId);
            bw.WriteSingle(m_probability);
            bw.WriteBoolean(m_enable);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
