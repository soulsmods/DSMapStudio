using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Numerics;
using FSParam;
using ImGuiNET;
using SoulsFormats;

namespace StudioCore.ParamEditor
{
    public class ParamUtils
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
        public static bool RowMatches(Param.Row row, Param.Row vrow)
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
    }
}