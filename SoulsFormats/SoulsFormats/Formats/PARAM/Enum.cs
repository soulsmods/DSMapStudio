using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoulsFormats
{
    public partial class PARAM
    {
        /// <summary>
        /// A collection of named values.
        /// </summary>
        public class Enum : List<Enum.Item>
        {
            /// <summary>
            /// The type of values in this enum, which should match the types of the cells using it.
            /// </summary>
            public CellType Type { get; }

            /// <summary>
            /// Creates a new empty Enum with the given type.
            /// </summary>
            public Enum(CellType type) : base()
            {
                Type = type;
            }

            internal Enum(XmlNode node)
            {
                Type = (CellType)System.Enum.Parse(typeof(CellType), node.Attributes["type"].InnerText);
                foreach (XmlNode itemNode in node.SelectNodes("item"))
                {
                    string itemName = itemNode.Attributes["name"].InnerText;
                    object itemValue = Layout.ParseParamValue(Type, itemNode.Attributes["value"].InnerText);
                    Add(new Item(itemName, itemValue));
                }
            }

            /// <summary>
            /// A value and corresponding name in an Enum.
            /// </summary>
            public class Item
            {
                /// <summary>
                /// The name of the value.
                /// </summary>
                public string Name { get; }

                /// <summary>
                /// The value of the value.
                /// </summary>
                public object Value { get; }

                /// <summary>
                /// Creates a new Item with the given properties.
                /// </summary>
                public Item(string name, object value)
                {
                    Name = name;
                    Value = value;
                }
            }
        }
    }
}
