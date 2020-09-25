using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclClothSetupObject : hkReferencedObject
    {
        public string m_name;
        public List<hclBufferSetupObject> m_bufferSetupObjects;
        public List<hclTransformSetSetupObject> m_transformSetSetupObjects;
        public List<hclSimClothSetupObject> m_simClothSetupObjects;
        public List<hclOperatorSetupObject> m_operatorSetupObjects;
        public List<hclClothStateSetupObject> m_clothStateSetupObjects;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_bufferSetupObjects = des.ReadClassPointerArray<hclBufferSetupObject>(br);
            m_transformSetSetupObjects = des.ReadClassPointerArray<hclTransformSetSetupObject>(br);
            m_simClothSetupObjects = des.ReadClassPointerArray<hclSimClothSetupObject>(br);
            m_operatorSetupObjects = des.ReadClassPointerArray<hclOperatorSetupObject>(br);
            m_clothStateSetupObjects = des.ReadClassPointerArray<hclClothStateSetupObject>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
