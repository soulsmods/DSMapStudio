using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkFloat16Transform : IHavokObject
    {
        public hkFloat16 m_elements_0;
        public hkFloat16 m_elements_1;
        public hkFloat16 m_elements_2;
        public hkFloat16 m_elements_3;
        public hkFloat16 m_elements_4;
        public hkFloat16 m_elements_5;
        public hkFloat16 m_elements_6;
        public hkFloat16 m_elements_7;
        public hkFloat16 m_elements_8;
        public hkFloat16 m_elements_9;
        public hkFloat16 m_elements_10;
        public hkFloat16 m_elements_11;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_elements_0 = new hkFloat16();
            m_elements_0.Read(des, br);
            m_elements_1 = new hkFloat16();
            m_elements_1.Read(des, br);
            m_elements_2 = new hkFloat16();
            m_elements_2.Read(des, br);
            m_elements_3 = new hkFloat16();
            m_elements_3.Read(des, br);
            m_elements_4 = new hkFloat16();
            m_elements_4.Read(des, br);
            m_elements_5 = new hkFloat16();
            m_elements_5.Read(des, br);
            m_elements_6 = new hkFloat16();
            m_elements_6.Read(des, br);
            m_elements_7 = new hkFloat16();
            m_elements_7.Read(des, br);
            m_elements_8 = new hkFloat16();
            m_elements_8.Read(des, br);
            m_elements_9 = new hkFloat16();
            m_elements_9.Read(des, br);
            m_elements_10 = new hkFloat16();
            m_elements_10.Read(des, br);
            m_elements_11 = new hkFloat16();
            m_elements_11.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_elements_0.Write(bw);
            m_elements_1.Write(bw);
            m_elements_2.Write(bw);
            m_elements_3.Write(bw);
            m_elements_4.Write(bw);
            m_elements_5.Write(bw);
            m_elements_6.Write(bw);
            m_elements_7.Write(bw);
            m_elements_8.Write(bw);
            m_elements_9.Write(bw);
            m_elements_10.Write(bw);
            m_elements_11.Write(bw);
        }
    }
}
