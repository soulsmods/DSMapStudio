using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A UI configuration format used in DS1.
    /// </summary>
    public class DRB
    {
        /// <summary>
        /// If true, Shapes have scaling information.
        /// </summary>
        public bool DSR { get; set; }

        /// <summary>
        /// Available UI textures for reference; not actually necessary for textures to work.
        /// </summary>
        public List<Texture> Textures { get; set; }

        /// <summary>
        /// Unknown data used by Aniks.
        /// </summary>
        public byte[] AnipBytes { get; set; }

        /// <summary>
        /// Unknown data used by Aniks.
        /// </summary>
        public byte[] IntpBytes { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public List<Anim> Anims { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public List<Scdl> Scdls { get; set; }

        /// <summary>
        /// Groups of UI elements.
        /// </summary>
        public List<Dlg> Dlgs { get; set; }

        private const int ANIK_SIZE = 0x20;
        private const int ANIO_SIZE = 0x10;
        private const int SCDK_SIZE = 0x20;
        private const int SCDO_SIZE = 0x10;
        private const int DLGO_SIZE = 0x20;

        private static bool Is(BinaryReaderEx br)
        {
            if (br.Length < 4)
                return false;

            string magic = br.GetASCII(0, 4);
            return magic == "DRB\0";
        }

        private void Read(BinaryReaderEx br, bool dsr)
        {
            br.BigEndian = false;
            DSR = dsr;

            ReadNullBlock(br, "DRB\0");
            Dictionary<int, string> strings = ReadSTR(br);
            Textures = ReadTEXI(br, strings);
            long shprStart = ReadBlobBlock(br, "SHPR");
            long ctprStart = ReadBlobBlock(br, "CTPR");
            AnipBytes = ReadBlobBytes(br, "ANIP");
            IntpBytes = ReadBlobBytes(br, "INTP");
            long scdpStart = ReadBlobBlock(br, "SCDP");
            Dictionary<int, Shape> shapes = ReadSHAP(br, dsr, strings, shprStart);
            Dictionary<int, Control> controls = ReadCTRL(br, strings, ctprStart);
            Dictionary<int, Anik> aniks = ReadANIK(br, strings);
            Dictionary<int, Anio> anios = ReadANIO(br, aniks);
            Anims = ReadANIM(br, strings, anios);
            Dictionary<int, Scdk> scdks = ReadSCDK(br, strings, scdpStart);
            Dictionary<int, Scdo> scdos = ReadSCDO(br, strings, scdks);
            Scdls = ReadSCDL(br, strings, scdos);
            Dictionary<int, Dlgo> dlgos = ReadDLGO(br, strings, shapes, controls);
            Dlgs = ReadDLG(br, strings, shapes, controls, dlgos);
            ReadNullBlock(br, "END\0");

            void CheckShape(Shape shape)
            {
                if (shape is Shape.Dialog dialog)
                {
                    if (dialog.DlgIndex != -1)
                        dialog.Dlg = Dlgs[dialog.DlgIndex];
                }
            }

            foreach (Dlg dlg in Dlgs)
            {
                CheckShape(dlg.Shape);
                foreach (Dlgo dlgo in dlg.Dlgos)
                    CheckShape(dlgo.Shape);
            }
        }

        private void Write(BinaryWriterEx bw)
        {
            void CheckShape(Shape shape)
            {
                if (shape is Shape.Dialog dialog)
                {
                    if (dialog.Dlg == null)
                        dialog.DlgIndex = -1;
                    else if (Dlgs.Contains(dialog.Dlg))
                        dialog.DlgIndex = (short)Dlgs.IndexOf(dialog.Dlg);
                    else
                        throw new InvalidDataException($"Dlg \"{dialog.Dlg.Name}\" is referenced but no found in Dlgs list.");
                }
            }

            foreach (Dlg dlg in Dlgs)
            {
                CheckShape(dlg.Shape);
                foreach (Dlgo dlgo in dlg.Dlgos)
                    CheckShape(dlgo.Shape);
            }

            bw.BigEndian = false;

            WriteNullBlock(bw, "DRB\0");
            Dictionary<string, int> stringOffsets = WriteSTR(bw);
            WriteTEXI(bw, stringOffsets);
            Queue<int> shprOffsets = WriteSHPR(bw, DSR, stringOffsets);
            Queue<int> ctprOffsets = WriteCTPR(bw);
            WriteBlobBytes(bw, "ANIP", AnipBytes);
            WriteBlobBytes(bw, "INTP", IntpBytes);
            Queue<int> scdpOffsets = WriteSCDP(bw);
            Queue<int> shapOffsets = WriteSHAP(bw, stringOffsets, shprOffsets);
            Queue<int> ctrlOffsets = WriteCTRL(bw, stringOffsets, ctprOffsets);
            Queue<int> anikOffsets = WriteANIK(bw, stringOffsets);
            Queue<int> anioOffsets = WriteANIO(bw, anikOffsets);
            WriteANIM(bw, stringOffsets, anioOffsets);
            Queue<int> scdkOffsets = WriteSCDK(bw, stringOffsets, scdpOffsets);
            Queue<int> scdoOffsets = WriteSCDO(bw, stringOffsets, scdkOffsets);
            WriteSCDL(bw, stringOffsets, scdoOffsets);
            Queue<int> dlgoOffsets = WriteDLGO(bw, stringOffsets, shapOffsets, ctrlOffsets);
            WriteDLG(bw, stringOffsets, shapOffsets, ctrlOffsets, dlgoOffsets);
            WriteNullBlock(bw, "END\0");
        }

        /// <summary>
        /// Returns the element group with the given name, or null if not found.
        /// </summary>
        public Dlg this[string name] => Dlgs.Find(dlg => dlg.Name == name);

        /// <summary>
        /// A texture available to be referenced by UI elements.
        /// </summary>
        public class Texture
        {
            /// <summary>
            /// A friendly name for the texture.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The network path to the texture.
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// Creates a Texture with default values.
            /// </summary>
            public Texture()
            {
                Name = "";
                Path = "";
            }

            /// <summary>
            /// Creates a Texture with the given name and path.
            /// </summary>
            public Texture(string name, string path)
            {
                Name = name;
                Path = path;
            }

            internal Texture(BinaryReaderEx br, Dictionary<int, string> strings)
            {
                int nameOffset = br.ReadInt32();
                int pathOffset = br.ReadInt32();
                br.AssertInt32(0);
                br.AssertInt32(0);

                Name = strings[nameOffset];
                Path = strings[pathOffset];
            }

            internal void Write(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
            {
                bw.WriteInt32(stringOffsets[Name]);
                bw.WriteInt32(stringOffsets[Path]);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }

            /// <summary>
            /// Returns the name and path of the texture.
            /// </summary>
            public override string ToString()
            {
                return $"{Name} - {Path}";
            }
        }

        /// <summary>
        /// Possible types of a UI element.
        /// </summary>
        public enum ShapeType
        {
            /// <summary>
            /// References another element group attached to this element.
            /// </summary>
            Dialog,

            /// <summary>
            /// A rectangle with color interpolated between each corner.
            /// </summary>
            GouraudRect,

            /// <summary>
            /// Presumably a sprite with interpolated coloring.
            /// </summary>
            GouraudSprite,

            /// <summary>
            /// Unknown.
            /// </summary>
            Mask,

            /// <summary>
            /// Unknown.
            /// </summary>
            MonoFrame,

            /// <summary>
            /// A rectangle with a solid color.
            /// </summary>
            MonoRect,

            /// <summary>
            /// An invisible element used to mark a position.
            /// </summary>
            Null,

            /// <summary>
            /// A scrolling text field.
            /// </summary>
            ScrollText,

            /// <summary>
            /// Displays a region of a texture.
            /// </summary>
            Sprite,

            /// <summary>
            /// A fixed text field.
            /// </summary>
            Text
        }

        /// <summary>
        /// Describes the appearance of a UI element.
        /// </summary>
        public abstract class Shape
        {
            /// <summary>
            /// The type of this element.
            /// </summary>
            public abstract ShapeType Type { get; }

            /// <summary>
            /// Left bound of this element, relative to 1280x720.
            /// </summary>
            public short LeftEdge { get; set; }

            /// <summary>
            /// Top bound of this element, relative to 1280x720.
            /// </summary>
            public short TopEdge { get; set; }

            /// <summary>
            /// Right bound of this element, relative to 1280x720.
            /// </summary>
            public short RightEdge { get; set; }

            /// <summary>
            /// Bottom bound of this element, relative to 1280x720.
            /// </summary>
            public short BottomEdge { get; set; }

            /// <summary>
            /// For DSR, the X coordinate which the element scales relative to.
            /// </summary>
            public short ScalingOriginX { get; set; }

            /// <summary>
            /// For DSR, the Y coordinate which the element scales relative to.
            /// </summary>
            public short ScalingOriginY { get; set; }

            /// <summary>
            /// For DSR, the behavior of scaling for this element.
            /// </summary>
            public int ScalingType { get; set; }

            internal Shape()
            {
                ScalingOriginX = -1;
                ScalingOriginY = -1;
            }

            internal static Shape Read(BinaryReaderEx br, bool dsr, Dictionary<int, string> strings, long shprStart)
            {
                int typeOffset = br.ReadInt32();
                int shprOffset = br.ReadInt32();
                string type = strings[typeOffset];

                Shape result;
                br.StepIn(shprStart + shprOffset);
                {
                    if (type == "Dialog")
                        result = new Dialog(br, dsr);
                    else if (type == "GouraudRect")
                        result = new GouraudRect(br, dsr);
                    else if (type == "GouraudSprite")
                        result = new GouraudSprite(br, dsr);
                    else if (type == "Mask")
                        result = new Mask(br, dsr);
                    else if (type == "MonoFrame")
                        result = new MonoFrame(br, dsr);
                    else if (type == "MonoRect")
                        result = new MonoRect(br, dsr);
                    else if (type == "Null")
                        result = new Null(br, dsr);
                    else if (type == "ScrollText")
                        result = new ScrollText(br, dsr, strings);
                    else if (type == "Sprite")
                        result = new Sprite(br, dsr);
                    else if (type == "Text")
                        result = new Text(br, dsr, strings);
                    else
                        throw new InvalidDataException($"Unknown shape type: {type}");
                }
                br.StepOut();
                return result;
            }

            internal Shape(BinaryReaderEx br, bool dsr)
            {
                LeftEdge = br.ReadInt16();
                TopEdge = br.ReadInt16();
                RightEdge = br.ReadInt16();
                BottomEdge = br.ReadInt16();

                if (dsr && Type != ShapeType.Null)
                {
                    ScalingOriginX = br.ReadInt16();
                    ScalingOriginY = br.ReadInt16();
                    ScalingType = br.ReadInt32();
                }
                else
                {
                    ScalingOriginX = -1;
                    ScalingOriginY = -1;
                    ScalingType = 0;
                }
            }

            internal void WriteData(BinaryWriterEx bw, bool dsr, Dictionary<string, int> stringOffsets)
            {
                bw.WriteInt16(LeftEdge);
                bw.WriteInt16(TopEdge);
                bw.WriteInt16(RightEdge);
                bw.WriteInt16(BottomEdge);

                if (dsr && Type != ShapeType.Null)
                {
                    bw.WriteInt16(ScalingOriginX);
                    bw.WriteInt16(ScalingOriginY);
                    bw.WriteInt32(ScalingType);
                }

                WriteSpecific(bw, stringOffsets);
            }

            internal abstract void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets);

            internal void WriteHeader(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> shprOffsets)
            {
                bw.WriteInt32(stringOffsets[Type.ToString()]);
                bw.WriteInt32(shprOffsets.Dequeue());
            }

            /// <summary>
            /// Returns the type and bounds of this Shape.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} ({LeftEdge}, {TopEdge}) ({RightEdge}, {BottomEdge})";
            }

            /// <summary>
            /// References another element group attached to this element.
            /// </summary>
            public class Dialog : Shape
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.Dialog;

                /// <summary>
                /// Dlg referenced by this element; must be found in the DRB's Dlg list.
                /// </summary>
                public Dlg Dlg { get; set; }
                internal short DlgIndex;

                /// <summary>
                /// Unknown; always 0 or 1.
                /// </summary>
                public byte Unk02 { get; set; }

                /// <summary>
                /// Unknown; always 1.
                /// </summary>
                public byte Unk03 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Creates a Dialog with default values.
                /// </summary>
                public Dialog()
                {
                    DlgIndex = -1;
                    Unk03 = 1;
                }

                internal Dialog(BinaryReaderEx br, bool dsr) : base(br, dsr)
                {
                    DlgIndex = br.ReadInt16();
                    Unk02 = br.ReadByte();
                    Unk03 = br.ReadByte();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    bw.WriteInt16(DlgIndex);
                    bw.WriteByte(Unk02);
                    bw.WriteByte(Unk03);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                }
            }

            /// <summary>
            /// A rectangle with color interpolated between each corner.
            /// </summary>
            public class GouraudRect : Shape
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.GouraudRect;

                /// <summary>
                /// Unknown; always 1.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// The color of the top left corner of the rectangle.
                /// </summary>
                public Color TopLeftColor { get; set; }

                /// <summary>
                /// The color of the top right corner of the rectangle.
                /// </summary>
                public Color TopRightColor { get; set; }

                /// <summary>
                /// The color of the bottom right corner of the rectangle.
                /// </summary>
                public Color BottomRightColor { get; set; }

                /// <summary>
                /// The color of the bottom left corner of the rectangle.
                /// </summary>
                public Color BottomLeftColor { get; set; }

                /// <summary>
                /// Creates a GouraudRect with default values.
                /// </summary>
                public GouraudRect() : base()
                {
                    Unk00 = 1;
                }

                internal GouraudRect(BinaryReaderEx br, bool dsr) : base(br, dsr)
                {
                    Unk00 = br.ReadInt32();
                    TopLeftColor = ReadABGR(br);
                    TopRightColor = ReadABGR(br);
                    BottomRightColor = ReadABGR(br);
                    BottomLeftColor = ReadABGR(br);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    bw.WriteInt32(Unk00);
                    WriteABGR(bw, TopLeftColor);
                    WriteABGR(bw, TopRightColor);
                    WriteABGR(bw, BottomRightColor);
                    WriteABGR(bw, BottomLeftColor);
                }
            }

            /// <summary>
            /// Presumably a sprite with interpolated coloring.
            /// </summary>
            public class GouraudSprite : Shape
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.GouraudSprite;

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown; always -1.
                /// </summary>
                public short Unk08 { get; set; }

                /// <summary>
                /// Unknown; always 256.
                /// </summary>
                public short Unk0A { get; set; }

                /// <summary>
                /// The color of the top left corner of the sprite.
                /// </summary>
                public Color TopLeftColor { get; set; }

                /// <summary>
                /// The color of the top right corner of the sprite.
                /// </summary>
                public Color TopRightColor { get; set; }

                /// <summary>
                /// The color of the bottom right corner of the sprite.
                /// </summary>
                public Color BottomRightColor { get; set; }

                /// <summary>
                /// The color of the bottom left corner of the sprite.
                /// </summary>
                public Color BottomLeftColor { get; set; }

                /// <summary>
                /// Creates a GouraudSprite with default values.
                /// </summary>
                public GouraudSprite()
                {
                    Unk08 = -1;
                    Unk0A = 256;
                }

                internal GouraudSprite(BinaryReaderEx br, bool dsr) : base(br, dsr)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt16();
                    Unk0A = br.ReadInt16();
                    TopLeftColor = ReadABGR(br);
                    TopRightColor = ReadABGR(br);
                    BottomRightColor = ReadABGR(br);
                    BottomLeftColor = ReadABGR(br);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt16(Unk08);
                    bw.WriteInt16(Unk0A);
                    WriteABGR(bw, TopLeftColor);
                    WriteABGR(bw, TopRightColor);
                    WriteABGR(bw, BottomRightColor);
                    WriteABGR(bw, BottomLeftColor);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Mask : Shape
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.Mask;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown; always 1.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public byte Unk0C { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public byte Unk0D { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public byte Unk0E { get; set; }

                /// <summary>
                /// Creates a Mask with default values.
                /// </summary>
                public Mask() : base()
                {
                    Unk04 = 1;
                }

                internal Mask(BinaryReaderEx br, bool dsr) : base(br, dsr)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadByte();
                    Unk0D = br.ReadByte();
                    Unk0E = br.ReadByte();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteByte(Unk0C);
                    bw.WriteByte(Unk0D);
                    bw.WriteByte(Unk0E);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class MonoFrame : Shape
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.MonoFrame;

                /// <summary>
                /// Unknown; always 1.
                /// </summary>
                public byte Unk00 { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public byte Unk01 { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public byte Unk02 { get; set; }

                /// <summary>
                /// Unknown; always 1-3.
                /// </summary>
                public byte Unk03 { get; set; }

                /// <summary>
                /// Unknown; always 0-7.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown; possibly a color.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Creates a MonoFrame with default values.
                /// </summary>
                public MonoFrame() : base()
                {
                    Unk00 = 1;
                    Unk03 = 1;
                }

                internal MonoFrame(BinaryReaderEx br, bool dsr) : base(br, dsr)
                {
                    Unk00 = br.ReadByte();
                    Unk01 = br.ReadByte();
                    Unk02 = br.ReadByte();
                    Unk03 = br.ReadByte();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    bw.WriteByte(Unk00);
                    bw.WriteByte(Unk01);
                    bw.WriteByte(Unk02);
                    bw.WriteByte(Unk03);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                }
            }

            /// <summary>
            /// A rectangle with a solid color.
            /// </summary>
            public class MonoRect : Shape
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.MonoRect;

                /// <summary>
                /// Unknown; always 1.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Chooses a color from a palette of 1-80, or 0 to use a custom color.
                /// </summary>
                public int PaletteColor { get; set; }

                /// <summary>
                /// When PaletteColor is 0, specifies the color of the rectangle.
                /// </summary>
                public Color CustomColor { get; set; }

                /// <summary>
                /// Creates a MonoRect with default values.
                /// </summary>
                public MonoRect() : base()
                {
                    Unk00 = 1;
                }

                internal MonoRect(BinaryReaderEx br, bool dsr) : base(br, dsr)
                {
                    Unk00 = br.ReadInt32();
                    PaletteColor = br.ReadInt32();
                    CustomColor = ReadABGR(br);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(PaletteColor);
                    WriteABGR(bw, CustomColor);
                }
            }

            /// <summary>
            /// An invisible element used to mark a position.
            /// </summary>
            public class Null : Shape
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.Null;

                /// <summary>
                /// Creates a Null with default values.
                /// </summary>
                public Null() : base() { }

                internal Null(BinaryReaderEx br, bool dsr) : base(br, dsr) { }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets) { }
            }

            /// <summary>
            /// A scrolling text field.
            /// </summary>
            public class ScrollText : TextBase
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.ScrollText;

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk1C { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk20 { get; set; }

                /// <summary>
                /// Unknown; always 15 or 100.
                /// </summary>
                public int Unk24 { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk28 { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public short Unk2C { get; set; }

                /// <summary>
                /// Creates a ScrollText with default values.
                /// </summary>
                public ScrollText() : base()
                {
                    Unk24 = 15;
                }

                internal ScrollText(BinaryReaderEx br, bool dsr, Dictionary<int, string> strings) : base(br, dsr, strings)
                {
                    Unk1C = br.ReadInt32();
                    Unk20 = br.ReadInt32();
                    Unk24 = br.ReadInt32();
                    Unk28 = br.ReadInt32();
                    Unk2C = br.ReadInt16();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    base.WriteSpecific(bw, stringOffsets);
                    bw.WriteInt32(Unk1C);
                    bw.WriteInt32(Unk20);
                    bw.WriteInt32(Unk24);
                    bw.WriteInt32(Unk28);
                    bw.WriteInt16(Unk2C);
                }
            }

            /// <summary>
            /// Indicates the display properties of a Sprite.
            /// </summary>
            [Flags]
            public enum SpriteFlags : ushort
            {
                /// <summary>
                /// No modification.
                /// </summary>
                None = 0,

                /// <summary>
                /// Rotate texture 90 degrees clockwise.
                /// </summary>
                RotateCW = 0x10,

                /// <summary>
                /// Rotate texture 180 degrees.
                /// </summary>
                Rotate180 = 0x20,

                /// <summary>
                /// Flip texture vertically.
                /// </summary>
                FlipVertical = 0x40,

                /// <summary>
                /// Flip texture horizontally.
                /// </summary>
                FlipHorizontal = 0x80,

                /// <summary>
                /// Allow alpha transparency.
                /// </summary>
                Alpha = 0x100,

                /// <summary>
                /// Overlay the texture on the underlying visual.
                /// </summary>
                Overlay = 0x200,
            }

            /// <summary>
            /// Displays a region of a texture.
            /// </summary>
            public class Sprite : Shape
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.Sprite;

                /// <summary>
                /// Left bound of the texture region displayed by this element.
                /// </summary>
                public short TexLeftEdge { get; set; }

                /// <summary>
                /// Top bound of the texture region displayed by this element.
                /// </summary>
                public short TexTopEdge { get; set; }

                /// <summary>
                /// Right bound of the texture region displayed by this element.
                /// </summary>
                public short TexRightEdge { get; set; }

                /// <summary>
                /// Bottom bound of the texture region displayed by this element.
                /// </summary>
                public short TexBottomEdge { get; set; }

                /// <summary>
                /// The texture to display, indexing textures in menu.tpf.
                /// </summary>
                public short TextureIndex { get; set; }

                /// <summary>
                /// Flags modifying how the texture is displayed.
                /// </summary>
                public SpriteFlags Flags { get; set; }

                /// <summary>
                /// Tints the sprite from a palette of 1-80, or 0 to use custom color below.
                /// </summary>
                public int PaletteColor { get; set; }

                /// <summary>
                /// Tints the sprite a certain color.
                /// </summary>
                public Color CustomColor { get; set; }

                /// <summary>
                /// Creates a Sprite with default values.
                /// </summary>
                public Sprite() : base()
                {
                    Flags = SpriteFlags.Alpha;
                    CustomColor = Color.White;
                }

                internal Sprite(BinaryReaderEx br, bool dsr) : base(br, dsr)
                {
                    TexLeftEdge = br.ReadInt16();
                    TexTopEdge = br.ReadInt16();
                    TexRightEdge = br.ReadInt16();
                    TexBottomEdge = br.ReadInt16();
                    TextureIndex = br.ReadInt16();
                    Flags = (SpriteFlags)br.ReadUInt16();
                    PaletteColor = br.ReadInt32();
                    CustomColor = ReadABGR(br);
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    bw.WriteInt16(TexLeftEdge);
                    bw.WriteInt16(TexTopEdge);
                    bw.WriteInt16(TexRightEdge);
                    bw.WriteInt16(TexBottomEdge);
                    bw.WriteInt16(TextureIndex);
                    bw.WriteUInt16((ushort)Flags);
                    bw.WriteInt32(PaletteColor);
                    WriteABGR(bw, CustomColor);
                }
            }

            /// <summary>
            /// Indicates the positioning of text within its element.
            /// </summary>
            [Flags]
            public enum AlignFlags : byte
            {
                /// <summary>
                /// Anchor to top left.
                /// </summary>
                TopLeft = 0,

                /// <summary>
                /// Anchor to right side.
                /// </summary>
                Right = 1,

                /// <summary>
                /// Center horizontally.
                /// </summary>
                CenterHorizontal = 2,

                /// <summary>
                /// Anchor to bottom side.
                /// </summary>
                Bottom = 4,

                /// <summary>
                /// Center vertically.
                /// </summary>
                CenterVertical = 8
            }

            /// <summary>
            /// Indicates the source of a text element's text.
            /// </summary>
            public enum TxtType : byte
            {
                /// <summary>
                /// Text is a literal value stored in the DRB.
                /// </summary>
                Literal = 0,

                /// <summary>
                /// Text is a static FMG ID.
                /// </summary>
                FMG = 1,

                /// <summary>
                /// Text is assigned at runtime.
                /// </summary>
                Dynamic = 2
            }

            /// <summary>
            /// A fixed text field.
            /// </summary>
            public class Text : TextBase
            {
                /// <summary>
                /// The type of this element.
                /// </summary>
                public override ShapeType Type => ShapeType.Text;

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk1C { get; set; }

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public short Unk20 { get; set; }

                /// <summary>
                /// Creates a Text with default values.
                /// </summary>
                public Text() : base() { }

                internal Text(BinaryReaderEx br, bool dsr, Dictionary<int, string> strings) : base(br, dsr, strings)
                {
                    Unk1C = br.ReadInt32();
                    Unk20 = br.ReadInt16();
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    base.WriteSpecific(bw, stringOffsets);
                    bw.WriteInt32(Unk1C);
                    bw.WriteInt16(Unk20);
                }
            }

            /// <summary>
            /// Either a fixed text or scrolling text element.
            /// </summary>
            public abstract class TextBase : Shape
            {
                /// <summary>
                /// Unknown; always 0-2.
                /// </summary>
                public byte Unk00 { get; set; }

                /// <summary>
                /// Unknown; always 1.
                /// </summary>
                public byte Unk01 { get; set; }

                /// <summary>
                /// Distance between each line of text.
                /// </summary>
                public short LineSpacing { get; set; }

                /// <summary>
                /// Chooses a color from a palette of 1-80, or 0 to use a custom color.
                /// </summary>
                public int PaletteColor { get; set; }

                /// <summary>
                /// When PaletteColor is 0, specifies the color of the text.
                /// </summary>
                public Color CustomColor { get; set; }

                /// <summary>
                /// From 0-11, different sizes of the menu font. 12 is the subtitle font.
                /// </summary>
                public short FontSize { get; set; }

                /// <summary>
                /// The horizontal and vertical alignment of the text.
                /// </summary>
                public AlignFlags Alignment { get; set; }

                /// <summary>
                /// Whether the element uses a text literal, a static FMG ID, or is assigned at runtime.
                /// </summary>
                public TxtType TextType { get; set; }

                /// <summary>
                /// Unknown; always 0x1C.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// The maximum characters to display.
                /// </summary>
                public int CharLength { get; set; }

                /// <summary>
                /// If TextType is Literal, the text to display, otherwise null.
                /// </summary>
                public string TextLiteral { get; set; }

                /// <summary>
                /// If TextType is FMG, the FMG ID to display, otherwise -1.
                /// </summary>
                public int TextID { get; set; }

                internal TextBase() : base()
                {
                    Unk01 = 1;
                    TextType = TxtType.FMG;
                    Unk10 = 0x1C;
                    TextID = -1;
                }

                internal TextBase(BinaryReaderEx br, bool dsr, Dictionary<int, string> strings) : base(br, dsr)
                {
                    Unk00 = br.ReadByte();
                    Unk01 = br.ReadByte();
                    LineSpacing = br.ReadInt16();
                    PaletteColor = br.ReadInt32();
                    CustomColor = ReadABGR(br);
                    FontSize = br.ReadInt16();
                    Alignment = (AlignFlags)br.ReadByte();
                    TextType = br.ReadEnum8<TxtType>();
                    Unk10 = br.ReadInt32();

                    if (TextType == TxtType.Literal)
                    {
                        int textOffset = br.ReadInt32();
                        TextLiteral = strings[textOffset];
                        CharLength = -1;
                        TextID = -1;
                    }
                    else if (TextType == TxtType.FMG)
                    {
                        CharLength = br.ReadInt32();
                        TextID = br.ReadInt32();
                        TextLiteral = null;
                    }
                    else if (TextType == TxtType.Dynamic)
                    {
                        CharLength = br.ReadInt32();
                        TextLiteral = null;
                        TextID = -1;
                    }
                }

                internal override void WriteSpecific(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
                {
                    bw.WriteByte(Unk00);
                    bw.WriteByte(Unk01);
                    bw.WriteInt16(LineSpacing);
                    bw.WriteInt32(PaletteColor);
                    WriteABGR(bw, CustomColor);
                    bw.WriteInt16(FontSize);
                    bw.WriteByte((byte)Alignment);
                    bw.WriteByte((byte)TextType);
                    bw.WriteInt32(Unk10);

                    if (TextType == TxtType.Literal)
                    {
                        bw.WriteInt32(stringOffsets[TextLiteral]);
                    }
                    else if (TextType == TxtType.FMG)
                    {
                        bw.WriteInt32(CharLength);
                        bw.WriteInt32(TextID);
                    }
                    else if (TextType == TxtType.Dynamic)
                    {
                        bw.WriteInt32(CharLength);
                    }
                }
            }
        }

        /// <summary>
        /// Indicates the behavior of a UI element.
        /// </summary>
        public enum ControlType
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            DmeCtrlScrollText,

            /// <summary>
            /// Unknown.
            /// </summary>
            FrpgMenuDlgObjContentsHelpItem,

            /// <summary>
            /// Unknown.
            /// </summary>
            Static
        }

        /// <summary>
        /// Determines the behavior of a UI element.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public abstract class Control
        {
            /// <summary>
            /// The type of this control.
            /// </summary>
            public abstract ControlType Type { get; }

            internal static Control Read(BinaryReaderEx br, Dictionary<int, string> strings, long ctprStart)
            {
                int typeOffset = br.ReadInt32();
                int ctprOffset = br.ReadInt32();
                string type = strings[typeOffset];

                Control result;
                br.StepIn(ctprStart + ctprOffset);
                {
                    if (type == "DmeCtrlScrollText")
                        result = new ScrollTextDummy(br);
                    else if (type == "FrpgMenuDlgObjContentsHelpItem")
                        result = new HelpItem(br);
                    else if (type == "Static")
                        result = new Static(br);
                    else
                        throw new InvalidDataException($"Unknown control type: {type}");
                }
                br.StepOut();
                return result;
            }

            internal abstract void WriteData(BinaryWriterEx bw);

            internal void WriteHeader(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> ctprOffsets)
            {
                bw.WriteInt32(stringOffsets[Type.ToString()]);
                bw.WriteInt32(ctprOffsets.Dequeue());
            }

            /// <summary>
            /// Returns the type of the control.
            /// </summary>
            public override string ToString()
            {
                return $"{Type}";
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class ScrollTextDummy : Control
            {
                /// <summary>
                /// ControlType.DmeCtrlScrollText
                /// </summary>
                public override ControlType Type => ControlType.DmeCtrlScrollText;

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Creates a ScrollTextDummy with default values.
                /// </summary>
                public ScrollTextDummy() : base() { }

                internal ScrollTextDummy(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class HelpItem : Control
            {
                /// <summary>
                /// ControlType.FrpgMenuDlgObjContentsHelpItem
                /// </summary>
                public override ControlType Type => ControlType.FrpgMenuDlgObjContentsHelpItem;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// An FMG ID.
                /// </summary>
                public int TextID { get; set; }

                /// <summary>
                /// Creates a HelpItem with default values.
                /// </summary>
                public HelpItem() : base()
                {
                    TextID = -1;
                }

                internal HelpItem(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                    TextID = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                    bw.WriteInt32(TextID);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Static : Control
            {
                /// <summary>
                /// ControlType.Static
                /// </summary>
                public override ControlType Type => ControlType.Static;

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Creates a Static with default values.
                /// </summary>
                public Static() : base() { }

                internal Static(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                }
            }
        }

        /// <summary>
        /// Unknown.
        /// </summary>
        public class Anik
        {
            /// <summary>
            /// The name of this Anik.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk04 { get; set; }

            /// <summary>
            /// Unknown; always 0-1.
            /// </summary>
            public byte Unk08 { get; set; }

            /// <summary>
            /// Unknown; always 1-2.
            /// </summary>
            public byte Unk09 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public short Unk0A { get; set; }

            /// <summary>
            /// An offset into the INTP block.
            /// </summary>
            public int IntpOffset { get; set; }

            /// <summary>
            /// An offset into the ANIP block.
            /// </summary>
            public int AnipOffset { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk14 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk18 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk1C { get; set; }

            internal Anik(BinaryReaderEx br, Dictionary<int, string> strings)
            {
                int nameOffset = br.ReadInt32();
                Unk04 = br.ReadInt32();
                Unk08 = br.ReadByte();
                Unk09 = br.ReadByte();
                Unk0A = br.ReadInt16();
                IntpOffset = br.ReadInt32();
                AnipOffset = br.ReadInt32();
                Unk14 = br.ReadInt32();
                Unk18 = br.ReadInt32();
                Unk1C = br.ReadInt32();

                Name = strings[nameOffset];
            }

            internal void Write(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
            {
                bw.WriteInt32(stringOffsets[Name]);
                bw.WriteInt32(Unk04);
                bw.WriteByte(Unk08);
                bw.WriteByte(Unk09);
                bw.WriteInt16(Unk0A);
                bw.WriteInt32(IntpOffset);
                bw.WriteInt32(AnipOffset);
                bw.WriteInt32(Unk14);
                bw.WriteInt32(Unk18);
                bw.WriteInt32(Unk1C);
            }

            /// <summary>
            /// Returns the name of this Anik.
            /// </summary>
            public override string ToString()
            {
                return $"{Name}";
            }
        }

        /// <summary>
        /// Unknown.
        /// </summary>
        public class Anio
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk00 { get; set; }

            /// <summary>
            /// Aniks in this Anio.
            /// </summary>
            public List<Anik> Aniks { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk0C { get; set; }

            /// <summary>
            /// Creates an Anio with default values.
            /// </summary>
            public Anio()
            {
                Aniks = new List<Anik>();
            }

            internal Anio(BinaryReaderEx br, Dictionary<int, Anik> aniks)
            {
                Unk00 = br.ReadInt32();
                int anikCount = br.ReadInt32();
                int anikOffset = br.ReadInt32();
                Unk0C = br.ReadInt32();

                Aniks = new List<Anik>(anikCount);
                for (int i = 0; i < anikCount; i++)
                {
                    int offset = anikOffset + ANIK_SIZE * i;
                    Aniks.Add(aniks[offset]);
                    aniks.Remove(offset);
                }
            }

            internal void Write(BinaryWriterEx bw, Queue<int> anikOffsets)
            {
                bw.WriteInt32(Unk00);
                bw.WriteInt32(Aniks.Count);
                bw.WriteInt32(anikOffsets.Dequeue());
                bw.WriteInt32(Unk0C);
            }

            /// <summary>
            /// Returns the number of Aniks in this Anio.
            /// </summary>
            public override string ToString()
            {
                return $"Anio[{Aniks.Count}]";
            }
        }

        /// <summary>
        /// Unknown.
        /// </summary>
        public class Anim
        {
            /// <summary>
            /// The name of this Anim.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Anios in this Anim.
            /// </summary>
            public List<Anio> Anios { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk0C { get; set; }

            /// <summary>
            /// Unknown; always 4.
            /// </summary>
            public int Unk10 { get; set; }

            /// <summary>
            /// Unknown; always 4.
            /// </summary>
            public int Unk14 { get; set; }

            /// <summary>
            /// Unknown; always 4.
            /// </summary>
            public int Unk18 { get; set; }

            /// <summary>
            /// Unknown; always 1.
            /// </summary>
            public int Unk1C { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk20 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk24 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk28 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk2C { get; set; }

            /// <summary>
            /// Creates an Anim with default values.
            /// </summary>
            public Anim()
            {
                Name = "";
                Anios = new List<Anio>();
                Unk10 = 4;
                Unk14 = 4;
                Unk18 = 4;
                Unk1C = 1;
            }

            internal Anim(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Anio> anios)
            {
                int nameOffset = br.ReadInt32();
                int anioCount = br.ReadInt32();
                int anioOffset = br.ReadInt32();
                Unk0C = br.ReadInt32();
                Unk10 = br.ReadInt32();
                Unk14 = br.ReadInt32();
                Unk18 = br.ReadInt32();
                Unk1C = br.ReadInt32();
                Unk20 = br.ReadInt32();
                Unk24 = br.ReadInt32();
                Unk28 = br.ReadInt32();
                Unk2C = br.ReadInt32();

                Name = strings[nameOffset];
                Anios = new List<Anio>(anioCount);
                for (int i = 0; i < anioCount; i++)
                {
                    int offset = anioOffset + ANIO_SIZE * i;
                    Anios.Add(anios[offset]);
                    anios.Remove(offset);
                }
            }

            internal void Write(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> anioOffsets)
            {
                bw.WriteInt32(stringOffsets[Name]);
                bw.WriteInt32(Anios.Count);
                bw.WriteInt32(anioOffsets.Dequeue());
                bw.WriteInt32(Unk0C);
                bw.WriteInt32(Unk10);
                bw.WriteInt32(Unk14);
                bw.WriteInt32(Unk18);
                bw.WriteInt32(Unk1C);
                bw.WriteInt32(Unk20);
                bw.WriteInt32(Unk24);
                bw.WriteInt32(Unk28);
                bw.WriteInt32(Unk2C);
            }

            /// <summary>
            /// Returns the name and number of Anios.
            /// </summary>
            public override string ToString()
            {
                return $"{Name}[{Anios.Count}]";
            }
        }

        /// <summary>
        /// Unknown.
        /// </summary>
        public class Scdk
        {
            /// <summary>
            /// The name of this Scdk.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk04 { get; set; }

            /// <summary>
            /// Unknown; always 1.
            /// </summary>
            public int Unk08 { get; set; }

            /// <summary>
            /// Unknown; always 0-1.
            /// </summary>
            public int Unk0C { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk14 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk18 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk1C { get; set; }

            /// <summary>
            /// An index into the Anim list.
            /// </summary>
            public int AnimIndex { get; set; }

            /// <summary>
            /// Unknown; always 0-1.
            /// </summary>
            public int Scdp04 { get; set; }

            /// <summary>
            /// Creates a Scdk with default values.
            /// </summary>
            public Scdk()
            {
                Name = "";
                Unk08 = 1;
            }

            internal Scdk(BinaryReaderEx br, Dictionary<int, string> strings, long scdpStart)
            {
                int nameOffset = br.ReadInt32();
                Unk04 = br.ReadInt32();
                Unk08 = br.ReadInt32();
                Unk0C = br.ReadInt32();
                int scdpOffset = br.ReadInt32();
                Unk14 = br.ReadInt32();
                Unk18 = br.ReadInt32();
                Unk1C = br.ReadInt32();

                Name = strings[nameOffset];
                br.StepIn(scdpStart + scdpOffset);
                {
                    AnimIndex = br.ReadInt32();
                    Scdp04 = br.ReadInt32();
                }
                br.StepOut();
            }

            internal void WriteSCDP(BinaryWriterEx bw)
            {
                bw.WriteInt32(AnimIndex);
                bw.WriteInt32(Scdp04);
            }

            internal void Write(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> scdpOffsets)
            {
                bw.WriteInt32(stringOffsets[Name]);
                bw.WriteInt32(Unk04);
                bw.WriteInt32(Unk08);
                bw.WriteInt32(Unk0C);
                bw.WriteInt32(scdpOffsets.Dequeue());
                bw.WriteInt32(Unk14);
                bw.WriteInt32(Unk18);
                bw.WriteInt32(Unk1C);
            }

            /// <summary>
            /// Returns the name of this Scdk.
            /// </summary>
            public override string ToString()
            {
                return $"{Name}";
            }
        }

        /// <summary>
        /// Unknown.
        /// </summary>
        public class Scdo
        {
            /// <summary>
            /// The name of this Scdo.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Scdks in this Scdo.
            /// </summary>
            public List<Scdk> Scdks { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk0C { get; set; }

            /// <summary>
            /// Creates a Scdo with default values.
            /// </summary>
            public Scdo()
            {
                Name = "";
                Scdks = new List<Scdk>();
            }

            internal Scdo(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Scdk> scdks)
            {
                int nameOffset = br.ReadInt32();
                int scdkCount = br.ReadInt32();
                int scdkOffset = br.ReadInt32();
                Unk0C = br.ReadInt32();

                Name = strings[nameOffset];
                Scdks = new List<Scdk>(scdkCount);
                for (int i = 0; i < scdkCount; i++)
                {
                    int offset = scdkOffset + SCDK_SIZE * i;
                    Scdks.Add(scdks[offset]);
                    scdks.Remove(offset);
                }
            }

            internal void Write(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> scdkOffsets)
            {
                bw.WriteInt32(stringOffsets[Name]);
                bw.WriteInt32(Scdks.Count);
                bw.WriteInt32(scdkOffsets.Dequeue());
                bw.WriteInt32(Unk0C);
            }

            /// <summary>
            /// Returns the name and number of Scdks.
            /// </summary>
            public override string ToString()
            {
                return $"{Name}[{Scdks.Count}]";
            }
        }

        /// <summary>
        /// Unknown.
        /// </summary>
        public class Scdl
        {
            /// <summary>
            /// The name of this Scdl.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Scdos in this Scdl.
            /// </summary>
            public List<Scdo> Scdos { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk0C { get; set; }

            /// <summary>
            /// Creates a Scdl with default values.
            /// </summary>
            public Scdl()
            {
                Name = "";
                Scdos = new List<Scdo>();
            }

            internal Scdl(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Scdo> scdos)
            {
                int nameOffset = br.ReadInt32();
                int scdoCount = br.ReadInt32();
                int scdoOffset = br.ReadInt32();
                Unk0C = br.ReadInt32();

                Name = strings[nameOffset];
                Scdos = new List<Scdo>(scdoCount);
                for (int i = 0; i < scdoCount; i++)
                {
                    int offset = scdoOffset + SCDO_SIZE * i;
                    Scdos.Add(scdos[offset]);
                    scdos.Remove(offset);
                }
            }

            internal void Write(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> scdoOffsets)
            {
                bw.WriteInt32(stringOffsets[Name]);
                bw.WriteInt32(Scdos.Count);
                bw.WriteInt32(scdoOffsets.Dequeue());
                bw.WriteInt32(Unk0C);
            }

            /// <summary>
            /// Returns the name and number of Scdos.
            /// </summary>
            public override string ToString()
            {
                return $"{Name}[{Scdos.Count}]";
            }
        }

        /// <summary>
        /// An individual UI element.
        /// </summary>
        public class Dlgo
        {
            /// <summary>
            /// The name of the element.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The visual properties of the element.
            /// </summary>
            [Browsable(false)]
            public Shape Shape { get; set; }

            /// <summary>
            /// The behavior of the element.
            /// </summary>
            public Control Control { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk0C { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk10 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk14 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk18 { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk1C { get; set; }

            /// <summary>
            /// Creates a Dlgo with default values.
            /// </summary>
            public Dlgo()
            {
                Name = "";
                Shape = new Shape.Null();
                Control = new Control.Static();
            }

            /// <summary>
            /// Creates a Dlgo with the given values.
            /// </summary>
            public Dlgo(string name, Shape shape, Control control)
            {
                Name = name;
                Shape = shape;
                Control = control;
            }

            internal Dlgo(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Shape> shapes, Dictionary<int, Control> controls)
            {
                int nameOffset = br.ReadInt32();
                int shapOffset = br.ReadInt32();
                int ctrlOffset = br.ReadInt32();
                Unk0C = br.ReadInt32();
                Unk10 = br.ReadInt32();
                Unk14 = br.ReadInt32();
                Unk18 = br.ReadInt32();
                Unk1C = br.ReadInt32();

                Name = strings[nameOffset];
                Shape = shapes[shapOffset];
                shapes.Remove(shapOffset);
                Control = controls[ctrlOffset];
                controls.Remove(ctrlOffset);
            }

            internal void Write(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> shapOffsets, Queue<int> ctrlOffsets)
            {
                bw.WriteInt32(stringOffsets[Name]);
                bw.WriteInt32(shapOffsets.Dequeue());
                bw.WriteInt32(ctrlOffsets.Dequeue());
                bw.WriteInt32(Unk0C);
                bw.WriteInt32(Unk10);
                bw.WriteInt32(Unk14);
                bw.WriteInt32(Unk18);
                bw.WriteInt32(Unk1C);
            }

            /// <summary>
            /// Returns the name, shape type, and control type of the element.
            /// </summary>
            public override string ToString()
            {
                return $"{Name} ({Control.Type} {Shape.Type})";
            }
        }

        /// <summary>
        /// A group of UI elements, which is itself a UI element.
        /// </summary>
        public class Dlg : Dlgo
        {
            /// <summary>
            /// The child elements attached to this group.
            /// </summary>
            [Browsable(false)]
            public List<Dlgo> Dlgos { get; set; }

            /// <summary>
            /// Left edge of the group.
            /// </summary>
            public short LeftEdge { get; set; }

            /// <summary>
            /// Top edge of the group.
            /// </summary>
            public short TopEdge { get; set; }

            /// <summary>
            /// Right edge of the group.
            /// </summary>
            public short RightEdge { get; set; }

            /// <summary>
            /// Bottom edge of the group.
            /// </summary>
            public short BottomEdge { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short[] Unk30 { get; private set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public short Unk3A { get; set; }

            /// <summary>
            /// Unknown; always 0.
            /// </summary>
            public int Unk3C { get; set; }

            /// <summary>
            /// Creates a Dlg with default values.
            /// </summary>
            public Dlg() : base()
            {
                Dlgos = new List<Dlgo>();
                Unk30 = new short[5] { -1, -1, -1, -1, -1 };
            }

            /// <summary>
            /// Creates a Dlg with the given values.
            /// </summary>
            public Dlg(string name, Shape shape, Control control) : base(name, shape, control)
            {
                Dlgos = new List<Dlgo>();
                Unk30 = new short[5] { -1, -1, -1, -1, -1 };
            }

            internal Dlg(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Shape> shapes, Dictionary<int, Control> controls, Dictionary<int, Dlgo> dlgos) : base(br, strings, shapes, controls)
            {
                int dlgoCount = br.ReadInt32();
                int dlgoOffset = br.ReadInt32();
                LeftEdge = br.ReadInt16();
                TopEdge = br.ReadInt16();
                RightEdge = br.ReadInt16();
                BottomEdge = br.ReadInt16();
                Unk30 = br.ReadInt16s(5);
                Unk3A = br.ReadInt16();
                Unk3C = br.ReadInt32();

                Dlgos = new List<Dlgo>(dlgoCount);
                for (int i = 0; i < dlgoCount; i++)
                {
                    int offset = dlgoOffset + DLGO_SIZE * i;
                    Dlgos.Add(dlgos[offset]);
                    dlgos.Remove(offset);
                }
            }

            internal void Write(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> shapOffsets, Queue<int> ctrlOffsets, Queue<int> dlgoOffsets)
            {
                Write(bw, stringOffsets, shapOffsets, ctrlOffsets);
                bw.WriteInt32(Dlgos.Count);
                bw.WriteInt32(dlgoOffsets.Dequeue());
                bw.WriteInt16(LeftEdge);
                bw.WriteInt16(TopEdge);
                bw.WriteInt16(RightEdge);
                bw.WriteInt16(BottomEdge);
                bw.WriteInt16s(Unk30);
                bw.WriteInt16(Unk3A);
                bw.WriteInt32(Unk3C);
            }

            /// <summary>
            /// Returns the child element with the given name, or null if not found.
            /// </summary>
            public Dlgo this[string name] => Dlgos.Find(dlgo => dlgo.Name == name);

            /// <summary>
            /// Returns the name, number of child elements, shape type, and control type of this group.
            /// </summary>
            public override string ToString()
            {
                return $"{Name} ({Control.Type} {Shape.Type} [{Dlgos.Count}])";
            }
        }

        #region Read/write methods
        private Dictionary<int, string> ReadSTR(BinaryReaderEx br)
        {
            long start = ReadBlockHeader(br, "STR\0", out int count);
            var strings = new Dictionary<int, string>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = (int)(br.Position - start);
                strings[offset] = br.ReadUTF16();
            }
            br.Pad(0x10);
            return strings;
        }

        private Dictionary<string, int> WriteSTR(BinaryWriterEx bw)
        {
            long start = WriteBlockHeader(bw, "STR\0");
            var stringOffsets = new Dictionary<string, int>();
            void writeString(string str)
            {
                if (!stringOffsets.ContainsKey(str))
                {
                    long offset = bw.Position - start;
                    bw.WriteUTF16(str, true);
                    stringOffsets[str] = (int)offset;
                }
            }

            foreach (Texture tex in Textures)
            {
                writeString(tex.Name);
                writeString(tex.Path);
            }

            foreach (Anim anim in Anims)
            {
                writeString(anim.Name);
                foreach (Anio anio in anim.Anios)
                {
                    foreach (Anik anik in anio.Aniks)
                    {
                        writeString(anik.Name);
                    }
                }
            }

            foreach (Scdl scdl in Scdls)
            {
                writeString(scdl.Name);
                foreach (Scdo scdo in scdl.Scdos)
                {
                    writeString(scdo.Name);
                    foreach (Scdk scdk in scdo.Scdks)
                    {
                        writeString(scdk.Name);
                    }
                }
            }

            void writeShapeStrings(Shape shape)
            {
                writeString(shape.Type.ToString());
                if (shape is Shape.ScrollText scrollText && scrollText.TextType == Shape.TxtType.Literal)
                {
                    writeString(scrollText.TextLiteral);
                }
                else if (shape is Shape.Text text && text.TextType == Shape.TxtType.Literal)
                {
                    writeString(text.TextLiteral);
                }
            }

            foreach (Dlg dlg in Dlgs)
            {
                writeString(dlg.Name);
                writeShapeStrings(dlg.Shape);
                writeString(dlg.Control.Type.ToString());
                foreach (Dlgo dlgo in dlg.Dlgos)
                {
                    writeString(dlgo.Name);
                    writeShapeStrings(dlgo.Shape);
                    writeString(dlgo.Control.Type.ToString());
                }
            }

            FinishBlockHeader(bw, "STR\0", start, stringOffsets.Count);
            return stringOffsets;
        }

        private List<Texture> ReadTEXI(BinaryReaderEx br, Dictionary<int, string> strings)
        {
            ReadBlockHeader(br, "TEXI", out int count);
            var textures = new List<Texture>(count);
            for (int i = 0; i < count; i++)
            {
                textures.Add(new Texture(br, strings));
            }
            br.Pad(0x10);
            return textures;
        }

        private void WriteTEXI(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
        {
            long start = WriteBlockHeader(bw, "TEXI");
            foreach (Texture tex in Textures)
                tex.Write(bw, stringOffsets);
            FinishBlockHeader(bw, "TEXI", start, Textures.Count);
        }

        private Queue<int> WriteSHPR(BinaryWriterEx bw, bool dsr, Dictionary<string, int> stringOffsets)
        {
            long start = WriteBlobBlock(bw, "SHPR");
            var shprOffsets = new Queue<int>();
            void writeShape(Shape shape)
            {
                int offset = (int)(bw.Position - start);
                shprOffsets.Enqueue(offset);
                shape.WriteData(bw, dsr, stringOffsets);
            }

            foreach (Dlg dlg in Dlgs)
            {
                foreach (Dlgo dlgo in dlg.Dlgos)
                {
                    writeShape(dlgo.Shape);
                }
            }

            foreach (Dlg dlg in Dlgs)
            {
                writeShape(dlg.Shape);
            }

            FinishBlobBlock(bw, "SHPR", start);
            return shprOffsets;
        }

        private Queue<int> WriteCTPR(BinaryWriterEx bw)
        {
            long start = WriteBlobBlock(bw, "CTPR");
            var ctprOffsets = new Queue<int>();
            void writeControl(Control control)
            {
                int offset = (int)(bw.Position - start);
                ctprOffsets.Enqueue(offset);
                control.WriteData(bw);
            }

            foreach (Dlg dlg in Dlgs)
            {
                foreach (Dlgo dlgo in dlg.Dlgos)
                {
                    writeControl(dlgo.Control);
                }
            }

            foreach (Dlg dlg in Dlgs)
            {
                writeControl(dlg.Control);
            }

            FinishBlobBlock(bw, "CTPR", start);
            return ctprOffsets;
        }

        private Queue<int> WriteSCDP(BinaryWriterEx bw)
        {
            long start = WriteBlobBlock(bw, "SCDP");
            var scdpOffsets = new Queue<int>();

            foreach (Scdl scdl in Scdls)
            {
                foreach (Scdo scdo in scdl.Scdos)
                {
                    foreach (Scdk scdk in scdo.Scdks)
                    {
                        int offset = (int)(bw.Position - start);
                        scdpOffsets.Enqueue(offset);
                        scdk.WriteSCDP(bw);
                    }
                }
            }

            FinishBlobBlock(bw, "SCDP", start);
            return scdpOffsets;
        }

        private Dictionary<int, Shape> ReadSHAP(BinaryReaderEx br, bool dsr, Dictionary<int, string> strings, long shprStart)
        {
            long start = ReadBlockHeader(br, "SHAP", out int count);
            var shapes = new Dictionary<int, Shape>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = (int)(br.Position - start);
                shapes[offset] = Shape.Read(br, dsr, strings, shprStart);
            }
            br.Pad(0x10);
            return shapes;
        }

        private Queue<int> WriteSHAP(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> shprOffsets)
        {
            long start = WriteBlockHeader(bw, "SHAP");
            var shapOffsets = new Queue<int>();
            void writeShape(Shape shape)
            {
                int offset = (int)(bw.Position - start);
                shapOffsets.Enqueue(offset);
                shape.WriteHeader(bw, stringOffsets, shprOffsets);
            }

            foreach (Dlg dlg in Dlgs)
            {
                foreach (Dlgo dlgo in dlg.Dlgos)
                {
                    writeShape(dlgo.Shape);
                }
            }

            foreach (Dlg dlg in Dlgs)
            {
                writeShape(dlg.Shape);
            }

            FinishBlockHeader(bw, "SHAP", start, shapOffsets.Count);
            return shapOffsets;
        }

        private Dictionary<int, Control> ReadCTRL(BinaryReaderEx br, Dictionary<int, string> strings, long ctprStart)
        {
            long start = ReadBlockHeader(br, "CTRL", out int count);
            var controls = new Dictionary<int, Control>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = (int)(br.Position - start);
                controls[offset] = Control.Read(br, strings, ctprStart);
            }
            br.Pad(0x10);
            return controls;
        }

        private Queue<int> WriteCTRL(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> ctprOffsets)
        {
            long start = WriteBlockHeader(bw, "CTRL");
            var ctrlOffsets = new Queue<int>();
            void writeControl(Control control)
            {
                int offset = (int)(bw.Position - start);
                ctrlOffsets.Enqueue(offset);
                control.WriteHeader(bw, stringOffsets, ctprOffsets);
            }

            foreach (Dlg dlg in Dlgs)
            {
                foreach (Dlgo dlgo in dlg.Dlgos)
                {
                    writeControl(dlgo.Control);
                }
            }

            foreach (Dlg dlg in Dlgs)
            {
                writeControl(dlg.Control);
            }

            FinishBlockHeader(bw, "CTRL", start, ctrlOffsets.Count);
            return ctrlOffsets;
        }

        private Dictionary<int, Anik> ReadANIK(BinaryReaderEx br, Dictionary<int, string> strings)
        {
            long start = ReadBlockHeader(br, "ANIK", out int count);
            var aniks = new Dictionary<int, Anik>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = (int)(br.Position - start);
                aniks[offset] = new Anik(br, strings);
            }
            br.Pad(0x10);
            return aniks;
        }

        private Queue<int> WriteANIK(BinaryWriterEx bw, Dictionary<string, int> stringOffsets)
        {
            long start = WriteBlockHeader(bw, "ANIK");
            int count = 0;
            var anikOffsets = new Queue<int>();

            foreach (Anim anim in Anims)
            {
                foreach (Anio anio in anim.Anios)
                {
                    int offset = (int)(bw.Position - start);
                    anikOffsets.Enqueue(offset);
                    count += anio.Aniks.Count;
                    foreach (Anik anik in anio.Aniks)
                        anik.Write(bw, stringOffsets);
                }
            }

            FinishBlockHeader(bw, "ANIK", start, count);
            return anikOffsets;
        }

        private Dictionary<int, Anio> ReadANIO(BinaryReaderEx br, Dictionary<int, Anik> aniks)
        {
            long start = ReadBlockHeader(br, "ANIO", out int count);
            var anios = new Dictionary<int, Anio>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = (int)(br.Position - start);
                anios[offset] = new Anio(br, aniks);
            }
            br.Pad(0x10);
            return anios;
        }

        private Queue<int> WriteANIO(BinaryWriterEx bw, Queue<int> anikOffsets)
        {
            long start = WriteBlockHeader(bw, "ANIO");
            int count = 0;
            var anioOffsets = new Queue<int>();

            foreach (Anim anim in Anims)
            {
                int offset = (int)(bw.Position - start);
                anioOffsets.Enqueue(offset);
                count += anim.Anios.Count;
                foreach (Anio anio in anim.Anios)
                    anio.Write(bw, anikOffsets);
            }

            FinishBlockHeader(bw, "ANIO", start, count);
            return anioOffsets;
        }

        private List<Anim> ReadANIM(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Anio> anios)
        {
            ReadBlockHeader(br, "ANIM", out int count);
            var anims = new List<Anim>(count);
            for (int i = 0; i < count; i++)
            {
                anims.Add(new Anim(br, strings, anios));
            }
            br.Pad(0x10);
            return anims;
        }

        private void WriteANIM(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> anioOffsets)
        {
            long start = WriteBlockHeader(bw, "ANIM");
            foreach (Anim anim in Anims)
            {
                anim.Write(bw, stringOffsets, anioOffsets);
            }
            FinishBlockHeader(bw, "ANIM", start, Anims.Count);
        }

        private Dictionary<int, Scdk> ReadSCDK(BinaryReaderEx br, Dictionary<int, string> strings, long scdpStart)
        {
            long start = ReadBlockHeader(br, "SCDK", out int count);
            var scdks = new Dictionary<int, Scdk>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = (int)(br.Position - start);
                scdks[offset] = new Scdk(br, strings, scdpStart);
            }
            br.Pad(0x10);
            return scdks;
        }

        private Queue<int> WriteSCDK(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> scdpOffsets)
        {
            long start = WriteBlockHeader(bw, "SCDK");
            int count = 0;
            var scdkOffsets = new Queue<int>();

            foreach (Scdl scdl in Scdls)
            {
                foreach (Scdo scdo in scdl.Scdos)
                {
                    int offset = (int)(bw.Position - start);
                    scdkOffsets.Enqueue(offset);
                    count += scdo.Scdks.Count;
                    foreach (Scdk scdk in scdo.Scdks)
                        scdk.Write(bw, stringOffsets, scdpOffsets);
                }
            }

            FinishBlockHeader(bw, "SCDK", start, count);
            return scdkOffsets;
        }

        private Dictionary<int, Scdo> ReadSCDO(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Scdk> scdks)
        {
            long start = ReadBlockHeader(br, "SCDO", out int count);
            var scdos = new Dictionary<int, Scdo>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = (int)(br.Position - start);
                scdos[offset] = new Scdo(br, strings, scdks);
            }
            br.Pad(0x10);
            return scdos;
        }

        private Queue<int> WriteSCDO(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> scdkOffsets)
        {
            long start = WriteBlockHeader(bw, "SCDO");
            int count = 0;
            var scdoOffsets = new Queue<int>();

            foreach (Scdl scdl in Scdls)
            {
                int offset = (int)(bw.Position - start);
                scdoOffsets.Enqueue(offset);
                count += scdl.Scdos.Count;
                foreach (Scdo scdo in scdl.Scdos)
                    scdo.Write(bw, stringOffsets, scdkOffsets);
            }

            FinishBlockHeader(bw, "SCDO", start, count);
            return scdoOffsets;
        }

        private List<Scdl> ReadSCDL(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Scdo> scdos)
        {
            ReadBlockHeader(br, "SCDL", out int count);
            var scdls = new List<Scdl>(count);
            for (int i = 0; i < count; i++)
            {
                scdls.Add(new Scdl(br, strings, scdos));
            }
            br.Pad(0x10);
            return scdls;
        }

        private void WriteSCDL(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> scdoOffsets)
        {
            long start = WriteBlockHeader(bw, "SCDL");
            foreach (Scdl scdl in Scdls)
            {
                scdl.Write(bw, stringOffsets, scdoOffsets);
            }
            FinishBlockHeader(bw, "SCDL", start, Scdls.Count);
        }

        private Dictionary<int, Dlgo> ReadDLGO(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Shape> shapes, Dictionary<int, Control> controls)
        {
            long start = ReadBlockHeader(br, "DLGO", out int count);
            var dlgos = new Dictionary<int, Dlgo>(count);
            for (int i = 0; i < count; i++)
            {
                int offset = (int)(br.Position - start);
                dlgos[offset] = new Dlgo(br, strings, shapes, controls);
            }
            br.Pad(0x10);
            return dlgos;
        }

        private Queue<int> WriteDLGO(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> shapOffsets, Queue<int> ctrlOffsets)
        {
            long start = WriteBlockHeader(bw, "DLGO");
            int count = 0;
            var dlgoOffsets = new Queue<int>();

            foreach (Dlg dlg in Dlgs)
            {
                int offset = (int)(bw.Position - start);
                dlgoOffsets.Enqueue(offset);
                count += dlg.Dlgos.Count;
                foreach (Dlgo dlgo in dlg.Dlgos)
                    dlgo.Write(bw, stringOffsets, shapOffsets, ctrlOffsets);
            }

            FinishBlockHeader(bw, "DLGO", start, count);
            return dlgoOffsets;
        }

        private List<Dlg> ReadDLG(BinaryReaderEx br, Dictionary<int, string> strings, Dictionary<int, Shape> shapes, Dictionary<int, Control> controls, Dictionary<int, Dlgo> dlgos)
        {
            ReadBlockHeader(br, "DLG\0", out int count);
            var dlgs = new List<Dlg>(count);
            for (int i = 0; i < count; i++)
            {
                dlgs.Add(new Dlg(br, strings, shapes, controls, dlgos));
            }
            br.Pad(0x10);
            return dlgs;
        }

        private void WriteDLG(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> shapOffsets, Queue<int> ctrlOffsets, Queue<int> dlgoOffsets)
        {
            long start = WriteBlockHeader(bw, "DLG\0");
            foreach (Dlg dlg in Dlgs)
            {
                dlg.Write(bw, stringOffsets, shapOffsets, ctrlOffsets, dlgoOffsets);
            }
            FinishBlockHeader(bw, "DLG\0", start, Dlgs.Count);
        }
        #endregion

        #region Read/write utilities
        private static void ReadNullBlock(BinaryReaderEx br, string name)
        {
            br.AssertASCII(name);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
        }

        private static void WriteNullBlock(BinaryWriterEx bw, string name)
        {
            bw.WriteASCII(name);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
        }

        private static long ReadBlobBlock(BinaryReaderEx br, string name)
        {
            br.AssertASCII(name);
            int size = br.ReadInt32();
            br.AssertInt32(1);
            br.AssertInt32(0);

            long start = br.Position;
            br.Skip(size);
            br.Pad(0x10);
            return start;
        }

        private static long WriteBlobBlock(BinaryWriterEx bw, string name)
        {
            bw.WriteASCII(name);
            bw.ReserveInt32($"BlobBlockSize{name}");
            bw.WriteInt32(1);
            bw.WriteInt32(0);
            return bw.Position;
        }

        private static void FinishBlobBlock(BinaryWriterEx bw, string name, long start)
        {
            bw.Pad(0x10);
            bw.FillInt32($"BlobBlockSize{name}", (int)(bw.Position - start));
        }

        private static byte[] ReadBlobBytes(BinaryReaderEx br, string name)
        {
            br.AssertASCII(name);
            int size = br.ReadInt32();
            br.AssertInt32(1);
            br.AssertInt32(0);

            byte[] bytes = br.ReadBytes(size);
            br.Pad(0x10);
            return bytes;
        }

        private static void WriteBlobBytes(BinaryWriterEx bw, string name, byte[] bytes)
        {
            bw.WriteASCII(name);
            bw.ReserveInt32("BlobSize");
            bw.WriteInt32(1);
            bw.WriteInt32(0);

            long start = bw.Position;
            bw.WriteBytes(bytes);
            bw.Pad(0x10);
            bw.FillInt32("BlobSize", (int)(bw.Position - start));
        }

        private static long ReadBlockHeader(BinaryReaderEx br, string name, out int count)
        {
            br.AssertASCII(name);
            int size = br.ReadInt32();
            count = br.ReadInt32();
            br.AssertInt32(0);
            return br.Position;
        }

        private static long WriteBlockHeader(BinaryWriterEx bw, string name)
        {
            bw.WriteASCII(name);
            bw.ReserveInt32($"BlockSize{name}");
            bw.ReserveInt32($"BlockCount{name}");
            bw.WriteInt32(0);
            return bw.Position;
        }

        private static void FinishBlockHeader(BinaryWriterEx bw, string name, long start, int count)
        {
            bw.Pad(0x10);
            bw.FillInt32($"BlockSize{name}", (int)(bw.Position - start));
            bw.FillInt32($"BlockCount{name}", count);
        }

        private static Color ReadABGR(BinaryReaderEx br)
        {
            byte[] abgr = br.ReadBytes(4);
            return Color.FromArgb(abgr[0], abgr[3], abgr[2], abgr[1]);
        }

        private static void WriteABGR(BinaryWriterEx bw, Color color)
        {
            bw.WriteByte(color.A);
            bw.WriteByte(color.B);
            bw.WriteByte(color.G);
            bw.WriteByte(color.R);
        }
        #endregion

        #region SoulsFile boilerplate
        /// <summary>
        /// The type of DCX compression to be used when writing.
        /// </summary>
        public DCX.Type Compression = DCX.Type.None;

        /// <summary>
        /// Returns true if the bytes appear to be a DRB.
        /// </summary>
        public static bool Is(byte[] bytes)
        {
            if (bytes.Length == 0)
                return false;

            BinaryReaderEx br = new BinaryReaderEx(false, bytes);
            return Is(SFUtil.GetDecompressedBR(br, out _));
        }

        /// <summary>
        /// Returns true if the file appears to be a DRB.
        /// </summary>
        public static bool Is(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                if (stream.Length == 0)
                    return false;

                BinaryReaderEx br = new BinaryReaderEx(false, stream);
                return Is(SFUtil.GetDecompressedBR(br, out _));
            }
        }

        /// <summary>
        /// Loads a DRB from a byte array, automatically decompressing it if necessary.
        /// </summary>
        public static DRB Read(byte[] bytes, bool dsr)
        {
            BinaryReaderEx br = new BinaryReaderEx(false, bytes);
            DRB drb = new DRB();
            br = SFUtil.GetDecompressedBR(br, out drb.Compression);
            drb.Read(br, dsr);
            return drb;
        }

        /// <summary>
        /// Loads a DRB from the specified path, automatically decompressing it if necessary.
        /// </summary>
        public static DRB Read(string path, bool dsr)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                BinaryReaderEx br = new BinaryReaderEx(false, stream);
                DRB drb = new DRB();
                br = SFUtil.GetDecompressedBR(br, out drb.Compression);
                drb.Read(br, dsr);
                return drb;
            }
        }

        /// <summary>
        /// Writes file data to a BinaryWriterEx, compressing it afterwards if specified.
        /// </summary>
        private void Write(BinaryWriterEx bw, DCX.Type compression)
        {
            if (compression == DCX.Type.None)
            {
                Write(bw);
            }
            else
            {
                BinaryWriterEx bwUncompressed = new BinaryWriterEx(false);
                Write(bwUncompressed);
                byte[] uncompressed = bwUncompressed.FinishBytes();
                DCX.Compress(uncompressed, bw, compression);
            }
        }

        /// <summary>
        /// Writes the file to an array of bytes, automatically compressing it if necessary.
        /// </summary>
        public byte[] Write()
        {
            return Write(Compression);
        }

        /// <summary>
        /// Writes the file to an array of bytes, compressing it as specified.
        /// </summary>
        public byte[] Write(DCX.Type compression)
        {
            BinaryWriterEx bw = new BinaryWriterEx(false);
            Write(bw, compression);
            return bw.FinishBytes();
        }

        /// <summary>
        /// Writes the file to the specified path, automatically compressing it if necessary.
        /// </summary>
        public void Write(string path)
        {
            Write(path, Compression);
        }

        /// <summary>
        /// Writes the file to the specified path, compressing it as specified.
        /// </summary>
        public void Write(string path, DCX.Type compression)
        {
            using (FileStream stream = File.Create(path))
            {
                BinaryWriterEx bw = new BinaryWriterEx(false, stream);
                Write(bw, compression);
                bw.Finish();
            }
        }
        #endregion
    }
}
