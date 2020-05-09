using System;
using System.Collections.Generic;
using System.Text;

namespace StudioCore.Scene
{
    /// <summary>
    /// Simple interface for a modal dialogue
    /// </summary>
    interface IModal
    {
        public bool IsClosed { get; }
        public void OpenModal();
        public void OnGui();
    }
}
