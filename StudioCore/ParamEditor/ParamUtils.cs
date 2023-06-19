using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Numerics;
using FSParam;
using ImGuiNET;
using SoulsFormats;
using System.Linq;

namespace StudioCore.ParamEditor
{
    public static class ParamUtils
    {
        public static string Dummy8Write(Byte[] dummy8)
        {
            string val = null;
            foreach (Byte b in dummy8)
            {
                if (val == null)
                    val = "["+b;
                else
                    val += "|"+b;
            }
            if (val == null)
                val = "[]";
            else
                val += "]";
            return val;
        }
        public static Byte[] Dummy8Read(string dummy8, int expectedLength)
        {
            Byte[] nval = new Byte[expectedLength];
            if (!(dummy8.StartsWith('[') && dummy8.EndsWith(']')))
                return null;
            string[] spl = dummy8.Substring(1, dummy8.Length-2).Split('|');
            if (nval.Length != spl.Length)
            {
                return null;
            }
            for (int i=0; i<nval.Length; i++)
            {
                if (!byte.TryParse(spl[i], out nval[i]))
                    return null;
            }
            return nval;
        }
        public static bool RowMatches(this Param.Row row, Param.Row vrow)
        {
            if (row.Def.ParamType != vrow.Def.ParamType || row.Def.DataVersion != vrow.Def.DataVersion)
                return false;
            
            return row.DataEquals(vrow);
        }
        public static bool ByteArrayEquals(byte[] v1, byte[] v2) {
            if (v1.Length!=v2.Length)
                return false;
            for (int i=0; i<v1.Length; i++)
            {
                if (v1[i]!=v2[i])
                    return false;
            }
            return true;
        }
        public static bool IsValueDiff(ref object value, ref object valueBase, Type t)
        {
            return value != null && valueBase != null && !(value.Equals(valueBase) || (t == typeof(byte[]) && ParamUtils.ByteArrayEquals((byte[])value, (byte[])valueBase)));
        }
        
        public static string ToParamEditorString(this object val)
        {
            return val.GetType() == typeof(byte[]) ? ParamUtils.Dummy8Write((byte[])val) : val.ToString();
        }

        public static object Get(this Param.Row row, (PseudoColumn, Param.Column) col)
        {
            return col.Item1 == PseudoColumn.ID ? row.ID : col.Item1 == PseudoColumn.Name ? row.Name : row[col.Item2].Value;
        }

        public static (PseudoColumn, Param.Column) GetCol(this Param param, string field)
        {
            PseudoColumn pc = field.Equals("ID") ? PseudoColumn.ID : field.Equals("Name") ? PseudoColumn.Name : PseudoColumn.None;
            Param.Column col = param?[field];
            return (pc, col);
        }

        public static (PseudoColumn, Param.Column) GetAs(this (PseudoColumn, Param.Column) col, Param newParam)
        {
            return (col.Item1, col.Item2 == null || newParam == null ? null : newParam.Cells.FirstOrDefault((x) => x.Def.InternalName == col.Item2.Def.InternalName && x.GetByteOffset() == col.Item2.GetByteOffset()));
        }
        public static bool IsColumnValid(this (PseudoColumn, Param.Column) col)
        {
            return col.Item1 != PseudoColumn.None || col.Item2 != null;
        }
        public static Type GetColumnType(this (PseudoColumn, Param.Column) col)
        {
            if (col.Item1 == PseudoColumn.ID)
                return typeof(int);
            else if (col.Item1 == PseudoColumn.Name)
                return typeof(string);
            else
                return col.Item2.ValueType;
        }
        public static string GetColumnSfType(this (PseudoColumn, Param.Column) col)
        {
            if (col.Item1 == PseudoColumn.ID)
                return "_int";
            else if (col.Item1 == PseudoColumn.Name)
                return "_string";
            else
                return col.Item2.Def.InternalType;
        }
        public static IEnumerable<(object, int)> GetParamValueDistribution(IEnumerable<Param.Row> rows, (PseudoColumn, Param.Column) col)
        {
            var vals = rows.Select((row, i) => row.Get(col));
            return vals.GroupBy((val) => val).Select((g) => (g.Key, g.Count()));
        }
        public static (float[], int, float, float) getCalcCorrectedData(CalcCorrectDefinition ccd, Param.Row row)
        {
            float[] stageMaxVal = ccd.stageMaxVal.Select((x, i) => (float)row[x].Value.Value).ToArray();
            float[] stageMaxGrowVal = ccd.stageMaxGrowVal.Select((x, i) => (float)row[x].Value.Value).ToArray();
            float[] adjPoint_maxGrowVal = ccd.adjPoint_maxGrowVal.Select((x, i) => (float)row[x].Value.Value).ToArray();

            int length = (int)(stageMaxVal[stageMaxVal.Length-1] - stageMaxVal[0] + 1);
            if (length <= 0 || length > 1000)
                return (new float[0], 0, 0, 0);
            float[] values = new float[length];
            for (int i=0; i<values.Length; i++)
            {
                float baseVal = i + stageMaxVal[0];
                int band = 0;
                while (band + 1 < stageMaxVal.Length && stageMaxVal[band + 1] < baseVal)
                    band++;
                if (band + 1 >= stageMaxVal.Length)
                    values[i] = stageMaxGrowVal[stageMaxGrowVal.Length-1];
                else
                {
                    float adjValRate = stageMaxVal[band] == stageMaxVal[band+1] ? 0 : (baseVal - stageMaxVal[band]) / (stageMaxVal[band+1] - stageMaxVal[band]);
                    float adjGrowValRate = adjPoint_maxGrowVal[band] >= 0 ? (float)Math.Pow(adjValRate, adjPoint_maxGrowVal[band]) : 1 - (float)Math.Pow(1 - adjValRate, -adjPoint_maxGrowVal[band]);
                    values[i] = adjGrowValRate * (stageMaxGrowVal[band+1] - stageMaxGrowVal[band]) + stageMaxGrowVal[band];
                }
            }
            return (values, (int)stageMaxVal[0], stageMaxGrowVal[0], stageMaxGrowVal[stageMaxGrowVal.Length-1]);
        }
        public static (float[], float) getSoulCostData(SoulCostDefinition scd, Param.Row row)
        {
            float init_inclination_soul = (float)row[scd.init_inclination_soul].Value.Value;
            float adjustment_value = (float)row[scd.adjustment_value].Value.Value;
            float boundry_inclination_soul = (float)row[scd.boundry_inclination_soul].Value.Value;
            float boundry_value = (float)row[scd.boundry_value].Value.Value;

            float[] values = new float[scd.max_level_for_game + 1];
            for (int level=0; level<values.Length; level++)
            {
                int level80 = level + 80;
                int level80S = level80 * level80;
                float boundry = Math.Max(0, level80 - boundry_value);
                float lateScaling = level80S * boundry_inclination_soul * boundry;
                float earlyScaling = level80S * init_inclination_soul;
                values[level] = float.Floor(lateScaling + earlyScaling + adjustment_value);
            }
            return (values, values[values.Length-1]);
        }
    }

    public enum PseudoColumn
    {
        None,
        ID,
        Name
    }
}