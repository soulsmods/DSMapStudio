using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclUpdateVertexFramesSetupObject : hclOperatorSetupObject
    {
        public string m_name;
        public hclBufferSetupObject m_buffer;
        public hclVertexSelectionInput m_vertexSelection;
        public bool m_updateNormals;
        public bool m_updateTangents;
        public bool m_updateBiTangents;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_buffer = des.ReadClassPointer<hclBufferSetupObject>(br);
            m_vertexSelection = new hclVertexSelectionInput();
            m_vertexSelection.Read(des, br);
            m_updateNormals = br.ReadBoolean();
            m_updateTangents = br.ReadBoolean();
            m_updateBiTangents = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            m_vertexSelection.Write(bw);
            bw.WriteBoolean(m_updateNormals);
            bw.WriteBoolean(m_updateTangents);
            bw.WriteBoolean(m_updateBiTangents);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
