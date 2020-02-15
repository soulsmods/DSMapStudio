using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
    partial class HKX2
    {
        private class HKXHeader
        {
            public uint Magic0;
            public uint Magic1;
            public int UserTag;
            public int Version;
            public byte PointerSize;
            public byte Endian;
            public byte PaddingOption;
            public byte BaseClass;
            public int SectionCount;
            public int ContentsSectionIndex;
            public int ContentsSectionOffset;
            public int ContentsClassNameSectionIndex;
            public int ContentsClassNameSectionOffset;
            public string ContentsVersionString;
            public int Flags;
            public short Unk3C;
            public short SectionOffset;
            public uint Unk40;
            public uint Unk44;
            public uint Unk48;
            public uint Unk4C;
        }

        private class LocalFixup
        {
            public uint Src;
            public uint Dst;

            internal LocalFixup(BinaryReaderEx br)
            {
                Src = br.ReadUInt32();
                Dst = br.ReadUInt32();
            }
        }

        private class GlobalFixup
        {
            public uint Src;
            public uint DstSectionIndex;
            public uint Dst;

            internal GlobalFixup(BinaryReaderEx br)
            {
                Src = br.ReadUInt32();
                DstSectionIndex = br.ReadUInt32();
                Dst = br.ReadUInt32();
            }
        }

        private class VirtualFixup
        {
            public uint Src;
            public uint SectionIndex;
            public uint NameOffset;

            internal VirtualFixup(BinaryReaderEx br)
            {
                Src = br.ReadUInt32();
                SectionIndex = br.ReadUInt32();
                NameOffset = br.ReadUInt32();
            }
        }

        // Class names data found in the __classnames__ section of the hkx
        private class HKXClassNames
        {
            public List<HKXClassName> ClassNames;
            public Dictionary<uint, HKXClassName> OffsetClassNamesMap;

            public void Read(HKX hkx, HKXSection section, BinaryReaderEx br, HKX.HKXVariation variation)
            {
                ClassNames = new List<HKXClassName>();
                OffsetClassNamesMap = new Dictionary<uint, HKXClassName>();
                while (br.ReadUInt16() != 0xFFFF)
                {
                    br.Position -= 2;
                    uint stringStart = (uint)br.Position + 5;
                    var className = new HKXClassName(br);
                    ClassNames.Add(className);
                    OffsetClassNamesMap.Add(stringStart, className);
                }
            }

            public void Write(HKX hkx, HKXSection section, BinaryWriterEx bw, uint sectionBaseOffset, HKX.HKXVariation variation)
            {
                foreach (var cls in ClassNames)
                {
                    cls.Write(bw, sectionBaseOffset);
                }
                while ((bw.Position % 16) != 0)
                {
                    // Write padding bytes to 16 byte align
                    bw.WriteByte(0xFF);
                }
            }

            public HKXClassName Lookup(uint offset)
            {
                return OffsetClassNamesMap[offset];
            }
        }

        private class HKXSection
        {
            public int SectionID;

            public string SectionTag;
            public uint AbsoluteDataStart;
            public uint LocalFixupsOffset;
            public uint GlobalFixupsOffset;
            public uint VirtualFixupsOffset;
            public uint ExportsOffset;
            public uint ImportsOffset;
            public uint EndOffset;

            public List<LocalFixup> LocalFixups;
            public List<GlobalFixup> GlobalFixups;
            public List<VirtualFixup> VirtualFixups;

            public byte[] SectionData;

            internal HKXSection(BinaryReaderEx br, HKX.HKXVariation variation)
            {
                SectionTag = br.ReadFixStr(19);
                br.AssertByte(0xFF);
                AbsoluteDataStart = br.ReadUInt32();
                LocalFixupsOffset = br.ReadUInt32();
                GlobalFixupsOffset = br.ReadUInt32();
                VirtualFixupsOffset = br.ReadUInt32();
                ExportsOffset = br.ReadUInt32();
                ImportsOffset = br.ReadUInt32();
                EndOffset = br.ReadUInt32();

                // Read Data
                br.StepIn(AbsoluteDataStart);
                SectionData = br.ReadBytes((int)LocalFixupsOffset);
                br.StepOut();

                // Local fixups
                LocalFixups = new List<LocalFixup>();
                br.StepIn(AbsoluteDataStart + LocalFixupsOffset);
                for (int i = 0; i < (GlobalFixupsOffset - LocalFixupsOffset) / 8; i++)
                {
                    if (br.ReadUInt32() != 0xFFFFFFFF)
                    {
                        br.Position -= 4;
                        LocalFixups.Add(new LocalFixup(br));
                    }
                }
                br.StepOut();

                // Global fixups
                GlobalFixups = new List<GlobalFixup>();
                br.StepIn(AbsoluteDataStart + GlobalFixupsOffset);
                for (int i = 0; i < (VirtualFixupsOffset - GlobalFixupsOffset) / 12; i++)
                {
                    if (br.ReadUInt32() != 0xFFFFFFFF)
                    {
                        br.Position -= 4;
                        GlobalFixups.Add(new GlobalFixup(br));
                    }
                }
                br.StepOut();

                // Virtual fixups
                VirtualFixups = new List<VirtualFixup>();
                br.StepIn(AbsoluteDataStart + VirtualFixupsOffset);
                for (int i = 0; i < (ExportsOffset - VirtualFixupsOffset) / 12; i++)
                {
                    if (br.ReadUInt32() != 0xFFFFFFFF)
                    {
                        br.Position -= 4;
                        VirtualFixups.Add(new VirtualFixup(br));
                    }
                }
                br.StepOut();

                if (variation == HKX.HKXVariation.HKXBloodBorne || variation == HKX.HKXVariation.HKXDS3)
                {
                    br.AssertUInt32(0xFFFFFFFF);
                    br.AssertUInt32(0xFFFFFFFF);
                    br.AssertUInt32(0xFFFFFFFF);
                    br.AssertUInt32(0xFFFFFFFF);
                }
            }

            public void WriteHeader(BinaryWriterEx bw, HKX.HKXVariation variation)
            {
                bw.WriteFixStr(SectionTag, 19);
                bw.WriteByte(0xFF);
                bw.ReserveUInt32("absoffset" + SectionID);
                bw.ReserveUInt32("locoffset" + SectionID);
                bw.ReserveUInt32("globoffset" + SectionID);
                bw.ReserveUInt32("virtoffset" + SectionID);
                bw.ReserveUInt32("expoffset" + SectionID);
                bw.ReserveUInt32("impoffset" + SectionID);
                bw.ReserveUInt32("endoffset" + SectionID);
                if (variation == HKX.HKXVariation.HKXBloodBorne || variation == HKX.HKXVariation.HKXDS3)
                {
                    bw.WriteUInt32(0xFFFFFFFF);
                    bw.WriteUInt32(0xFFFFFFFF);
                    bw.WriteUInt32(0xFFFFFFFF);
                    bw.WriteUInt32(0xFFFFFFFF);
                }
            }

            public void WriteData(BinaryWriterEx bw, HKX hkx, HKX.HKXVariation variation)
            {
                /*uint absoluteOffset = (uint)bw.Position;
                bw.FillUInt32("absoffset" + SectionID, absoluteOffset);
                foreach (var obj in Objects)
                {
                    obj.Write(hkx, this, bw, absoluteOffset, variation);
                }

                // Local fixups
                bw.FillUInt32("locoffset" + SectionID, (uint)bw.Position - absoluteOffset);
                foreach (var loc in LocalReferences)
                {
                    loc.Write(bw);
                }
                while ((bw.Position % 16) != 0)
                {
                    bw.WriteByte(0xFF); // 16 byte align
                }

                // Global fixups
                bw.FillUInt32("globoffset" + SectionID, (uint)bw.Position - absoluteOffset);
                foreach (var glob in GlobalReferences)
                {
                    glob.Write(bw);
                }
                while ((bw.Position % 16) != 0)
                {
                    bw.WriteByte(0xFF); // 16 byte align
                }

                // Virtual fixups
                bw.FillUInt32("virtoffset" + SectionID, (uint)bw.Position - absoluteOffset);
                foreach (var virt in VirtualReferences)
                {
                    virt.Write(bw);
                }
                while ((bw.Position % 16) != 0)
                {
                    bw.WriteByte(0xFF); // 16 byte align
                }

                bw.FillUInt32("expoffset" + SectionID, (uint)bw.Position - absoluteOffset);
                bw.FillUInt32("impoffset" + SectionID, (uint)bw.Position - absoluteOffset);
                bw.FillUInt32("endoffset" + SectionID, (uint)bw.Position - absoluteOffset);*/
            }

            // Only use for a classnames structure after preliminary deserialization
            internal void ReadClassnames(HKX hkx)
            {
                BinaryReaderEx br = new BinaryReaderEx(false, SectionData);
                var classnames = new HKXClassNames();
                classnames.Read(hkx, this, br, HKX.HKXVariation.HKXDS3);
            }


            /// <summary>
            /// Allocates a heap for the data in this section, copies the data, and
            /// patches the pointers. This assumes a 64-bit havok file on a 64-bit system
            /// </summary>
            internal unsafe IntPtr AllocateAndPatchPointers64()
            {
                IntPtr data = Marshal.AllocHGlobal(SectionData.Length);
                Marshal.Copy(SectionData, 0, data, SectionData.Length);

                // Use references to patch pointers
                byte* start = (byte*)data.ToPointer();
                foreach (var loc in LocalFixups)
                {
                    byte** patchoffset = (byte**)(start + loc.Src);
                    *patchoffset = start + loc.Dst;
                }

                byte* startg = (byte*)data.ToPointer();
                foreach (var glob in GlobalFixups)
                {
                    byte** patchoffset = (byte**)(startg + glob.Src);
                    *patchoffset = start + glob.Dst;
                }

                return data;
            }
        }

        public class PackFileDeserializer
        {
            public static void Deserialize(HKX2 hkx, BinaryReaderEx br)
            {
                
            }
        }
    }
}
