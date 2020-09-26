using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Runtime.InteropServices;

namespace HKX2
{
    internal class HKXHeader
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

    internal class LocalFixup
    {
        public uint Src;
        public uint Dst;

        internal LocalFixup(BinaryReaderEx br)
        {
            Src = br.ReadUInt32();
            Dst = br.ReadUInt32();
        }
    }

    internal class GlobalFixup
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

    internal class VirtualFixup
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

    public class HKXClassName
    {
        public uint Signature;
        public string ClassName;
        public uint SectionOffset;

        internal HKXClassName(BinaryReaderEx br)
        {
            Signature = br.ReadUInt32();
            br.AssertByte(0x09); // Seems random but ok
            SectionOffset = (uint)br.Position;
            ClassName = br.ReadASCII();
        }

        public void Write(BinaryWriterEx bw, uint sectionBaseOffset)
        {
            bw.WriteUInt32(Signature);
            bw.WriteByte(0x09);
            bw.WriteASCII(ClassName, true);
        }
    }

    // Class names data found in the __classnames__ section of the hkx
    internal class HKXClassNames
    {
        public List<HKXClassName> ClassNames;
        public Dictionary<uint, HKXClassName> OffsetClassNamesMap;

        public void Read(PackFileDeserializer hkx, HKXSection section, BinaryReaderEx br, HKX.HKXVariation variation)
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

    internal class HKXSection
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

        public Dictionary<uint, LocalFixup> _localMap = new Dictionary<uint, LocalFixup>();
        public Dictionary<uint, GlobalFixup> _globalMap = new Dictionary<uint, GlobalFixup>();
        public Dictionary<uint, VirtualFixup> _virtualMap = new Dictionary<uint, VirtualFixup>();

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
                    var f = new LocalFixup(br);
                    _localMap.Add(f.Src, f);
                    LocalFixups.Add(f);
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
                    var f = new GlobalFixup(br);
                    _globalMap.Add(f.Src, f);
                    GlobalFixups.Add(f);
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
                    var f = new VirtualFixup(br);
                    _virtualMap.Add(f.Src, f);
                    VirtualFixups.Add(f);
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
        internal HKXClassNames ReadClassnames(PackFileDeserializer hkx)
        {
            BinaryReaderEx br = new BinaryReaderEx(false, SectionData);
            var classnames = new HKXClassNames();
            classnames.Read(hkx, this, br, HKX.HKXVariation.HKXDS3);
            return classnames;
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
        internal HKXHeader _header;
        internal HKXSection _classSection;
        internal HKXSection _typeSection;
        internal HKXSection _dataSection;

        internal HKXClassNames _classnames;

        internal HKX.HKXVariation _variation;

        internal Dictionary<uint, IHavokObject> _deserializedObjects;

        public List<T> ReadClassArray<T>(BinaryReaderEx br) where T : IHavokObject, new()
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            var res = new List<T>();
            if (size > 0)
            {
                // Do a local fixup lookup
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    var cls = new T();
                    cls.Read(this, br);
                    res.Add(cls);
                }
                br.StepOut();
            }
            return res;
        }

        public T ReadClassPointer<T>(BinaryReaderEx br) where T : IHavokObject
        {
            // Do a global fixup lookup
            if (!_dataSection._globalMap.ContainsKey((uint)br.Position))
            {
                br.AssertUInt64(0);
                return default(T);
            }
            var f = _dataSection._globalMap[(uint)br.Position];
            // Consume pointer
            br.AssertUInt64(0);
            return (T)ConstructVirtualClass(br, f.Dst);
        }

