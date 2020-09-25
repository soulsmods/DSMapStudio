using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclClothContainer : hkReferencedObject
    {
        public List<hclCollidable> m_collidables;
        public List<hclClothData> m_clothDatas;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_collidables = des.ReadClassPointerArray<hclCollidable>(br);
            m_clothDatas = des.ReadClassPointerArray<hclClothData>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
