﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSParam;
using SoulsFormats;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Because fromsoft divided enemy placement into two params in DS2, this class exists
    /// to combine them into a "superparam" that maintains the two param rows under the hood
    /// </summary>
    public class MergedParamRow
    {
        public int ID
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

        public List<Param.Cell> CellHandles
        {
            get
            {
                var ret = new List<Param.Cell>();
                foreach (var r in Rows)
                {
                    ret.AddRange(r.Cells);
                }
                return ret;
            }
        }

        private List<Param.Row> Rows = new List<Param.Row>();
        private Dictionary<string, Param.Row> RowMap = new Dictionary<string, Param.Row>();

        public MergedParamRow() { }

        public MergedParamRow(MergedParamRow clone)
        {
            foreach (var entry in clone.RowMap)
            {
                var n = new Param.Row(entry.Value);
                RowMap.Add(entry.Key, n);
                Rows.Add(n);
            }
        }

        public void AddRow(string key, Param.Row row)
        {
            if (Rows.Count > 0 && row.ID != Rows[0].ID)
            {
                throw new ArgumentException("All IDs must match for param rows to be merged");
            }
            RowMap.Add(key, row);
            Rows.Add(row);
        }

        public Param.Row GetRow(string key)
        {
            if (RowMap.ContainsKey(key))
            {
                return RowMap[key];
            }
            return null;
        }

        public Param.Cell? this[string name]
        {
            get
            {
                foreach (var r in Rows)
                {
                    if (r.Columns.FirstOrDefault(cell => cell.Def.InternalName == name) != null)
                    {
                        return r[name];
                    }
                }
                return null;
            }
        }
    }
}
