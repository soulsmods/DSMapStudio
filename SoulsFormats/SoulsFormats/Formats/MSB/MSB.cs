using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SoulsFormats
{
    /// <summary>
    /// Common classes and functions for MSB formats.
    /// </summary>
    public static partial class MSB
    {
        internal static void AssertHeader(BinaryReaderEx br)
        {
            br.AssertASCII("MSB ");
            br.AssertInt32(1);
            br.AssertInt32(0x10);
            br.AssertBoolean(false); // isBigEndian
            br.AssertBoolean(false); // isBitBigEndian
            br.AssertByte(1); // textEncoding
            br.AssertByte(0xFF); // is64BitOffset
        }

        internal static void WriteHeader(BinaryWriterEx bw)
        {
            bw.WriteASCII("MSB ");
            bw.WriteInt32(1);
            bw.WriteInt32(0x10);
            bw.WriteBoolean(false);
            bw.WriteBoolean(false);
            bw.WriteByte(1);
            bw.WriteByte(0xFF);
        }

        internal static void DisambiguateNames<T>(List<T> entries) where T : IMsbEntry
        {
            bool ambiguous;
            do
            {
                ambiguous = false;
                var nameCounts = new Dictionary<string, int>();
                foreach (IMsbEntry entry in entries)
                {
                    string name = entry.Name;
                    if (!nameCounts.ContainsKey(name))
                    {
                        nameCounts[name] = 1;
                    }
                    else
                    {
                        ambiguous = true;
                        nameCounts[name]++;
                        entry.Name = $"{name} {{{nameCounts[name]}}}";
                    }
                }
            }
            while (ambiguous);
        }

        internal static string ReambiguateName(string name)
        {
            return Regex.Replace(name, @" \{\d+\}", "");
        }

        internal static string FindName<T>(List<T> list, int index) where T : IMsbEntry
        {
            if (index == -1)
                return null;
            else
                return list[index].Name;
        }

        internal static string[] FindNames<T>(List<T> list, int[] indices) where T : IMsbEntry
        {
            var names = new string[indices.Length];
            for (int i = 0; i < indices.Length; i++)
                names[i] = FindName(list, indices[i]);
            return names;
        }

        internal static int FindIndex<T>(List<T> list, string name) where T : IMsbEntry
        {
            if (name == null)
            {
                return -1;
            }
            else
            {
                int result = list.FindIndex(entry => entry.Name == name);
                if (result == -1)
                    throw new KeyNotFoundException($"Name not found: {name}");
                return result;
            }
        }

        internal static int[] FindIndices<T>(List<T> list, string[] names) where T : IMsbEntry
        {
            var indices = new int[names.Length];
            for (int i = 0; i < names.Length; i++)
                indices[i] = FindIndex(list, names[i]);
            return indices;
        }
    }
}
