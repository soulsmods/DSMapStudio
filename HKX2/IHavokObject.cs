using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace HKX2
{
    public interface IHavokObject
    {
        public abstract void Read(PackFileDeserializer des, BinaryReaderEx br);

        public abstract void Write(BinaryWriterEx bw);
    }
}
