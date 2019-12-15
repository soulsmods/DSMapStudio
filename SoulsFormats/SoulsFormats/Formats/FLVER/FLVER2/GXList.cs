using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    public partial class FLVER2
    {
        /// <summary>
        /// A collection of items that set various material properties.
        /// </summary>
        public class GXList : List<GXItem>
        {
            /// <summary>
            /// The length in bytes of the terminator data block; most likely not important, but varies in original files.
            /// </summary>
            public int TerminatorLength { get; set; }

            /// <summary>
            /// Creates an empty GXList.
            /// </summary>
            public GXList() : base() { }

            internal GXList(BinaryReaderEx br, FLVERHeader header) : base()
            {
                while (br.GetInt32(br.Position) != int.MaxValue)
                    Add(new GXItem(br, header));

                br.AssertInt32(int.MaxValue);
                br.AssertInt32(100);
                TerminatorLength = br.ReadInt32() - 0xC;
                br.AssertPattern(TerminatorLength, 0x00);
            }

            internal void Write(BinaryWriterEx bw, FLVERHeader header)
            {
                foreach (GXItem item in this)
                    item.Write(bw, header);

                bw.WriteInt32(int.MaxValue);
                bw.WriteInt32(100);
                bw.WriteInt32(TerminatorLength + 0xC);
                bw.WritePattern(TerminatorLength, 0x00);
            }
        }

        /// <summary>
        /// Rendering parameters used by materials.
        /// </summary>
        public class GXItem
        {
            /// <summary>
            /// In DS2, ID is just a number; in other games, it's 4 ASCII characters.
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// Unknown; typically 100.
            /// </summary>
            public int Unk04 { get; set; }

            /// <summary>
            /// Raw parameter data, usually just a bunch of floats.
            /// </summary>
            public byte[] Data { get; set; }

            /// <summary>
            /// Creates a GXItem with default values.
            /// </summary>
            public GXItem()
            {
                ID = "0";
                Unk04 = 100;
                Data = new byte[0];
            }

            /// <summary>
            /// Creates a GXItem with the given values.
            /// </summary>
            public GXItem(string id, int unk04, byte[] data)
            {
                ID = id;
                Unk04 = unk04;
                Data = data;
            }

            internal GXItem(BinaryReaderEx br, FLVERHeader header)
            {
                if (header.Version <= 0x20010)
                {
                    ID = br.ReadInt32().ToString();
                }
                else
                {
                    ID = br.ReadFixStr(4);
                }
                Unk04 = br.ReadInt32();
                int length = br.ReadInt32();
                Data = br.ReadBytes(length - 0xC);
            }

            internal void Write(BinaryWriterEx bw, FLVERHeader header)
            {
                if (header.Version <= 0x20010)
                {
                    if (int.TryParse(ID, out int id))
                        bw.WriteInt32(id);
                    else
                        throw new FormatException("For Dark Souls 2, GX IDs must be convertible to int.");
                }
                else
                {
                    bw.WriteFixStr(ID, 4);
                }
                bw.WriteInt32(Unk04);
                bw.WriteInt32(Data.Length + 0xC);
                bw.WriteBytes(Data);
            }
        }
    }
}
