using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace StudioCore.Gui
{
    /// <summary>
    /// A self contained view that contains a self contained functional GUI
    /// </summary>
    public abstract class GuiView
    {
        /// <summary>
        /// The Rect that contains this view
        /// </summary>
        protected Rectangle Bounds;

        public abstract void OnGui();
    }
}
