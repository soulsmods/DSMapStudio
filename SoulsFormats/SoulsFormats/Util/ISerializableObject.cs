using System;
using System.Collections.Generic;
using System.Text;

namespace SoulsFormats
{
    public interface ISerializableObject
    {
        public void Write(BinaryWriterEx br);
    }
}
