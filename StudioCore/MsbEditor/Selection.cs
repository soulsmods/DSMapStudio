using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.MsbEditor
{
    public static class Selection
    {
        private static HashSet<Scene.ISelectable> _selected = new HashSet<Scene.ISelectable>();

        public static bool IsSelection()
        {
            return _selected.Count > 0;
        }

        public static bool IsFilteredSelection<T>() where T : Scene.ISelectable
        {
            return GetFilteredSelection<T>().Count > 0;
        }

        public static bool IsFilteredSelection<T>(Func<T, bool> filt) where T : Scene.ISelectable
        {
            return GetFilteredSelection<T>(filt).Count > 0;
        }

        public static bool IsSingleSelection()
        {
            return _selected.Count == 1;
        }

        public static bool IsSingleFilteredSelection<T>() where T : Scene.ISelectable
        {
            return GetFilteredSelection<T>().Count == 1;
        }

        public static bool IsSingleFilteredSelection<T>(Func<T, bool> filt) where T : Scene.ISelectable
        {
            return GetFilteredSelection<T>(filt).Count == 1;
        }

        public static Scene.ISelectable GetSingleSelection()
        {
            if (IsSingleSelection())
            {
                return _selected.First();
            }
            return null;
        }

        public static T GetSingleFilteredSelection<T>() where T : Scene.ISelectable
        {
            var filt = GetFilteredSelection<T>();
            if (filt.Count() == 1)
            {
                return filt.First();
            }
            return default(T);
        }

        public static T GetSingleFilteredSelection<T>(Func<T, bool> filt) where T : Scene.ISelectable
        {
            var f = GetFilteredSelection<T>(filt);
            if (f.Count() == 1)
            {
                return f.First();
            }
            return default(T);
        }

        public static HashSet<Scene.ISelectable> GetSelection()
        {
            return _selected;
        }

        public static HashSet<T> GetFilteredSelection<T>() where T : Scene.ISelectable
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

        public static HashSet<T> GetFilteredSelection<T>(Func<T, bool> filt) where T : Scene.ISelectable
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

        public static void ClearSelection()
        {
            foreach (var sel in _selected)
            {
                sel.OnDeselected();
            }
            _selected.Clear();
        }

        public static void AddSelection(Scene.ISelectable selected)
        {
            if (selected != null)
            {
                selected.OnSelected();
                _selected.Add(selected);
            }
        }

        public static void AddSelection(List<Scene.ISelectable> selected)
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
    }
}
