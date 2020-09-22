using System;
using SoulsFormats.XmlExtensions;
using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace HKXClassGen
{
    class Program
    {
        public static StreamWriter CurrentFile;
        public static int CurrentIndent = 0;

        public static Dictionary<string, HKXClass> Classes = new Dictionary<string, HKXClass>();

        public static void PushIndent()
        {
            CurrentIndent++;
        }

        public static void PopIndent()
        {
            CurrentIndent--;
        }

        public static void WriteLine(string s)
        {
            string indent = "";
            for (int i = 0; i < CurrentIndent; i++)
            {
                indent += "    ";
            }
            CurrentFile.WriteLine(indent + s);
        }

        public class HKXClass
        {
            public string ParentName;
            public uint Signature;
            public List<HKXEnum> Enums = new List<HKXEnum>();
            public List<HKXMember> Members = new List<HKXMember>();
        }

        public class HKXMember
        {
            public string Name;
            public int Offset;
            public string VType;
            public string VSubType;
            public string CType;
            public string EType;
            public int ArraySize;
            public string Flags;
        }

        public class HKXEnum
        {
            public string Name;
            public Dictionary<string, int> EnumValues = new Dictionary<string, int>();
        }

        public static void ParseMembers(XmlNodeList nodes, HKXClass hkclass)
        {
            foreach (XmlNode n in nodes)
            {
                HKXMember m = new HKXMember();
                m.Name = n.ReadStringAttribute("name");
                m.Offset = n.ReadInt32Attribute("offset");
                m.VType = n.ReadStringAttribute("vtype");
                m.VSubType = n.ReadStringAttribute("vsubtype");
                m.CType = n.ReadStringAttributeOrDefault("ctype");
                m.EType = n.ReadStringAttributeOrDefault("etype");
                m.ArraySize = n.ReadInt32Attribute("arrsize");
                m.Flags = n.ReadStringAttribute("flags");
                hkclass.Members.Add(m);
            }
        }

        public static void ParseEnums(XmlNodeList nodes, HKXClass hkclass)
        {
            foreach (XmlNode n in nodes)
            {
                HKXEnum e = new HKXEnum();
                e.Name = n.ReadStringAttribute("name");
                foreach (XmlNode c in n.ChildNodes)
                {
                    string name = c.ReadStringAttribute("name");
                    int val = c.ReadInt32Attribute("value");
                    e.EnumValues.Add(name, val);
                }
                hkclass.Enums.Add(e);
            }
        }

        public static HKXClass ParseClass(XmlDocument doc)
        {
            var cls = doc.FirstChild;
            var hkclass = new HKXClass();
            hkclass.Signature = cls.ReadUInt32HexAttribute("signature");
            hkclass.ParentName = cls.ReadStringAttributeOrDefault("parent");

            var enums = cls.SelectSingleNode("enums");
            if (enums != null)
            {
                ParseEnums(enums.ChildNodes, hkclass);
            }

            ParseMembers(cls.SelectSingleNode("members").ChildNodes, hkclass);

            return hkclass;
        }

        public static void WriteEnum(HKXEnum e)
        {
            WriteLine("public enum " + e.Name);
            WriteLine("{");
            PushIndent();

            foreach (var m in e.EnumValues)
            {
                WriteLine($@"{m.Key} = {m.Value},");
            }

            PopIndent();
            WriteLine("}");
        }

        public static string ReduceType(HKXMember m, string t, bool v = false)
        {
            string r = t;
            if (t == "TYPE_ENUM")
            {
                r = m.EType;
            }
            else if (t == "TYPE_CSTRING")
            {
                r = "string";
            }
            else if (t == "TYPE_STRINGPTR")
            {
                r = "string";
            }
            else if (t == "TYPE_UINT8")
            {
                r = "byte";
            }
            else if (t == "TYPE_INT8")
            {
                r = "char";
            }
            else if (t == "TYPE_CHAR")
            {
                r = "char";
            }
            else if (t == "TYPE_UINT16")
            {
                r = "ushort";
            }
            else if (t == "TYPE_INT16")
            {
                r = "short";
            }
            else if (t == "TYPE_HALF")
            {
                r = "ushort";
            }
            else if (t == "TYPE_UINT32")
            {
                r = "uint";
            }
            else if (t == "TYPE_INT32")
            {
                r = "int";
            }
            else if (t == "TYPE_ULONG")
            {
                r = "ulong";
            }
            else if (t == "TYPE_UINT64")
            {
                r = "ulong";
            }
            else if (t == "TYPE_LONG")
            {
                r = "long";
            }
            else if (t == "TYPE_INT64")
            {
                r = "long";
            }
            else if (t == "TYPE_INT32")
            {
                r = "int";
            }
            else if (t == "TYPE_BOOL")
            {
                r = "bool";
            }
            else if (t == "TYPE_FLAGS")
            {
                r = "uint";
            }
            else if (t == "TYPE_REAL")
            {
                r = "float";
            }
            else if (t == "TYPE_QUATERNION")
            {
                r = "Quaternion";
            }
            else if (t == "TYPE_ROTATION")
            {
                // Not even sure what this is but I don't think Souls even uses classes with this
                // type so whatever lmao
                r = "Quaternion";
            }
            else if (t == "TYPE_VECTOR4")
            {
                r = "Vector4";
            }
            else if (t == "TYPE_MATRIX4")
            {
                r = "Matrix4x4";
            }
            else if (t == "TYPE_MATRIX3")
            {
                r = "Matrix4x4";
            }
            else if (t == "TYPE_TRANSFORM")
            {
                r = "Matrix4x4";
            }
            else if (t == "TYPE_QSTRANSFORM")
            {
                r = "hkQTransform";
            }
            else if (t == "TYPE_POINTER")
            {
                if (v)
                {
                    if (m.CType != null)
                    {
                        r = ReduceType(m, m.CType);
                    }
                    else
                    {
                        r = "void*";
                    }
                    return r;
                }
                r = ReduceType(m, m.VSubType, true);
            }
            else if (t == "TYPE_ARRAY")
            {
                r = "List<" + ReduceType(m, m.VSubType, true) + ">";
            }
            else if (t == "TYPE_RELARRAY")
            {
                r = "List<" + ReduceType(m, m.VSubType, true) + ">";
            }
            else if (t == "TYPE_SIMPLEARRAY")
            {
                r = "List<" + ReduceType(m, m.VSubType, true) + ">";
            }
            else if (t == "TYPE_STRUCT")
            {
                r = ReduceType(m, m.CType);
            }
            return r;
        }

        public static void WriteMember(HKXMember m)
        {
            if (m.CType != null && m.CType == "hkxMesh")
            {
                return;
            }
            var n = "m_" + m.Name;

            string type = ReduceType(m, m.VType);
            WriteLine("public " + type + " " + n + ";");
        }

        public static bool IsSubEnum(string e)
        {
            return (e == "ScaleNormalBehavior" ||
                e == "Type" ||
                e == "Flags" ||
                e == "FlagValues" ||
                e == "FlagsEnum" ||
                e == "Axis" ||
                e == "ScaleNormalBehaviour" ||
                e == "ControlByte" ||
                e == "SetAngleMethod" ||
                e == "RotationAxisCoordinates" ||
                e == "Component" ||
                e == "InternalFlags" ||
                e == "HandleChangeMode");
        }

        public static void WriteClass(string name, HKXClass cls)
        {
            WriteLine("namespace HKX2\r\n{");
            PushIndent();

            foreach (var e in cls.Enums)
            {
                if (!IsSubEnum(e.Name) && name != "hknpShapeType")
                {
                    WriteEnum(e);
                    WriteLine("");
                }
            }

            WriteLine("public class " + name + (cls.ParentName != null ? " : " + cls.ParentName : ""));
            WriteLine("{");
            PushIndent();

            foreach (var e in cls.Enums)
            {
                if (IsSubEnum(e.Name) || name == "hknpShapeType")
                {
                    WriteEnum(e);
                    WriteLine("");
                }
            }

            foreach (var m in cls.Members)
            {
                if (m.Flags != null && !m.Flags.Contains("SERIALIZE_IGNORED"))
                {
                    WriteMember(m);
                }
            }

            PopIndent();
            WriteLine("}");

            PopIndent();
            WriteLine("}");
        }

        static void Main(string[] args)
        {
            var classfiles = Directory.GetFileSystemEntries($@"classxmlds3", @"*.xml").ToList();
            foreach (var f in classfiles)
            {
                var xml = new XmlDocument();
                xml.Load(f);
                var c = ParseClass(xml);
                string name = Path.GetFileNameWithoutExtension(f);
                if (name.IndexOf('_') != -1)
                {
                    name = name.Substring(0, name.IndexOf('_'));
                }
                Classes.Add(name, c);
            }

            foreach (var c in Classes)
            {
                Directory.CreateDirectory("out");
                using (CurrentFile = new StreamWriter($@"out\{c.Key}.cs"))
                {
                    WriteLine("using System.Collections.Generic;");
                    WriteLine("using System.Numerics;");
                    WriteLine("");
                    WriteClass(c.Key, c.Value);
                }
            }
        }
    }
}
