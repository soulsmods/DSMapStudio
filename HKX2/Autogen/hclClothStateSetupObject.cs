using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclClothStateSetupObject : hkReferencedObject
    {
        public string m_name;
        public List<hclOperatorSetupObject> m_operatorSetupObjects;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_operatorSetupObjects = des.ReadClassPointerArray<hclOperatorSetupObject>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
