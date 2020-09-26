using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclVertexCopySetupObject : hclOperatorSetupObject
    {
        public string m_name;
        public hclBufferSetupObject m_inputBufferSetupObject;
        public hclBufferSetupObject m_outputBufferSetupObject;
        public bool m_copyNormals;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_inputBufferSetupObject = des.ReadClassPointer<hclBufferSetupObject>(br);
            m_outputBufferSetupObject = des.ReadClassPointer<hclBufferSetupObject>(br);
            m_copyNormals = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteBoolean(m_copyNormals);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
