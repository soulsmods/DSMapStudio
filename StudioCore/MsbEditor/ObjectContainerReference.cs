using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Reference to a top-level container entity, regardless of whether it is loaded or not.
    /// </summary>
    public class ObjectContainerReference : Scene.ISelectable
    {
        public string Name { get; set; }

        private Universe Universe;

        public ObjectContainerReference(string name, Universe universe)
        {
            Name = name;
            Universe = universe;
        }

        public Scene.ISelectable GetSelectionTarget()
        {
            if (Universe != null
                && Universe.LoadedObjectContainers.TryGetValue(Name, out ObjectContainer container)
                && container?.RootObject != null)
            {
                return container.RootObject;
            }
            return this;
        }

        public void OnSelected()
        {
            // No visual change from selection
        }

        public void OnDeselected()
        {
            // No visual change from selection
        }

        public override int GetHashCode() => Name.GetHashCode();
        public override bool Equals(object obj) => obj is ObjectContainerReference o && Name.Equals(o.Name);
    }
}
