using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BundleType
    {
        ANIMATION_BUNDLE = 0,
    }
    
    public class hkbAssetBundle : hkReferencedObject
    {
        public List<hkReferencedObject> m_assets;
        public string m_name;
        public BundleType m_type;
    }
}
