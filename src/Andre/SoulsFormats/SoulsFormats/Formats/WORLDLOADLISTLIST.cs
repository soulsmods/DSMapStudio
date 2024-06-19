using System.Collections.Generic;

namespace SoulsFormats
{
    /// <summary>
    /// A text file of maps to load introduced in ER. Extension: .worldloadlistlist
    /// </summary>
    public class WORLDLOADLISTLIST : SoulsFile<WORLDLOADLISTLIST>
    {
        /// <summary>
        /// Map entries for the game to load
        /// </summary>
        public List<MapEntry> MapEntries { get; set; }

        /// <summary>
        /// Creates an empty WORLDLOADLISTLIST.
        /// </summary>
        public WORLDLOADLISTLIST()
        {
            MapEntries = new List<MapEntry>();
        }

        /// <summary>
        /// Checks whether the data appears to be a file of this format.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 4)
                return false;

            string magic = br.GetASCII(10, 4);
            return magic == "map:";
        }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;

            //There are 138 lines in the worldloadlist, unknown if more can be added without side effects
            for (int i = 0; i < 138; i++)
            {
                string line = br.ReadUTF8Line();
                if (!string.IsNullOrEmpty(line))
                {
                    MapEntries.Add(new MapEntry(line));
                }
                else
                {
                    MapEntries.Add(null);
                } 
            }
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;
            //There are 138 lines in the worldloadlist, unknown if more can be added without side effects
            for (int i = 0; i < 138; i++)
            {
                if (MapEntries[i] == null)
                {
                    bw.WriteUTF8("", true);
                }
                else
                {
                    string text = "map:/MapStudio/" + MapEntries[i].Id + ".msb";
                    if (MapEntries[i].IsTestMap)
                    {
                        text += ",testmap=1";
                    }
                    bw.WriteUTF8(text, true);
                }
            }
        }

        public void InsertNewMapEntry(string id, bool isTestMap) {
            //Begin searching for empty lines and insert new line
            for (int i = 0; i < MapEntries.Count; i++)
            {
                if (MapEntries[i] == null)
                {
                    MapEntries[i] = new MapEntry(id, isTestMap);
                    break;
                }
            }
        }

        /// <summary>
        /// A map entry.
        /// </summary>
        public class MapEntry
        {
            /// <summary>
            /// Map Id.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Unknown how/if this is used.
            /// </summary>
            public bool IsTestMap { get; set; } = false;

            internal MapEntry(string mapEntryText)
            {
                Id = mapEntryText.Substring(15,12);
                if (mapEntryText.Length >= 40)
                { 
                    IsTestMap = mapEntryText[40] == '1';
                }
            }

            internal MapEntry(string id, bool isTestMap)
            {
                Id = id;
                IsTestMap = isTestMap;
            }
        }
    }
}
