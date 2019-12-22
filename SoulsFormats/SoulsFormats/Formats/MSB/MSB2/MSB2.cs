using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A map layout file used in DS2: SotFS. Extension: .msb
    /// </summary>
    public partial class MSB2 : SoulsFile<MSB2>, IMsb
    {
        /// <summary>
        /// Model files available for parts to use.
        /// </summary>
        public ModelParam Models { get; set; }
        IMsbParam<IMsbModel> IMsb.Models => Models;

        /// <summary>
        /// Abstract entities that set map properties or control behaviors.
        /// </summary>
        public EventParam Events { get; set; }
        IMsbParam<IMsbEvent> IMsb.Events => Events;

        /// <summary>
        /// Points or volumes that trigger certain behaviors.
        /// </summary>
        public PointParam Regions { get; set; }
        IMsbParam<IMsbRegion> IMsb.Regions => Regions;

        /// <summary>
        /// Concrete entities in the map.
        /// </summary>
        public PartsParam Parts { get; set; }
        IMsbParam<IMsbPart> IMsb.Parts => Parts;

        /// <summary>
        /// Predetermined poses applied to objects such as corpses.
        /// </summary>
        public List<PartPose> PartPoses { get; set; }

        /// <summary>
        /// Creates an empty MSB2.
        /// </summary>
        public MSB2()
        {
            Models = new ModelParam();
            Events = new EventParam();
            Regions = new PointParam();
            Parts = new PartsParam();
            PartPoses = new List<PartPose>();
        }

        /// <summary>
        /// Checks whether the data appears to be a file of this format.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 0x14)
                return false;

            br.BigEndian = false;
            string magic = br.GetASCII(0, 4);
            int modelVersion = br.GetInt32(0x10);
            return magic == "MSB " && modelVersion == 5;
        }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("MSB ");
            br.AssertInt32(1);
            br.AssertInt32(0x10);
            br.AssertBoolean(false); // isBigEndian
            br.AssertBoolean(false); // isBitBigEndian
            br.AssertByte(1); // textEncoding
            br.AssertSByte(-1); // is64BitOffset

            Entries entries;
            Models = new ModelParam();
            entries.Models = Models.Read(br);
            Events = new EventParam();
            entries.Events = Events.Read(br);
            Regions = new PointParam();
            entries.Regions = Regions.Read(br);
            new RouteParam().Read(br);
            new LayerParam().Read(br);
            Parts = new PartsParam();
            entries.Parts = Parts.Read(br);
            PartPoses = new MapstudioPartsPose().Read(br);
            entries.BoneNames = new MapstudioBoneName().Read(br);

            if (br.Position != 0)
                throw new InvalidDataException($"The next param offset of the final param should be 0, but it was 0x{br.Position:X}.");

            MSB.DisambiguateNames(entries.Models);
            MSB.DisambiguateNames(entries.Parts);
            MSB.DisambiguateNames(entries.BoneNames);

            foreach (Part part in entries.Parts)
                part.GetNames(this, entries);
            foreach (PartPose pose in PartPoses)
                pose.GetNames(entries);
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            Entries entries;
            entries.Models = Models.GetEntries();
            entries.Events = Events.GetEntries();
            entries.Regions = Regions.GetEntries();
            entries.Parts = Parts.GetEntries();
            entries.BoneNames = new List<BoneName>();

            Lookups lookups;
            lookups.Models = MakeNameLookup(entries.Models);
            lookups.Parts = MakeNameLookup(entries.Parts);
            lookups.Collisions = MakeNameLookup(Parts.Collisions);
            lookups.BoneNames = new Dictionary<string, int>();

            Models.DiscriminateModels();
            foreach (Part part in entries.Parts)
                part.GetIndices(lookups);
            foreach (PartPose pose in PartPoses)
                pose.GetIndices(lookups, entries);

            bw.BigEndian = false;
            bw.WriteASCII("MSB ");
            bw.WriteInt32(1);
            bw.WriteInt32(0x10);
            bw.WriteBoolean(false);
            bw.WriteBoolean(false);
            bw.WriteByte(1);
            bw.WriteByte(0xFF);

            Models.Write(bw, entries.Models);
            bw.FillInt64("NextParamOffset", bw.Position);
            Events.Write(bw, entries.Events);
            bw.FillInt64("NextParamOffset", bw.Position);
            Regions.Write(bw, entries.Regions);
            bw.FillInt64("NextParamOffset", bw.Position);
            new RouteParam().Write(bw, new List<Entry>());
            bw.FillInt64("NextParamOffset", bw.Position);
            new LayerParam().Write(bw, new List<Entry>());
            bw.FillInt64("NextParamOffset", bw.Position);
            Parts.Write(bw, entries.Parts);
            bw.FillInt64("NextParamOffset", bw.Position);
            new MapstudioPartsPose().Write(bw, PartPoses);
            bw.FillInt64("NextParamOffset", bw.Position);
            new MapstudioBoneName().Write(bw, entries.BoneNames);
            bw.FillInt64("NextParamOffset", 0);
        }

        internal struct Entries
        {
            public List<Model> Models;
            public List<Event> Events;
            public List<Region> Regions;
            public List<Part> Parts;
            public List<BoneName> BoneNames;
        }

        internal struct Lookups
        {
            public Dictionary<string, int> Models;
            public Dictionary<string, int> Parts;
            public Dictionary<string, int> Collisions;
            public Dictionary<string, int> BoneNames;
        }

        /// <summary>
        /// A generic entry in an MSB param.
        /// </summary>
        public abstract class Entry
        {
            internal abstract void Write(BinaryWriterEx bw, int index);
        }

        /// <summary>
        /// A generic entry in an MSB param that has a name.
        /// </summary>
        public abstract class NamedEntry : Entry, IMsbEntry
        {
            /// <summary>
            /// The name of this entry; should generally be unique.
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// A collection of entries in the MSB that share common properties.
        /// </summary>
        public abstract class Param<T> where T : Entry
        {
            internal abstract string Name { get; }
            internal abstract int Version { get; }

            internal List<T> Read(BinaryReaderEx br)
            {
                br.AssertInt32(Version);
                int offsetCount = br.ReadInt32();
                long nameOffset = br.ReadInt64();
                long[] entryOffsets = br.ReadInt64s(offsetCount - 1);
                long nextParamOffset = br.ReadInt64();

                string name = br.GetUTF16(nameOffset);
                if (name != Name)
                    throw new InvalidDataException($"Expected param \"{Name}\", got param \"{name}\"");

                var entries = new List<T>(offsetCount - 1);
                foreach (long offset in entryOffsets)
                {
                    br.Position = offset;
                    entries.Add(ReadEntry(br));
                }
                br.Position = nextParamOffset;
                return entries;
            }

            internal abstract T ReadEntry(BinaryReaderEx br);

            internal virtual void Write(BinaryWriterEx bw, List<T> entries)
            {
                bw.WriteInt32(Version);
                bw.WriteInt32(entries.Count + 1);
                bw.ReserveInt64("ParamNameOffset");
                for (int i = 0; i < entries.Count; i++)
                    bw.ReserveInt64($"EntryOffset{i}");
                bw.ReserveInt64("NextParamOffset");

                bw.FillInt64("ParamNameOffset", bw.Position);
                bw.WriteUTF16(Name, true);
                bw.Pad(8);

                int index = 0;
                Type type = null;
                for (int i = 0; i < entries.Count; i++)
                {
                    if (type != entries[i].GetType())
                    {
                        type = entries[i].GetType();
                        index = 0;
                    }

                    bw.FillInt64($"EntryOffset{i}", bw.Position);
                    entries[i].Write(bw, index);
                    bw.Pad(8);
                    index++;
                }
            }

            /// <summary>
            /// Returns every entry in the order they'll be written.
            /// </summary>
            public abstract List<T> GetEntries();
        }

        private static int FindIndex(Dictionary<string, int> lookup, string name)
        {
            if (name == null)
            {
                return -1;
            }
            else
            {
                if (!lookup.ContainsKey(name))
                    throw new KeyNotFoundException($"Name not found: {name}");
                return lookup[name];
            }
        }

        private static Dictionary<string, int> MakeNameLookup<T>(List<T> list) where T : NamedEntry
        {
            var lookup = new Dictionary<string, int>();
            for (int i = 0; i < list.Count; i++)
                lookup[list[i].Name] = i;
            return lookup;
        }
    }
}
