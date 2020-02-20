using System;
using System.Xml;

namespace SoulsFormats.XmlExtensions
{
    internal static class XmlNodeExtensions
    {
        public static bool ReadBoolean(this XmlNode node, string xpath)
            => bool.Parse(node.SelectSingleNode(xpath).InnerText);

        public static bool ReadBooleanOrDefault(this XmlNode node, string xpath, bool def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : bool.Parse(child.InnerText);
        }

        public static sbyte ReadSByte(this XmlNode node, string xpath)
            => sbyte.Parse(node.SelectSingleNode(xpath).InnerText);

        public static sbyte ReadSByteOrDefault(this XmlNode node, string xpath, sbyte def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : sbyte.Parse(child.InnerText);
        }

        public static byte ReadByte(this XmlNode node, string xpath)
            => byte.Parse(node.SelectSingleNode(xpath).InnerText);

        public static byte ReadByteOrDefault(this XmlNode node, string xpath, byte def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : byte.Parse(child.InnerText);
        }

        public static short ReadInt16(this XmlNode node, string xpath)
            => short.Parse(node.SelectSingleNode(xpath).InnerText);

        public static short ReadInt16OrDefault(this XmlNode node, string xpath, short def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : short.Parse(child.InnerText);
        }

        public static ushort ReadUInt16(this XmlNode node, string xpath)
            => ushort.Parse(node.SelectSingleNode(xpath).InnerText);

        public static ushort ReadUInt16OrDefault(this XmlNode node, string xpath, ushort def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : ushort.Parse(child.InnerText);
        }

        public static int ReadInt32(this XmlNode node, string xpath)
            => int.Parse(node.SelectSingleNode(xpath).InnerText);

        public static int ReadInt32OrDefault(this XmlNode node, string xpath, int def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : int.Parse(child.InnerText);
        }

        public static uint ReadUInt32(this XmlNode node, string xpath)
            => uint.Parse(node.SelectSingleNode(xpath).InnerText);

        public static uint ReadUInt32OrDefault(this XmlNode node, string xpath, uint def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : uint.Parse(child.InnerText);
        }

        public static float ReadSingle(this XmlNode node, string xpath)
            => float.Parse(node.SelectSingleNode(xpath).InnerText);

        public static float ReadSingle(this XmlNode node, string xpath, IFormatProvider provider)
            => float.Parse(node.SelectSingleNode(xpath).InnerText, provider);

        public static float ReadSingleOrDefault(this XmlNode node, string xpath, float def, IFormatProvider provider)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : float.Parse(child.InnerText, provider);
        }

        public static float ReadSingleOrDefault(this XmlNode node, string xpath, float def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : float.Parse(child.InnerText);
        }

        public static string ReadString(this XmlNode node, string xpath)
            => node.SelectSingleNode(xpath).InnerText;

        public static string ReadStringOrDefault(this XmlNode node, string xpath, string def)
        {
            XmlNode child = node.SelectSingleNode(xpath);
            return child == null ? def : child.InnerText;
        }
    }
}
