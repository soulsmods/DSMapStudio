using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaAnimationContainer : hkReferencedObject
    {
        public List<hkaSkeleton> m_skeletons;
        public List<hkaAnimation> m_animations;
        public List<hkaAnimationBinding> m_bindings;
        public List<hkaBoneAttachment> m_attachments;
        public List<hkaMeshBinding> m_skins;
    }
}
