namespace SoulsFormats
{
    public partial class MSB3
    {
        /// <summary>
        /// Different shapes that regions can take.
        /// </summary>
        public enum ShapeType : uint
        {
            /// <summary>
            /// A single point.
            /// </summary>
            Point = 0,

            /// <summary>
            /// A flat circle with a radius.
            /// </summary>
            Circle = 1,

            /// <summary>
            /// A sphere with a radius.
            /// </summary>
            Sphere = 2,

            /// <summary>
            /// A cylinder with a radius and height.
            /// </summary>
            Cylinder = 3,

            /// <summary>
            /// A flat square that is never used and I haven't bothered implementing.
            /// </summary>
            Square = 4,

            /// <summary>
            /// A rectangular prism with width, depth, and height.
            /// </summary>
            Box = 5,
        }

        /// <summary>
        /// A shape taken by a region.
        /// </summary>
        public abstract class Shape
        {
            /// <summary>
            /// The type of this shape.
            /// </summary>
            public abstract ShapeType Type { get; }

            internal abstract Shape Clone();

            internal abstract void Write(BinaryWriterEx bw, long start);

            /// <summary>
            /// A single point.
            /// </summary>
            public class Point : Shape
            {
                /// <summary>
                /// The type of this shape.
                /// </summary>
                public override ShapeType Type => ShapeType.Point;

                /// <summary>
                /// Creates a new Point.
                /// </summary>
                public Point() { }

                internal override Shape Clone()
                {
                    return new Point();
                }

                internal override void Write(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("ShapeDataOffset", 0);
                }
            }

            /// <summary>
            /// A flat circle.
            /// </summary>
            public class Circle : Shape
            {
                /// <summary>
                /// The type of this shape.
                /// </summary>
                public override ShapeType Type => ShapeType.Circle;

                /// <summary>
                /// The radius of the circle.
                /// </summary>
                public float Radius;

                /// <summary>
                /// Creates a new Circle with radius 1.
                /// </summary>
                public Circle() : this(1) { }

                /// <summary>
                /// Creates a new Circle with the given radius.
                /// </summary>
                public Circle(float radius)
                {
                    Radius = radius;
                }

                /// <summary>
                /// Creates a new Circle with the radius of another.
                /// </summary>
                public Circle(Circle clone) : this(clone.Radius) { }

                internal override Shape Clone()
                {
                    return new Circle(this);
                }

                internal Circle(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                }

                internal override void Write(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("ShapeDataOffset", bw.Position - start);
                    bw.WriteSingle(Radius);
                }
            }

            /// <summary>
            /// A volumetric sphere.
            /// </summary>
            public class Sphere : Shape
            {
                /// <summary>
                /// The type of this shape.
                /// </summary>
                public override ShapeType Type => ShapeType.Sphere;

                /// <summary>
                /// The radius of the sphere.
                /// </summary>
                public float Radius;

                /// <summary>
                /// Creates a new Sphere with radius 1.
                /// </summary>
                public Sphere() : this(1) { }

                /// <summary>
                /// Creates a new Sphere with the given radius.
                /// </summary>
                public Sphere(float radius)
                {
                    Radius = radius;
                }

                /// <summary>
                /// Creates a new Sphere with the radius of another.
                /// </summary>
                public Sphere(Sphere clone) : this(clone.Radius) { }

                internal override Shape Clone()
                {
                    return new Sphere(this);
                }

                internal Sphere(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                }

                internal override void Write(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("ShapeDataOffset", bw.Position - start);
                    bw.WriteSingle(Radius);
                }
            }

            /// <summary>
            /// A volumetric cylinder.
            /// </summary>
            public class Cylinder : Shape
            {
                /// <summary>
                /// The type of this shape.
                /// </summary>
                public override ShapeType Type => ShapeType.Cylinder;

                /// <summary>
                /// The radius of the cylinder.
                /// </summary>
                public float Radius;

                /// <summary>
                /// The height of the cylinder.
                /// </summary>
                public float Height;

                /// <summary>
                /// Creates a new Cylinder with radius and height 1.
                /// </summary>
                public Cylinder() : this(1, 1) { }

                /// <summary>
                /// Creates a new Cylinder with the given dimensions.
                /// </summary>
                public Cylinder(float radius, float height)
                {
                    Radius = radius;
                    Height = height;
                }

                /// <summary>
                /// Creates a new Cylinder with the dimensions of another.
                /// </summary>
                public Cylinder(Cylinder clone) : this(clone.Radius, clone.Height) { }

                internal override Shape Clone()
                {
                    return new Cylinder(this);
                }

                internal Cylinder(BinaryReaderEx br)
                {
                    Radius = br.ReadSingle();
                    Height = br.ReadSingle();
                }

                internal override void Write(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("ShapeDataOffset", bw.Position - start);
                    bw.WriteSingle(Radius);
                    bw.WriteSingle(Height);
                }
            }

            /// <summary>
            /// A rectangular prism.
            /// </summary>
            public class Box : Shape
            {
                /// <summary>
                /// The type of this shape.
                /// </summary>
                public override ShapeType Type => ShapeType.Box;

                /// <summary>
                /// The width of the box.
                /// </summary>
                public float Width;

                /// <summary>
                /// The depth of the box.
                /// </summary>
                public float Depth;

                /// <summary>
                /// The height of the box.
                /// </summary>
                public float Height;

                /// <summary>
                /// Creates a new Box with width, depth, and height 1.
                /// </summary>
                public Box() : this(1, 1, 1) { }

                /// <summary>
                /// Creates a new Box with the given dimensions.
                /// </summary>
                public Box(float width, float depth, float height)
                {
                    Width = width;
                    Depth = depth;
                    Height = height;
                }

                /// <summary>
                /// Creates a new Box with the dimensions of another.
                /// </summary>
                public Box(Box clone) : this(clone.Width, clone.Depth, clone.Height) { }

                internal override Shape Clone()
                {
                    return new Box(this);
                }

                internal Box(BinaryReaderEx br)
                {
                    Width = br.ReadSingle();
                    Depth = br.ReadSingle();
                    Height = br.ReadSingle();
                }

                internal override void Write(BinaryWriterEx bw, long start)
                {
                    bw.FillInt64("ShapeDataOffset", bw.Position - start);
                    bw.WriteSingle(Width);
                    bw.WriteSingle(Depth);
                    bw.WriteSingle(Height);
                }
            }
        }
    }
}
