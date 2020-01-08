using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB1
    {
        /// <summary>
        /// A collection of points and trigger volumes used by scripts and events.
        /// </summary>
        public class PointParam : Param<Region>, IMsbParam<IMsbRegion>
        {
            internal override string Name => "POINT_PARAM_ST";

            /// <summary>
            /// All regions in the map.
            /// </summary>
            public List<Region> Regions { get; set; }

            /// <summary>
            /// Creates an empty PointParam.
            /// </summary>
            public PointParam() : base()
            {
                Regions = new List<Region>();
            }

            /// <summary>
            /// Returns the list of regions.
            /// </summary>
            public override List<Region> GetEntries()
            {
                return Regions;
            }
            IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries() => GetEntries();

            internal override Region ReadEntry(BinaryReaderEx br)
            {
                var region = new Region(br);
                Regions.Add(region);
                return region;
            }

            public void Add(IMsbRegion item)
            {
                switch (item)
                {
                    case Region r:
                        Regions.Add(r);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }
        }

        /// <summary>
        /// A point or volume used by scripts or events.
        /// </summary>
        public class Region : Entry, IMsbRegion
        {
            /// <summary>
            /// Describes the physical shape of the region.
            /// </summary>
            public MSB.Shape Shape { get; set; }

            /// <summary>
            /// Location of the region.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Rotation of the region, in degrees.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// Identifies the region in external files.
            /// </summary>
            public int EntityID { get; set; }

            /// <summary>
            /// Creates a Region with default values.
            /// </summary>
            public Region()
            {
                Name = "";
                Shape = new MSB.Shape.Point();
                EntityID = -1;
            }

            internal Region(BinaryReaderEx br)
            {
                long start = br.Position;
                int nameOffset = br.ReadInt32();
                br.AssertInt32(0);
                br.ReadInt32(); // ID
                ShapeType shapeType = br.ReadEnum32<ShapeType>();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                int unkOffsetA = br.ReadInt32();
                int unkOffsetB = br.ReadInt32();
                int shapeDataOffset = br.ReadInt32();
                int entityDataOffset = br.ReadInt32();
                br.AssertInt32(0);

                Name = br.GetShiftJIS(start + nameOffset);

                br.Position = start + unkOffsetA;
                br.AssertInt32(0);
                br.Position = start + unkOffsetB;
                br.AssertInt32(0);

                br.Position = start + shapeDataOffset;
                switch (shapeType)
                {
                    case ShapeType.Point:
                        Shape = new MSB.Shape.Point();
                        break;

                    case ShapeType.Circle:
                        Shape = new MSB.Shape.Circle(br);
                        break;

                    case ShapeType.Sphere:
                        Shape = new MSB.Shape.Sphere(br);
                        break;

                    case ShapeType.Cylinder:
                        Shape = new MSB.Shape.Cylinder(br);
                        break;

                    case ShapeType.Rect:
                        Shape = new MSB.Shape.Rect(br);
                        break;

                    case ShapeType.Box:
                        Shape = new MSB.Shape.Box(br);
                        break;

                    default:
                        throw new NotImplementedException($"Unimplemented shape type: {shapeType}");
                }

                br.Position = start + entityDataOffset;
                EntityID = br.ReadInt32();
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;
                bw.ReserveInt32("NameOffset");
                bw.WriteInt32(0);
                bw.WriteInt32(id);
                bw.WriteUInt32((uint)Shape.Type);
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.ReserveInt32("UnkOffsetA");
                bw.ReserveInt32("UnkOffsetB");
                bw.ReserveInt32("ShapeDataOffset");
                bw.ReserveInt32("EntityDataOffset");
                bw.WriteInt32(0);

                bw.FillInt32("NameOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(MSB.ReambiguateName(Name), true);
                bw.Pad(4);

                bw.FillInt32("UnkOffsetA", (int)(bw.Position - start));
                bw.WriteInt32(0);
                bw.FillInt32("UnkOffsetB", (int)(bw.Position - start));
                bw.WriteInt32(0);

                if (Shape.HasShapeData)
                {
                    bw.FillInt32("ShapeDataOffset", (int)(bw.Position - start));
                    Shape.WriteShapeData(bw);
                }
                else
                {
                    bw.FillInt32("ShapeDataOffset", 0);
                }

                bw.FillInt32("EntityDataOffset", (int)(bw.Position - start));
                bw.WriteInt32(EntityID);
            }
        }
    }
}