        public List<T> ReadClassPointerArray<T>(BinaryReaderEx br) where T : IHavokObject
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            // Do a local fixup lookup
            var res = new List<T>();
            if (size > 0)
            {
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    res.Add(ReadClassPointer<T>(br));
                }
                br.StepOut();
            }
            return res;
        }

        public string ReadStringPointer(BinaryReaderEx br)
        {
            // Do a local fixup lookup
            if (!_dataSection._localMap.ContainsKey((uint)br.Position))
            {
                br.AssertUInt64(0);
                return null;
            }
            var f = _dataSection._localMap[(uint)br.Position];

            // Consume pointer
            br.AssertUInt64(0);
            br.StepIn(f.Dst);
            var ret = br.ReadASCII();
            br.StepOut();
            return ret;
        }

        public List<string> ReadStringPointerArray(BinaryReaderEx br)
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            var res = new List<string>();
            if (size > 0)
            {
                // Do a local fixup lookup
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    res.Add(ReadStringPointer(br));
                }
                br.StepOut();
            }
            return res;
        }

        public List<byte> ReadByteArray(BinaryReaderEx br)
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            var res = new List<byte>();
            if (size > 0)
            {
                // Do a local fixup lookup
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    res.Add(br.ReadByte());
                }
                br.StepOut();
            }
            return res;
        }

        public List<sbyte> ReadSByteArray(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public List<ushort> ReadUInt16Array(BinaryReaderEx br)
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            var res = new List<ushort>();
            if (size > 0)
            {
                // Do a local fixup lookup
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    res.Add(br.ReadUInt16());
                }
                br.StepOut();
            }
            return res;
        }

        public List<short> ReadInt16Array(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public List<uint> ReadUInt32Array(BinaryReaderEx br)
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            var res = new List<uint>();
            if (size > 0)
            {
                // Do a local fixup lookup
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    res.Add(br.ReadUInt32());
                }
                br.StepOut();
            }
            return res;
        }

        public List<int> ReadInt32Array(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public List<ulong> ReadUInt64Array(BinaryReaderEx br)
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            var res = new List<ulong>();
            if (size > 0)
            {
                // Do a local fixup lookup
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    res.Add(br.ReadUInt64());
                }
                br.StepOut();
            }
            return res;
        }

        public List<long> ReadInt64Array(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public List<float> ReadSingleArray(BinaryReaderEx br)
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            var res = new List<float>();
            if (size > 0)
            {
                // Do a local fixup lookup
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    res.Add(br.ReadSingle());
                }
                br.StepOut();
            }
            return res;
        }

        public List<bool> ReadBooleanArray(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public Vector4 ReadVector4(BinaryReaderEx br)
        {
            return br.ReadVector4();
        }

        public List<Vector4> ReadVector4Array(BinaryReaderEx br)
        {
            // Consume pointer
            br.AssertUInt64(0);
            uint size = br.ReadUInt32();
            br.ReadUInt32(); // Capacity and flags
            var res = new List<Vector4>();
            if (size > 0)
            {
                // Do a local fixup lookup
                var f = _dataSection._localMap[(uint)br.Position - 16];
                br.StepIn(f.Dst);
                for (int i = 0; i < size; i++)
                {
                    res.Add(ReadVector4(br));
                }
                br.StepOut();
            }
            return res;
        }

        public Matrix4x4 ReadMatrix3(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public List<Matrix4x4> ReadMatrix3Array(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public Matrix4x4 ReadMatrix4(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public List<Matrix4x4> ReadMatrix4Array(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public Matrix4x4 ReadTransform(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public List<Matrix4x4> ReadTransformArray(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public Matrix4x4 ReadQSTransform(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public List<Matrix4x4> ReadQSTransformArray(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public Quaternion ReadQuaternion(BinaryReaderEx br)
        {
            return new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        public List<Quaternion> ReadQuaternionArray(BinaryReaderEx br)
        {
            throw new NotImplementedException();
        }

        public IHavokObject ConstructVirtualClass(BinaryReaderEx br, uint offset)
        {
            if (_deserializedObjects.ContainsKey(offset))
            {
                return _deserializedObjects[offset];
            }
            var fixup = _dataSection._virtualMap[offset];
            var cname = _classnames.OffsetClassNamesMap[fixup.NameOffset].ClassName;
            Type klasst = Type.GetType($@"HKX2.{cname}");
            var ret = (IHavokObject)Activator.CreateInstance(klasst);
            br.StepIn(offset);
            ret.Read(this, br);
            br.StepOut();
            _deserializedObjects.Add(offset, ret);
            return ret;
        }

        public void Deserialize(BinaryReaderEx br)
        {
            br.BigEndian = false;

            // Peek ahead and read the endian byte
            br.StepIn(0x11);
            br.BigEndian = (br.ReadByte() == 0x0) ? true : false;
            br.StepOut();

            // Read header
            _header = new HKXHeader();
            _header.Magic0 = br.AssertUInt32(0x57E0E057);
            _header.Magic1 = br.AssertUInt32(0x10C0C010);
            //Header.UserTag = br.AssertInt32(0);
            _header.UserTag = br.ReadInt32();
            _header.Version = br.AssertInt32(0x05, 0x08, 0x0B);
            if (_header.Version == 0x05)
            {
                _variation = HKX.HKXVariation.HKXDeS;
            }
            else if (_header.Version == 0x08)
            {
                _variation = HKX.HKXVariation.HKXDS1;
            }
            else
            {
                _variation = HKX.HKXVariation.HKXDS3;
            }
            _header.PointerSize = br.AssertByte(4, 8);
            _header.Endian = br.AssertByte(0, 1);
            _header.PaddingOption = br.AssertByte(0, 1);
            _header.BaseClass = br.AssertByte(1); // ?
            _header.SectionCount = br.AssertInt32(3); // Always 3 sections pretty sure
            _header.ContentsSectionIndex = br.ReadInt32();
            _header.ContentsSectionOffset = br.ReadInt32();
            _header.ContentsClassNameSectionIndex = br.ReadInt32();
            _header.ContentsClassNameSectionOffset = br.ReadInt32();
            _header.ContentsVersionString = br.ReadFixStr(16); // Should be hk_2014.1.0-r1
            _header.Flags = br.ReadInt32();

            // Later versions of Havok have an extended header
            if (_header.Version >= 0x0B)
            {
                _header.Unk3C = br.ReadInt16();
                _header.SectionOffset = br.ReadInt16();
                _header.Unk40 = br.ReadUInt32();
                _header.Unk44 = br.ReadUInt32();
                _header.Unk48 = br.ReadUInt32();
                _header.Unk4C = br.ReadUInt32();

                // Read the 3 sections in the file
                br.Position = _header.SectionOffset + 0x40;
            }
            else
            {
                // Just padding
                br.AssertUInt32(0xFFFFFFFF);
            }

            _classSection = new HKXSection(br, _variation);
            _classSection.SectionID = 0;
            _typeSection = new HKXSection(br, _variation);
            _typeSection.SectionID = 1;
            _dataSection = new HKXSection(br, _variation);
            _dataSection.SectionID = 2;

            // Process the class names
            _classnames = _classSection.ReadClassnames(this);

            // Deserialize the objects
            _deserializedObjects = new Dictionary<uint, IHavokObject>();
            BinaryReaderEx br2 = new BinaryReaderEx((_header.Endian == 0) ? true : false, _dataSection.SectionData);
            var root = ConstructVirtualClass(br2, 0);
        }
    }
}
