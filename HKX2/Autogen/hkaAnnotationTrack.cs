using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaAnnotationTrack : IHavokObject
    {
        public string m_trackName;
        public List<hkaAnnotationTrackAnnotation> m_annotations;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_trackName = des.ReadStringPointer(br);
            m_annotations = des.ReadClassArray<hkaAnnotationTrackAnnotation>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
