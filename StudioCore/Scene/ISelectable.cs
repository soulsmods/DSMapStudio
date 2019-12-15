using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Scene
{
    /// <summary>
    /// An abstract object held by a render object that can be selected
    /// </summary>
    public interface ISelectable
    {
        public void OnSelected();
        public void OnDeselected();
    }
}
