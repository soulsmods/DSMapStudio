using System;
using DefType = SoulsFormats.PARAMDEF.DefType;
using EditFlags = SoulsFormats.PARAMDEF.EditFlags;

namespace SoulsFormats
{
    internal static class ParamUtil
    {
        public static string GetDefaultFormat(DefType type)
        {
            switch (type)
            {
                case DefType.s8: return "%d";
                case DefType.u8: return "%d";
                case DefType.s16: return "%d";
                case DefType.u16: return "%d";
                case DefType.s32: return "%d";
                case DefType.u32: return "%d";
                case DefType.f32: return "%f";
                case DefType.dummy8: return "";
                case DefType.fixstr: return "%d";
                case DefType.fixstrW: return "%d";

                default:
                    throw new NotImplementedException($"No default format specified for {nameof(DefType)}.{type}");
            }
        }

        public static float GetDefaultMinimum(DefType type)
        {
            switch (type)
            {
                case DefType.s8: return sbyte.MinValue;
                case DefType.u8: return byte.MinValue;
                case DefType.s16: return short.MinValue;
                case DefType.u16: return ushort.MinValue;
                case DefType.s32: return int.MinValue;
                case DefType.u32: return uint.MinValue;
                case DefType.f32: return float.MinValue;
                case DefType.dummy8: return 0;
                case DefType.fixstr: return -1;
                case DefType.fixstrW: return -1;

                default:
                    throw new NotImplementedException($"No default minimum specified for {nameof(DefType)}.{type}");
            }
        }

        public static float GetDefaultMaximum(DefType type)
        {
            switch (type)
            {
                case DefType.s8: return sbyte.MaxValue;
                case DefType.u8: return byte.MaxValue;
                case DefType.s16: return short.MaxValue;
                case DefType.u16: return ushort.MaxValue;
                case DefType.s32: return int.MaxValue;
                case DefType.u32: return uint.MaxValue;
                case DefType.f32: return float.MaxValue;
                case DefType.dummy8: return 0;
                case DefType.fixstr: return 1000000000;
                case DefType.fixstrW: return 1000000000;

                default:
                    throw new NotImplementedException($"No default maximum specified for {nameof(DefType)}.{type}");
            }
        }

        public static float GetDefaultIncrement(DefType type)
        {
            switch (type)
            {
                case DefType.s8: return 1;
                case DefType.u8: return 1;
                case DefType.s16: return 1;
                case DefType.u16: return 1;
                case DefType.s32: return 1;
                case DefType.u32: return 1;
                case DefType.f32: return 0.01f;
                case DefType.dummy8: return 0;
                case DefType.fixstr: return 1;
                case DefType.fixstrW: return 1;

                default:
                    throw new NotImplementedException($"No default increment specified for {nameof(DefType)}.{type}");
            }
        }

        public static EditFlags GetDefaultEditFlags(DefType type)
        {
            switch (type)
            {
                case DefType.s8: return EditFlags.Wrap;
                case DefType.u8: return EditFlags.Wrap;
                case DefType.s16: return EditFlags.Wrap;
                case DefType.u16: return EditFlags.Wrap;
                case DefType.s32: return EditFlags.Wrap;
                case DefType.u32: return EditFlags.Wrap;
                case DefType.f32: return EditFlags.Wrap;
                case DefType.dummy8: return EditFlags.None;
                case DefType.fixstr: return EditFlags.Wrap;
                case DefType.fixstrW: return EditFlags.Wrap;

                default:
                    throw new NotImplementedException($"No default edit flags specified for {nameof(DefType)}.{type}");
            }
        }

        public static bool IsArrayType(DefType type)
        {
            switch (type)
            {
                case DefType.dummy8:
                case DefType.fixstr:
                case DefType.fixstrW:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsBitType(DefType type)
        {
            switch (type)
            {
                case DefType.u8:
                case DefType.u16:
                case DefType.u32:
                case DefType.dummy8:
                    return true;

                default:
                    return false;
            }
        }

        public static int GetValueSize(DefType type)
        {
            switch (type)
            {
                case DefType.s8: return 1;
                case DefType.u8: return 1;
                case DefType.s16: return 2;
                case DefType.u16: return 2;
                case DefType.s32: return 4;
                case DefType.u32: return 4;
                case DefType.f32: return 4;
                case DefType.dummy8: return 1;
                case DefType.fixstr: return 1;
                case DefType.fixstrW: return 2;

                default:
                    throw new NotImplementedException($"No value size specified for {nameof(DefType)}.{type}");
            }
        }

        public static object CastDefaultValue(PARAMDEF.Field field)
        {
            switch (field.DisplayType)
            {
                case DefType.s8: return (sbyte)field.Default;
                case DefType.u8: return (byte)field.Default;
                case DefType.s16: return (short)field.Default;
                case DefType.u16: return (ushort)field.Default;
                case DefType.s32: return (int)field.Default;
                case DefType.u32: return (uint)field.Default;
                case DefType.f32: return field.Default;
                case DefType.fixstr: return "";
                case DefType.fixstrW: return "";
                case DefType.dummy8:
                    if (field.BitSize == -1)
                        return new byte[field.ArrayLength];
                    else
                        return (byte)field.Default;

                default:
                    throw new NotImplementedException($"Default not implemented for type {field.DisplayType}");
            }
        }

        public static int GetBitLimit(DefType type)
        {
            if (type == DefType.u8)
                return 8;
            else if (type == DefType.u16)
                return 16;
            else if (type == DefType.u32)
                return 32;
            else
                throw new InvalidOperationException($"Bit type may only be u8, u16, or u32.");
        }
    }
}
