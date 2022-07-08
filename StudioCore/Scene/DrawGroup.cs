using System;
using System.Collections.Generic;
using System.Text;

namespace StudioCore.Scene
{
    public class DrawGroup
    {
        public bool AlwaysVisible { get; set; } = true;
        public uint[] Drawgroups { get; set; } = null;

        public DrawGroup()
        {
            AlwaysVisible = true;
        }

        public DrawGroup(uint[] groups)
        {
            AlwaysVisible = false;
            Drawgroups = groups;
        }

        public bool IsInDisplayGroup(DrawGroup disp)
        {
            if (AlwaysVisible || disp.AlwaysVisible || Drawgroups == null)
            {
                return true;
            }
            bool isAllZero = true;

            for (int i = 0; i < Drawgroups.Length && i < disp.Drawgroups.Length; i++)
            {
                if ((Drawgroups[i] & disp.Drawgroups[i]) != 0)
                {
                    return true;
                }
                if (Drawgroups[i] != 0)
                {
                    isAllZero = false;
                }
            }
            if (isAllZero)
            {
                return true;
            }
            return false;
        }
    }
}
