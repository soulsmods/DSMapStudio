using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    /// <summary>
    /// A DS3 and BB file that seems to modify material parameters to light certain static objects. Used to darken objects in shadows for example.
    /// </summary>
    public class BTAB : SoulsFile<BTAB>
    {
        /// <summary>
        /// Entries in this BTAB.
        /// </summary>
        public List<Entry> Entries;

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;

            br.AssertInt32(1);
            br.AssertInt32(0);
            int entryCount = br.ReadInt32();
            int nameSize = br.ReadInt32();
            br.AssertInt32(0);
            // Entry size
            br.AssertInt32(0x28);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);

            long nameStart = br.Position;
            br.Position = nameStart + nameSize;
            Entries = new List<Entry>(entryCount);
            for (int i = 0; i < entryCount; i++)
                Entries.Add(new Entry(br, nameStart));
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;

            bw.WriteInt32(1);
            bw.WriteInt32(0);
            bw.WriteInt32(Entries.Count);
            bw.ReserveInt32("NameSize");
            bw.WriteInt32(0);
            bw.WriteInt32(0x28);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);

            long nameStart = bw.Position;
            var nameOffsets = new List<int>(Entries.Count * 2);
            foreach (Entry entry in Entries)
            {
                int nameOffset = (int)(bw.Position - nameStart);
                nameOffsets.Add(nameOffset);
                bw.WriteUTF16(entry.MSBPartName, true);
                if (nameOffset % 0x10 != 0)
                {
                    for (int i = 0; i < 0x10 - (nameOffset % 0x10); i++)
                        bw.WriteByte(0);
                }

                int nameOffset2 = (int)(bw.Position - nameStart);
                nameOffsets.Add(nameOffset2);
                bw.WriteUTF16(entry.FLVERMaterialName, true);
                if (nameOffset2 % 0x10 != 0)
                {
                    for (int i = 0; i < 0x10 - (nameOffset2 % 0x10); i++)
                        bw.WriteByte(0);
                }
            }

            bw.FillInt32("NameSize", (int)(bw.Position - nameStart));
            for (int i = 0; i < Entries.Count; i++)
                Entries[i].Write(bw, nameOffsets[i * 2], nameOffsets[i * 2 + 1]);
        }

        /// <summary>
        /// A BTAB entry.
        /// </summary>
        public class Entry
        {
            /// <summary>
            /// The name of the target part defined in the MSB file
            /// </summary>
            public string MSBPartName;

            /// <summary>
            /// The name of a material in the FLVER; not the name of the MTD file itself.
            /// </summary>
            public string FLVERMaterialName;

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk1C;

            // These floats are used to control the lighting/material parameters in some way.
            // Seem to be between 0.0-1.0 and sum up to 1.0 in some cases
            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk20, Unk24, Unk28, Unk2C;

            internal Entry(BinaryReaderEx br, long nameStart)
            {
                int nameOffset = br.ReadInt32();
                MSBPartName = br.GetUTF16(nameStart + nameOffset);
                br.AssertInt32(0);

                int nameOffset2 = br.ReadInt32();
                FLVERMaterialName = br.GetUTF16(nameStart + nameOffset2);
                br.AssertInt32(0);

                Unk1C = br.ReadInt32();
                Unk20 = br.ReadSingle();
                Unk24 = br.ReadSingle();
                Unk28 = br.ReadSingle();
                Unk2C = br.ReadSingle();
                br.AssertInt32(0);
            }

            internal void Write(BinaryWriterEx bw, int nameOffset, int nameOffset2)
            {
                bw.WriteInt32(nameOffset);
                bw.WriteInt32(0);
                bw.WriteInt32(nameOffset2);
                bw.WriteInt32(0);
                bw.WriteInt32(Unk1C);
                bw.WriteSingle(Unk20);
                bw.WriteSingle(Unk24);
                bw.WriteSingle(Unk28);
                bw.WriteSingle(Unk2C);
                bw.WriteInt32(0);
            }

            /// <summary>
            /// Returns the MSB part name and FLVER material name of the entry.
            /// </summary>
            public override string ToString()
            {
                return $"{MSBPartName} : {FLVERMaterialName}";
            }
        }
    }
}
