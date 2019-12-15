using System.Collections.Generic;

namespace SoulsFormats
{
    public partial class MSB3
    {
        /// <summary>
        /// A section containing layers, which probably don't actually do anything.
        /// </summary>
        public class LayerParam : Param<Layer>
        {
            internal override string Type => "LAYER_PARAM_ST";

            /// <summary>
            /// The layers in this section.
            /// </summary>
            public List<Layer> Layers;

            /// <summary>
            /// Creates a new LayerParam with no layers.
            /// </summary>
            public LayerParam(int unk1 = 3) : base(unk1)
            {
                Layers = new List<Layer>();
            }

            /// <summary>
            /// Returns every layer in the order they will be written.
            /// </summary>
            public override List<Layer> GetEntries()
            {
                return Layers;
            }

            internal override Layer ReadEntry(BinaryReaderEx br)
            {
                var layer = new Layer(br);
                Layers.Add(layer);
                return layer;
            }

            internal override void WriteEntry(BinaryWriterEx bw, int index, Layer entry)
            {
                entry.Write(bw);
            }
        }

        /// <summary>
        /// Unknown; seems to have been related to ceremonies but probably unused in release.
        /// </summary>
        public class Layer
        {
            /// <summary>
            /// The name of this layer.
            /// </summary>
            public string Name;

            /// <summary>
            /// Unknown; usually just counts up from 0.
            /// </summary>
            public int Unk08, Unk0C;

            /// <summary>
            /// Unknown; seems to always be 0.
            /// </summary>
            public int Unk10;

            /// <summary>
            /// Creates a new Layer with default values.
            /// </summary>
            public Layer()
            {
                Name = "";
            }

            internal Layer(BinaryReaderEx br)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                Unk08 = br.ReadInt32();
                Unk0C = br.ReadInt32();
                Unk10 = br.ReadInt32();

                Name = br.GetUTF16(start + nameOffset);
            }

            internal void Write(BinaryWriterEx bw)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteInt32(Unk08);
                bw.WriteInt32(Unk0C);
                bw.WriteInt32(Unk10);

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(Name, true);
                bw.Pad(8);
            }

            /// <summary>
            /// Returns the name and three values of this layer.
            /// </summary>
            public override string ToString()
            {
                return $"{Name} ({Unk08}, {Unk0C}, {Unk10})";
            }
        }
    }
}
