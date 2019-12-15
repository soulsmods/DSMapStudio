using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using SoulsFormats;

namespace MSBTypeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var typ = typeof(MSB1);
            var classes = new List<Type>();
            classes.Add(typ.GetNestedType("Part"));
            classes.Add(typ.GetNestedType("Region"));
            classes.Add(typ.GetNestedType("Event"));

            var xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.Encoding = Encoding.UTF8;
            xws.NewLineChars = "\r\n";
            Console.OutputEncoding = Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(Console.Out, xws);
            xw.WriteStartElement("MSBDefinition");
            xw.WriteAttributeString("class", typ.FullName);

            foreach (var cl in classes)
            {
                var baseprops = cl.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var basepropsset = new HashSet<string>(baseprops.Select(x => x.Name));
                var subclasses = cl.Assembly.GetTypes().Where(type => type.IsSubclassOf(cl)).ToList();
                bool subclassed = true;
                if (subclasses.Count == 0)
                {
                    subclasses.Add(cl);
                    subclassed = false;
                }

                foreach (var s in subclasses)
                {
                    xw.WriteStartElement("MSBClass");
                    xw.WriteAttributeString("class", s.FullName);
                    xw.WriteStartElement("Category");
                    xw.WriteAttributeString("name", cl.Name);
                    foreach (var p in baseprops)
                    {
                        if (p.CanWrite)
                        {
                            xw.WriteElementString("Property", p.Name);
                        }
                    }
                    xw.WriteEndElement();
                    var props = s.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    if (subclassed && props.Length > baseprops.Length)
                    {
                        xw.WriteStartElement("Category");
                        xw.WriteAttributeString("name", s.Name);
                        foreach (var p in props)
                        {
                            if (basepropsset.Contains(p.Name) || !p.CanWrite)
                            {
                                continue;
                            }
                            xw.WriteElementString("Property", p.Name);
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();
            xw.Close();
        }
    }
}
