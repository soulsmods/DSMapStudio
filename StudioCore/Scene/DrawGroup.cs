using System;
using System.Collections.Generic;
using System.Text;

namespace StudioCore.Scene
{
    public class DrawGroup
    {
        public bool AlwaysVisible { get; set; } = true;
        public uint[] RenderGroups { get; set; } = null;

        public DrawGroup()
        {
            AlwaysVisible = true;
        }

        public DrawGroup(uint[] groups)
        {
            AlwaysVisible = false;
            RenderGroups = groups;
        }

        public bool IsInDisplayGroup(DrawGroup disp)
        {
            if (AlwaysVisible || disp.AlwaysVisible || RenderGroups == null)
            {
                return true;
            }
            bool isAllZero = true;

            for (int i = 0; i < RenderGroups.Length && i < disp.RenderGroups.Length; i++)
            {
                if ((RenderGroups[i] & disp.RenderGroups[i]) != 0)
                {
                    return true;
                }
                if (RenderGroups[i] != 0)
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
