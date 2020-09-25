using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticTreeTreehkcdStaticTreeDynamicStorage32 : hkcdStaticTreeDynamicStorage32
    {
        public hkAabb m_domain;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_domain = new hkAabb();
            m_domain.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_domain.Write(bw);
        }
    }
}
