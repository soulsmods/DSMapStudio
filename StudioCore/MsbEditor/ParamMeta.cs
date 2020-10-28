using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Numerics;
using ImGuiNET;
using SoulsFormats;
using System.Xml;

namespace StudioCore.MsbEditor
{
    public class ParamMetaData
    {
        private static Dictionary<PARAMDEF, ParamMetaData> _ParamMetas = new Dictionary<PARAMDEF, ParamMetaData>();

        private const int XML_VERSION = 0;

        public static ParamMetaData Get(PARAMDEF def)
        {
            return _ParamMetas[def];
        }

        private static void Add(PARAMDEF key, ParamMetaData meta)
        {
            _ParamMetas.Add(key, meta);
        }

        private ParamMetaData(PARAMDEF def)
        {
            Add(def, this);
            foreach (PARAMDEF.Field f in def.Fields)
                new FieldMetaData(f);
            // Blank Metadata
        }

        private ParamMetaData(XmlDocument xml, PARAMDEF def)
        {
            XmlNode root = xml.SelectSingleNode("PARAMMETA");
            int xmlVersion = int.Parse(root.Attributes["XmlVersion"].InnerText);
            if (xmlVersion != XML_VERSION)
            {
                throw new InvalidDataException($"Mismatched XML version; current version: {XML_VERSION}, file version: {xmlVersion}");
            }
            Add(def, this);

            foreach (PARAMDEF.Field f in def.Fields)
            {
                XmlNode pairedNode = root.SelectSingleNode($"Field/{f.InternalName}");
                if (pairedNode == null)
                {
                    new FieldMetaData(f);
                    continue;
                }
                new FieldMetaData(pairedNode, f);
            }
        }

        public static ParamMetaData XmlDeserialize(string path, PARAMDEF def)
        {
            if (!File.Exists(path))
            {
                return new ParamMetaData(def);
            }
            var mxml = new XmlDocument();
            try
            {
                mxml.Load(path);
                return new ParamMetaData(mxml, def);
            }
            catch
            {
                return new ParamMetaData(def);
            }
        }
    }

    public class FieldMetaData
    {
        private static Dictionary<PARAMDEF.Field, FieldMetaData> _FieldMetas = new Dictionary<PARAMDEF.Field, FieldMetaData>();

        /// <summary>
        /// Name of another Param that a Field may refer to.
        /// </summary>
        public List<string> RefTypes { get; set; }

        /// <summary>
        /// Name linking fields from multiple params that may share values.
        /// </summary>
        public string VirtualRef {get; set;}

        public static FieldMetaData Get(PARAMDEF.Field def)
        {
            return _FieldMetas[def];
        }

        private static void Add(PARAMDEF.Field key, FieldMetaData meta)
        {
            _FieldMetas.Add(key, meta);
        }

        public FieldMetaData(PARAMDEF.Field field)
        {
            Add(field, this);
            // Blank Metadata
        }

        public FieldMetaData(XmlNode fieldMeta, PARAMDEF.Field field)
        {
            Add(field, this);
            RefTypes = null;
            VirtualRef = null;
            XmlAttribute Ref = fieldMeta.Attributes["Refs"];
            if (Ref != null)
            {
                RefTypes = new List<string>(Ref.InnerText.Split(","));
            }
            XmlAttribute VRef = fieldMeta.Attributes["VRef"];
            if (VRef != null)
            {
                VirtualRef = VRef.InnerText;
            }
        }
    }
}