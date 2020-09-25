using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiSilhouetteGeneratorSectionContext : IHavokObject
    {
        public hkQTransform m_lastRelativeTransform;
        public hkaiSilhouetteGenerator m_generator;
        public bool m_generatedLastFrame;
        public bool m_generatingThisFrame;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_lastRelativeTransform = new hkQTransform();
            m_lastRelativeTransform.Read(des, br);
            m_generator = des.ReadClassPointer<hkaiSilhouetteGenerator>(br);
            br.AssertUInt32(0);
            m_generatedLastFrame = br.ReadBoolean();
            m_generatingThisFrame = br.ReadBoolean();
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_lastRelativeTransform.Write(bw);
            // Implement Write
            bw.WriteUInt32(0);
            bw.WriteBoolean(m_generatedLastFrame);
            bw.WriteBoolean(m_generatingThisFrame);
            bw.WriteUInt16(0);
        }
    }
}
