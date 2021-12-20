using SoulsFormats.XmlExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace SoulsFormats
{
    public partial class PARAMDEF
    {
        private static class XmlSerializer
        {
            public const int CURRENT_XML_VERSION = 1;

            public static PARAMDEF Deserialize(XmlDocument xml)
            {
                var def = new PARAMDEF();
                XmlNode root = xml.SelectSingleNode("PARAMDEF");
                // In the interest of maximum compatibility, we will no longer check the XML version;
                // just try everything and hope it works.

                def.ParamType = root.SelectSingleNode("ParamType").InnerText;
                def.DataVersion = root.ReadInt16IfExist("DataVersion") ?? root.ReadInt16("Unk06");
                def.BigEndian = root.ReadBoolean("BigEndian");
                def.Unicode = root.ReadBoolean("Unicode");
                def.FormatVersion = root.ReadInt16IfExist("FormatVersion") ?? root.ReadInt16("Version");

                def.Fields = new List<Field>();
                foreach (XmlNode node in root.SelectNodes("Fields/Field"))
                {
                    def.Fields.Add(DeserializeField(node));
                }

                return def;
            }

            public static void Serialize(PARAMDEF def, XmlWriter xw, int xmlVersion)
            {
                if (xmlVersion < 0 || xmlVersion > CURRENT_XML_VERSION)
                    throw new InvalidOperationException($"XML version {xmlVersion} not recognized.");

                xw.WriteStartDocument();
                xw.WriteStartElement("PARAMDEF");
                xw.WriteAttributeString("XmlVersion", xmlVersion.ToString());
                xw.WriteElementString("ParamType", def.ParamType);
                xw.WriteElementString(xmlVersion == 0 ? "Unk06" : "DataVersion", def.DataVersion.ToString());
                xw.WriteElementString("BigEndian", def.BigEndian.ToString());
                xw.WriteElementString("Unicode", def.Unicode.ToString());
                xw.WriteElementString(xmlVersion == 0 ? "Version" : "FormatVersion", def.FormatVersion.ToString());

                xw.WriteStartElement("Fields");
                foreach (Field field in def.Fields)
                {
                    xw.WriteStartElement("Field");
                    SerializeField(field, xw);
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();

                xw.WriteEndElement();
            }


            private static readonly Regex defOuterRx = new Regex($@"^(?<type>\S+)\s+(?<name>.+?)(?:\s*=\s*(?<default>\S+))?$");
            private static readonly Regex defBitRx = new Regex($@"^(?<name>.+?)\s*:\s*(?<size>\d+)$");
            private static readonly Regex defArrayRx = new Regex($@"^(?<name>.+?)\s*\[\s*(?<length>\d+)\]$");

            private static Field DeserializeField(XmlNode node)
            {
                var field = new Field();
                string def = node.Attributes["Def"].InnerText;
                Match outerMatch = defOuterRx.Match(def);
                field.DisplayType = (DefType)Enum.Parse(typeof(DefType), outerMatch.Groups["type"].Value.Trim());
                if (outerMatch.Groups["default"].Success)
                    field.Default = float.Parse(outerMatch.Groups["default"].Value, CultureInfo.InvariantCulture);

                string internalName = outerMatch.Groups["name"].Value.Trim();
                Match bitMatch = defBitRx.Match(internalName);
                Match arrayMatch = defArrayRx.Match(internalName);
                field.BitSize = -1;
                field.ArrayLength = 1;
                if (ParamUtil.IsBitType(field.DisplayType) && bitMatch.Success)
                {
                    field.BitSize = int.Parse(bitMatch.Groups["size"].Value);
                    internalName = bitMatch.Groups["name"].Value;
                }
                else if (ParamUtil.IsArrayType(field.DisplayType))
                {
                    field.ArrayLength = int.Parse(arrayMatch.Groups["length"].Value);
                    internalName = arrayMatch.Groups["name"].Value;
                }
                field.InternalName = internalName;

                field.DisplayName = node.ReadStringOrDefault("DisplayName", field.InternalName);
                field.InternalType = node.ReadStringOrDefault("Enum", field.DisplayType.ToString());
                field.Description = node.ReadStringIfExist("Description");
                field.DisplayFormat = node.ReadStringOrDefault("DisplayFormat", ParamUtil.GetDefaultFormat(field.DisplayType));
                field.EditFlags = (EditFlags)Enum.Parse(typeof(EditFlags),
                    node.ReadStringOrDefault("EditFlags", ParamUtil.GetDefaultEditFlags(field.DisplayType).ToString()));
                field.Minimum = node.ReadSingleOrDefault("Minimum", ParamUtil.GetDefaultMinimum(field.DisplayType), CultureInfo.InvariantCulture);
                field.Maximum = node.ReadSingleOrDefault("Maximum", ParamUtil.GetDefaultMaximum(field.DisplayType), CultureInfo.InvariantCulture);
                field.Increment = node.ReadSingleOrDefault("Increment", ParamUtil.GetDefaultIncrement(field.DisplayType), CultureInfo.InvariantCulture);
                field.SortID = node.ReadInt32OrDefault("SortID", 0);

                field.UnkB8 = node.ReadStringIfExist("UnkB8");
                field.UnkC0 = node.ReadStringIfExist("UnkC0");
                field.UnkC8 = node.ReadStringIfExist("UnkC8");
                return field;
            }

            private static void SerializeField(Field field, XmlWriter xw)
            {
                string def = $"{field.DisplayType} {field.InternalName}";
                if (ParamUtil.IsBitType(field.DisplayType) && field.BitSize != -1)
                    def += $":{field.BitSize}";
                else if (ParamUtil.IsArrayType(field.DisplayType))
                    def += $"[{field.ArrayLength}]";

                if (field.Default != 0)
                    def += $" = {field.Default.ToString("R", CultureInfo.InvariantCulture)}";

                xw.WriteAttributeString("Def", def);
                xw.WriteDefaultElement("DisplayName", field.DisplayName, field.InternalName);
                xw.WriteDefaultElement("Enum", field.InternalType, field.DisplayType.ToString());
                xw.WriteDefaultElement("Description", field.Description, null);
                xw.WriteDefaultElement("DisplayFormat", field.DisplayFormat, ParamUtil.GetDefaultFormat(field.DisplayType));
                xw.WriteDefaultElement("EditFlags", field.EditFlags.ToString(), ParamUtil.GetDefaultEditFlags(field.DisplayType).ToString());
                xw.WriteDefaultElement("Minimum", field.Minimum, ParamUtil.GetDefaultMinimum(field.DisplayType), "R", CultureInfo.InvariantCulture);
                xw.WriteDefaultElement("Maximum", field.Maximum, ParamUtil.GetDefaultMaximum(field.DisplayType), "R", CultureInfo.InvariantCulture);
                xw.WriteDefaultElement("Increment", field.Increment, ParamUtil.GetDefaultIncrement(field.DisplayType), "R", CultureInfo.InvariantCulture);
                xw.WriteDefaultElement("SortID", field.SortID, 0);

                xw.WriteDefaultElement("UnkB8", field.UnkB8, null);
                xw.WriteDefaultElement("UnkC0", field.UnkC0, null);
                xw.WriteDefaultElement("UnkC8", field.UnkC8, null);
            }
        }
    }
}
