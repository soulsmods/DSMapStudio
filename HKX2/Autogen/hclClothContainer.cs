using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclClothContainer : hkReferencedObject
    {
        public List<hclCollidable> m_collidables;
        public List<hclClothData> m_clothDatas;
    }
}
