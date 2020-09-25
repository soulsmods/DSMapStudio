using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MutationFlagsEnum
    {
        MUTATION_AABB_CHANGED = 1,
        MUTATION_UPDATE_COLLISION_CACHES = 2,
        MUTATION_REBUILD_COLLISION_CACHES = 4,
    }
    
    public class hknpShapeSignals : IHavokObject
    {
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
