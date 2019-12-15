using System;

namespace SoulsFormats
{
    public partial class MSBS
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum ShapeType : uint
        {
            Point = 0,
            Circle = 1,
            Sphere = 2,
            Cylinder = 3,
            Rect = 4,
            Box = 5,
            Composite = 6,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// The shape of a map region.
        /// </summary>
        public abstract class Shape
        {
            internal abstract ShapeType Type { get; }

            internal abstract bool HasShapeData { get; }

            internal virtual void WriteShapeData(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Shape data should not be written for shapes with no shape data.");
            }

            /// <summary>
            /// A single point.
            /// </summary>
            public class Point : Shape
            {
                internal override ShapeType Type => ShapeType.Point;

                internal override bool HasShapeData => false;
            }

            /// <summary>
            /// A flat circle.
            /// </summary>
            public class Circle : Shape
            {
                internal override ShapeType Type => ShapeType.Circle;

                internal override bool HasShapeData => true;

                /// <summary>
                /// The radius of the circle.
                /// </summary>
                public float Radius { get; set; }

                /// <summary>
                /// Creates a Circle with default dimensions.
                /// </summary>
                public Circle() { }

                internal Circle(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                }

                internal override void WriteShapeData(BinaryWriterEx bw)
                {
                    bw.WriteSingle(Radius);
                }
            }

            /// <summary>
            /// A volumetric sphere.
            /// </summary>
            public class Sphere : Shape
            {
                internal override ShapeType Type => ShapeType.Sphere;

                internal override bool HasShapeData => true;

                /// <summary>
                /// The radius of the sphere.
                /// </summary>
                public float Radius { get; set; }

                /// <summary>
                /// Creates a Sphere with default dimensions.
                /// </summary>
                public Sphere() { }

                internal Sphere(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                }

                internal override void WriteShapeData(BinaryWriterEx bw)
                {
                    bw.WriteSingle(Radius);
                }
            }

            /// <summary>
            /// A volumetric cylinder.
            /// </summary>
            public class Cylinder : Shape
            {
                internal override ShapeType Type => ShapeType.Cylinder;

                internal override bool HasShapeData => true;

                /// <summary>
                /// The radius of the cylinder.
                /// </summary>
                public float Radius { get; set; }

                /// <summary>
                /// The height of the cylinder.
                /// </summary>
                public float Height { get; set; }

                /// <summary>
                /// Creates a Cylinder with default dimensions.
                /// </summary>
                public Cylinder() { }

                internal Cylinder(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                    Height = br.ReadSingle();
                }

                internal override void WriteShapeData(BinaryWriterEx bw)
                {
                    bw.WriteSingle(Radius);
                    bw.WriteSingle(Height);
                }
            }

            /// <summary>
            /// A flat rectangle.
            /// </summary>
            public class Rect : Shape
            {
                internal override ShapeType Type => ShapeType.Rect;

                internal override bool HasShapeData => true;

                /// <summary>
                /// The width of the rectangle.
                /// </summary>
                public float Width { get; set; }

                /// <summary>
                /// The depth of the rectangle.
                /// </summary>
                public float Depth { get; set; }

                /// <summary>
                /// Creates a Rect with default dimensions.
                /// </summary>
                public Rect() { }

                internal Rect(BinaryReaderEx br)
                {
                    Width = br.ReadSingle();
                    Depth = br.ReadSingle();
                }

                internal override void WriteShapeData(BinaryWriterEx bw)
                {
                    bw.WriteSingle(Width);
                    bw.WriteSingle(Depth);
                }
            }

            /// <summary>
            /// A rectangular prism.
            /// </summary>
            public class Box : Shape
            {
                internal override ShapeType Type => ShapeType.Box;

                internal override bool HasShapeData => true;

                /// <summary>
                /// The width of the box.
                /// </summary>
                public float Width { get; set; }

                /// <summary>
                /// The depth of the box.
                /// </summary>
                public float Depth { get; set; }

                /// <summary>
                /// The height of the box.
                /// </summary>
                public float Height { get; set; }

                /// <summary>
                /// Creates a Box with default dimensions.
                /// </summary>
                public Box() { }

                internal Box(BinaryReaderEx br)
                {
                    Width = br.ReadSingle();
                    Depth = br.ReadSingle();
                    Height = br.ReadSingle();
                }

                internal override void WriteShapeData(BinaryWriterEx bw)
                {
                    bw.WriteSingle(Width);
                    bw.WriteSingle(Depth);
                    bw.WriteSingle(Height);
                }
            }

            /// <summary>
            /// A shape composed of references to other regions' shapes.
            /// </summary>
            public class Composite : Shape
            {
                internal override ShapeType Type => ShapeType.Composite;

                internal override bool HasShapeData => true;

                /// <summary>
                /// Other regions referenced by this shape.
                /// </summary>
                public Child[] Children { get; private set; }

                /// <summary>
                /// Creates a Composite with 8 empty references.
                /// </summary>
                public Composite()
                {
                    Children = new Child[8];
                    for (int i = 0; i < 8; i++)
                        Children[i] = new Child();
                }

                internal Composite(BinaryReaderEx br)
                {
                    Children = new Child[8];
                    for (int i = 0; i < 8; i++)
                        Children[i] = new Child(br);
                }

                internal override void WriteShapeData(BinaryWriterEx bw)
                {
                    for (int i = 0; i < 8; i++)
                        Children[i].Write(bw);
                }

                /// <summary>
                /// A reference to another region.
                /// </summary>
                public class Child
                {
                    /// <summary>
                    /// The name of the child region.
                    /// </summary>
                    public string RegionName { get; set; }
                    private int RegionIndex;

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public int Unk04 { get; set; }

                    /// <summary>
                    /// Creates a Child with default values.
                    /// </summary>
                    public Child() { }

                    internal Child(BinaryReaderEx br)
                    {
                        RegionIndex = br.ReadInt32();
                        Unk04 = br.ReadInt32();
                    }

                    internal void Write(BinaryWriterEx bw)
                    {
                        bw.WriteInt32(RegionIndex);
                        bw.WriteInt32(Unk04);
                    }

                    internal void GetNames(Entries entries)
                    {
                        RegionName = MSB.FindName(entries.Regions, RegionIndex);
                    }

                    internal void GetIndices(Entries entries)
                    {
                        RegionIndex = MSB.FindIndex(entries.Regions, RegionName);
                    }
                }
            }
        }
    }
}
