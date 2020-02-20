using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Because fromsoft divided enemy placement into two params in DS2, this class exists
    /// to combine them into a "superparam" that maintains the two param rows under the hood
    /// </summary>
    public class MergedParamRow
    {
        public long ID
        {
            get
            {
                return (Rows.Count > 0) ? Rows[0].ID : -1;
            }
            set
            {
                foreach (var r in Rows)
                {
                    r.ID = value;
                }
            }
        }

        public string Name
        {
            get
            {
                return (Rows.Count > 0) ? Rows[0].Name : null;
            }
            set
            {
                foreach (var r in Rows)
                {
                    r.Name = value;
                }
            }
        }

        public List<PARAM.Cell> Cells
        {
            get
            {
                var ret = new List<PARAM.Cell>();
                foreach (var r in Rows)
                {
                    ret.AddRange(r.Cells);
                }
                return ret;
            }
        }

        private List<PARAM.Row> Rows = new List<PARAM.Row>();
        private Dictionary<string, PARAM.Row> RowMap = new Dictionary<string, PARAM.Row>();

        public MergedParamRow() { }

        public MergedParamRow(MergedParamRow clone)
        {
            foreach (var entry in clone.RowMap)
            {
                var n = new PARAM.Row(entry.Value);
                RowMap.Add(entry.Key, n);
                Rows.Add(n);
            }
        }

        public void AddRow(string key, PARAM.Row row)
        {
            if (Rows.Count > 0 && row.ID != Rows[0].ID)
            {
                throw new ArgumentException("All IDs must match for param rows to be merged");
            }
            RowMap.Add(key, row);
            Rows.Add(row);
        }

        public PARAM.Row GetRow(string key)
        {
            if (RowMap.ContainsKey(key))
            {
                return RowMap[key];
            }
            return null;
        }

        public PARAM.Cell this[string name]
        {
            get
            {
                foreach (var r in Rows)
                {
                    if (r.Cells.FirstOrDefault(cell => cell.Def.InternalName == name) != null)
                    {
                        return r[name];
                    }
                }
                return null;
            }
        }
    }
}
