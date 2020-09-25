using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbScriptGenerator : hkbGenerator
    {
        public hkbGenerator m_child;
        public string m_onActivateScript;
        public string m_onPreUpdateScript;
        public string m_onGenerateScript;
        public string m_onHandleEventScript;
        public string m_onDeactivateScript;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_child = des.ReadClassPointer<hkbGenerator>(br);
            m_onActivateScript = des.ReadStringPointer(br);
            m_onPreUpdateScript = des.ReadStringPointer(br);
            m_onGenerateScript = des.ReadStringPointer(br);
            m_onHandleEventScript = des.ReadStringPointer(br);
            m_onDeactivateScript = des.ReadStringPointer(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
