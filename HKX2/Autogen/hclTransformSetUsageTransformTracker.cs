using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclTransformSetUsageTransformTracker : IHavokObject
    {
        public hkBitField m_read;
        public hkBitField m_readBeforeWrite;
        public hkBitField m_written;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_read = new hkBitField();
            m_read.Read(des, br);
            m_readBeforeWrite = new hkBitField();
            m_readBeforeWrite.Read(des, br);
            m_written = new hkBitField();
            m_written.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_read.Write(bw);
            m_readBeforeWrite.Write(bw);
            m_written.Write(bw);
        }
    }
}
