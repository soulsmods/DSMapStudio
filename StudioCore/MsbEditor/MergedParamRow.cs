using System;
using System.Collections.Generic;
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

        public MergedParamRow() { }

        public MergedParamRow(MergedParamRow clone)
        {
            foreach (var row in clone.Rows)
            {
                Rows.Add(new PARAM.Row(row));
            }
        }

        public void AddRow(PARAM.Row row)
        {
            if (Rows.Count > 0 && row.ID != Rows[0].ID)
            {
                throw new ArgumentException("All IDs must match for param rows to be merged");
            }
            Rows.Add(row);
        }

        public PARAM.Cell this[string name]
        {
            get
            {
                foreach (var r in Rows)
                {
                    if (r[name] != null)
                    {
                        return r[name];
                    }
                }
                return null;
            }
        }
    }
}
