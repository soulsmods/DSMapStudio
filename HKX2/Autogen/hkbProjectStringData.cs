using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbProjectStringData : hkReferencedObject
    {
        public List<string> m_animationFilenames;
        public List<string> m_behaviorFilenames;
        public List<string> m_characterFilenames;
        public List<string> m_eventNames;
        public string m_animationPath;
        public string m_behaviorPath;
        public string m_characterPath;
        public string m_scriptsPath;
        public string m_fullPathToSource;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_animationFilenames = des.ReadStringPointerArray(br);
            m_behaviorFilenames = des.ReadStringPointerArray(br);
            m_characterFilenames = des.ReadStringPointerArray(br);
            m_eventNames = des.ReadStringPointerArray(br);
            m_animationPath = des.ReadStringPointer(br);
            m_behaviorPath = des.ReadStringPointer(br);
            m_characterPath = des.ReadStringPointer(br);
            m_scriptsPath = des.ReadStringPointer(br);
            m_fullPathToSource = des.ReadStringPointer(br);
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
