using System.Collections.Generic;

namespace SoulsFormats
{
    public partial class MSB3
    {
        /// <summary>
        /// A section containing bone name strings. Purpose unknown.
        /// </summary>
        public class MapstudioBoneName : Param<string>
        {
            internal override string Type => "MAPSTUDIO_BONE_NAME_STRING";

            /// <summary>
            /// The bone names in this section.
            /// </summary>
            public List<string> Names;

            /// <summary>
            /// Creates a new BoneNameSection with no bone names.
            /// </summary>
            public MapstudioBoneName(int unk1 = 0) : base(unk1)
            {
                Names = new List<string>();
            }

            /// <summary>
            /// Returns every bone name in the order they will be written.
            /// </summary>
            public override List<string> GetEntries()
            {
                return Names;
            }

            internal override string ReadEntry(BinaryReaderEx br)
            {
                var name = br.ReadUTF16();
                Names.Add(name);
                return name;
            }

            internal override void WriteEntry(BinaryWriterEx bw, int id, string entry)
            {
                bw.WriteUTF16(entry, true);
                bw.Pad(8);
            }
        }
    }
}
