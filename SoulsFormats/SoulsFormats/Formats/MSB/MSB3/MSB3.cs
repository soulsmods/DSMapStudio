using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A map layout format used in DS3.
    /// </summary>
    public partial class MSB3 : SoulsFile<MSB3>, IMsb
    {
        /// <summary>
        /// Models in this MSB.
        /// </summary>
        public ModelParam Models { get; set; }
        IMsbParam<IMsbModel> IMsb.Models => Models;

        /// <summary>
        /// Events in this MSB.
        /// </summary>
        public EventParam Events { get; set; }
        IMsbParam<IMsbEvent> IMsb.Events => Events;

        /// <summary>
        /// Regions in this MSB.
        /// </summary>
        public PointParam Regions { get; set; }
        IMsbParam<IMsbRegion> IMsb.Regions => Regions;

        /// <summary>
        /// Routes in this MSB.
        /// </summary>
        public RouteParam Routes { get; set; }

        /// <summary>
        /// Layers in this MSB.
        /// </summary>
        public LayerParam Layers { get; set; }

        /// <summary>
        /// Parts in this MSB.
        /// </summary>
        public PartsParam Parts { get; set; }
        IMsbParam<IMsbPart> IMsb.Parts => Parts;

        /// <summary>
        /// PartsPose data in this MSB.
        /// </summary>
        public MapstudioPartsPose PartsPoses { get; set; }

        /// <summary>
        /// Bone names in this MSB.
        /// </summary>
        public MapstudioBoneName BoneNames { get; set; }

        /// <summary>
        /// Creates a new MSB3 with all sections empty.
        /// </summary>
        public MSB3()
        {
            Models = new ModelParam();
            Events = new EventParam();
            Regions = new PointParam();
            Routes = new RouteParam();
            Layers = new LayerParam();
            Parts = new PartsParam();
            PartsPoses = new MapstudioPartsPose();
            BoneNames = new MapstudioBoneName();
        }

        /// <summary>
        /// Checks whether the data appears to be a file of this format.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 4)
                return false;

            string magic = br.GetASCII(0, 4);
            return magic == "MSB ";
        }

        internal struct Entries
        {
            public List<Model> Models;
            public List<Event> Events;
            public List<Region> Regions;
            public List<Route> Routes;
            public List<Layer> Layers;
            public List<Part> Parts;
            public List<PartsPose> PartsPoses;
            public List<string> BoneNames;
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
            br.AssertByte(0xFF); // is64BitOffset

            Entries entries = default;
            Models = new ModelParam();
            entries.Models = Models.Read(br);
            Events = new EventParam();
            entries.Events = Events.Read(br);
            Regions = new PointParam();
            entries.Regions = Regions.Read(br);
            Routes = new RouteParam();
            entries.Routes = Routes.Read(br);
            Layers = new LayerParam();
            entries.Layers = Layers.Read(br);
            Parts = new PartsParam();
            entries.Parts = Parts.Read(br);
            PartsPoses = new MapstudioPartsPose();
            entries.PartsPoses = PartsPoses.Read(br);
            BoneNames = new MapstudioBoneName();
            entries.BoneNames = BoneNames.Read(br);

            if (br.Position != 0)
                throw new InvalidDataException("The next param offset of the final param should be 0, but it wasn't.");

            MSB.DisambiguateNames(entries.Models);
            MSB.DisambiguateNames(entries.Parts);
            MSB.DisambiguateNames(entries.Regions);

            foreach (Event evt in entries.Events)
                evt.GetNames(this, entries);
            foreach (Region region in entries.Regions)
                region.GetNames(this, entries);
            foreach (Part part in entries.Parts)
                part.GetNames(this, entries);
            foreach (PartsPose pose in entries.PartsPoses)
                pose.GetNames(this, entries);
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;

            Entries entries;
            entries.Models = Models.GetEntries();
            entries.Events = Events.GetEntries();
            entries.Regions = Regions.GetEntries();
            entries.Routes = Routes.GetEntries();
            entries.Layers = Layers.GetEntries();
            entries.Parts = Parts.GetEntries();
            entries.PartsPoses = PartsPoses.GetEntries();
            entries.BoneNames = BoneNames.GetEntries();

            foreach (Model model in entries.Models)
                model.CountInstances(entries.Parts);
            foreach (Event evt in entries.Events)
                evt.GetIndices(this, entries);
            foreach (Region region in entries.Regions)
                region.GetIndices(this, entries);
            foreach (Part part in entries.Parts)
                part.GetIndices(this, entries);
            foreach (PartsPose pose in entries.PartsPoses)
                pose.GetIndices(this, entries);

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
            Routes.Write(bw, entries.Routes);
            bw.FillInt64("NextParamOffset", bw.Position);
            Layers.Write(bw, entries.Layers);
            bw.FillInt64("NextParamOffset", bw.Position);
            Parts.Write(bw, entries.Parts);
            bw.FillInt64("NextParamOffset", bw.Position);
            PartsPoses.Write(bw, entries.PartsPoses);
            bw.FillInt64("NextParamOffset", bw.Position);
            BoneNames.Write(bw, entries.BoneNames);
            bw.FillInt64("NextParamOffset", 0);
        }

        /// <summary>
        /// A generic MSB section containing a list of entries.
        /// </summary>
        public abstract class Param<T>
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            public int Version { get; set; }

            internal abstract string Type { get; }

            internal Param(int version)
            {
                Version = version;
            }

            /// <summary>
            /// Returns every entry in this section in the order they will be written.
            /// </summary>
            public abstract List<T> GetEntries();

            internal List<T> Read(BinaryReaderEx br)
            {
                Version = br.ReadInt32();
                int offsetCount = br.ReadInt32();
                long nameOffset = br.ReadInt64();
                long[] entryOffsets = br.ReadInt64s(offsetCount - 1);
                long nextParamOffset = br.ReadInt64();

                string type = br.GetUTF16(nameOffset);
                if (type != Type)
                    throw new InvalidDataException($"Expected param \"{Type}\", got param \"{type}\"");

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

            internal void Write(BinaryWriterEx bw, List<T> entries)
            {
                bw.WriteInt32(Version);
                bw.WriteInt32(entries.Count + 1);
                bw.ReserveInt64("ParamNameOffset");
                for (int i = 0; i < entries.Count; i++)
                    bw.ReserveInt64($"EntryOffset{i}");
                bw.ReserveInt64("NextParamOffset");

                bw.FillInt64("ParamNameOffset", bw.Position);
                bw.WriteUTF16(Type, true);
                bw.Pad(8);

                int id = 0;
                Type currentType = null;
                for (int i = 0; i < entries.Count; i++)
                {
                    if (currentType != entries[i].GetType())
                    {
                        currentType = entries[i].GetType();
                        id = 0;
                    }

                    bw.FillInt64($"EntryOffset{i}", bw.Position);
                    WriteEntry(bw, id, entries[i]);
                    id++;
                }
            }

            internal abstract void WriteEntry(BinaryWriterEx bw, int id, T entry);

            /// <summary>
            /// Returns the type string, unknown value and number of entries in this section.
            /// </summary>
            public override string ToString()
            {
                return $"{Type}:{Version}[{GetEntries().Count}]";
            }
        }

        /// <summary>
        /// A generic entry in an MSB section.
        /// </summary>
        public abstract class Entry : IMsbEntry
        {
            /// <summary>
            /// The name of this entry.
            /// </summary>
            public abstract string Name { get; set; }
        }
    }
}
