using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclMoveFixedParticlesSetupObject : hclOperatorSetupObject
    {
        public string m_name;
        public hclSimClothSetupObject m_simClothSetupObject;
        public hclBufferSetupObject m_displayBufferSetup;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_simClothSetupObject = des.ReadClassPointer<hclSimClothSetupObject>(br);
            m_displayBufferSetup = des.ReadClassPointer<hclBufferSetupObject>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
