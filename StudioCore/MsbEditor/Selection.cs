using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.MsbEditor
{
    public class Selection
    {
        private HashSet<Scene.ISelectable> _selected = new HashSet<Scene.ISelectable>();

        public bool IsSelection()
        {
            return _selected.Count > 0;
        }

        public bool IsFilteredSelection<T>() where T : Scene.ISelectable
        {
            return GetFilteredSelection<T>().Count > 0;
        }

        public bool IsFilteredSelection<T>(Func<T, bool> filt) where T : Scene.ISelectable
        {
            return GetFilteredSelection<T>(filt).Count > 0;
        }

        public bool IsSingleSelection()
        {
            return _selected.Count == 1;
        }

        public bool IsMultiSelection()
        {
            return _selected.Count > 1;
        }

        public bool IsSingleFilteredSelection<T>() where T : Scene.ISelectable
        {
            return GetFilteredSelection<T>().Count == 1;
        }

        public bool IsSingleFilteredSelection<T>(Func<T, bool> filt) where T : Scene.ISelectable
        {
            return GetFilteredSelection<T>(filt).Count == 1;
        }

        public Scene.ISelectable GetSingleSelection()
        {
            if (IsSingleSelection())
            {
                return _selected.First();
            }
            return null;
        }

        public T GetSingleFilteredSelection<T>() where T : Scene.ISelectable
        {
            var filt = GetFilteredSelection<T>();
            if (filt.Count() == 1)
            {
                return filt.First();
            }
            return default(T);
        }

        public T GetSingleFilteredSelection<T>(Func<T, bool> filt) where T : Scene.ISelectable
        {
            var f = GetFilteredSelection<T>(filt);
            if (f.Count() == 1)
            {
                return f.First();
            }
            return default(T);
        }

        public HashSet<Scene.ISelectable> GetSelection()
        {
            return _selected;
        }

        public HashSet<T> GetFilteredSelection<T>() where T : Scene.ISelectable
        {
            var filtered = new HashSet<T>();
            foreach (var sel in _selected)
            {
                if (sel is T filsel)
                {
                    filtered.Add(filsel);
                }
            }
            return filtered;
        }

        public HashSet<T> GetFilteredSelection<T>(Func<T, bool> filt) where T : Scene.ISelectable
        {
            var filtered = new HashSet<T>();
            foreach (var sel in _selected)
            {
                if (sel is T filsel && filt.Invoke(filsel))
                {
                    filtered.Add(filsel);
                }
            }
            return filtered;
        }

        public void ClearSelection()
        {
            foreach (var sel in _selected)
            {
                sel.OnDeselected();
            }
            _selected.Clear();
        }

        public void AddSelection(Scene.ISelectable selected)
        {
            if (selected != null)
            {
                selected.OnSelected();
                _selected.Add(selected);
            }
        }

        public void AddSelection(List<Scene.ISelectable> selected)
        {
            foreach (var sel in selected)
            {
                if (sel != null)
                {
                    sel.OnSelected();
                    _selected.Add(sel);
                }
            }
        }

        public bool IsSelected(Scene.ISelectable selected)
        {
            foreach (var sel in _selected)
            {
                if (sel == selected)
                {
                    return true;
                }
            }
            return false;
        }

        // State for SceneTree auto-scroll, as these are set at the same time as selections or using selections.
        // This is processed by SceneTree and cleared as soon as the goto is complete, or no goto target was found.
        //
        // More advanced functionality could be added to expand TreeNodes to show the entity, but this requires
        // tracking even more state in SceneTree, as well as path-from-root metadata for an entity. This should
        // probably be split out of Selection at that point (IGotoTarget, perhaps).
        public Scene.ISelectable GotoTreeTarget { get; set; }

        public bool ShouldGoto(Scene.ISelectable selected) => selected != null && selected.Equals(GotoTreeTarget);

        public void ClearGotoTarget() => GotoTreeTarget = null;
    }
}
