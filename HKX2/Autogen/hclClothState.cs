using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclClothState : hkReferencedObject
    {
        public string m_name;
        public List<uint> m_operators;
        public List<hclClothStateBufferAccess> m_usedBuffers;
        public List<hclClothStateTransformSetAccess> m_usedTransformSets;
        public List<uint> m_usedSimCloths;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_operators = des.ReadUInt32Array(br);
            m_usedBuffers = des.ReadClassArray<hclClothStateBufferAccess>(br);
            m_usedTransformSets = des.ReadClassArray<hclClothStateTransformSetAccess>(br);
            m_usedSimCloths = des.ReadUInt32Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
