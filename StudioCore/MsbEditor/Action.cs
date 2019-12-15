using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// An action that can be performed by the user in the editor that represents
    /// a single atomic editor action that affects the state of the map. Each action
    /// should have enough information to apply the action AND undo the action, as
    /// these actions get pushed to a stack for undo/redo
    /// </summary>
    public abstract class Action
    {
        abstract public void Execute();
        abstract public void Undo();
    }

    public class PropertiesChangedAction : Action
    {
        private class PropertyChange
        {
            public PropertyInfo Property;
            public object OldValue;
            public object NewValue;
        }

        private object ChangedObject;
        private List<PropertyChange> Changes = new List<PropertyChange>();
        private Action<bool> PostExecutionAction = null;

        public PropertiesChangedAction(object changed)
        {
            ChangedObject = changed;
        }

        public PropertiesChangedAction(PropertyInfo prop, object changed, object newval)
        {
            ChangedObject = changed;
            var change = new PropertyChange();
            change.Property = prop;
            change.OldValue = prop.GetValue(ChangedObject);
            change.NewValue = newval;
            Changes.Add(change);
        }

        public void AddPropertyChange(PropertyInfo prop, object newval)
        {
            var change = new PropertyChange();
            change.Property = prop;
            change.OldValue = prop.GetValue(ChangedObject);
            change.NewValue = newval;
            Changes.Add(change);
        }

        public void SetPostExecutionAction(Action<bool> action)
        {
            PostExecutionAction = action;
        }

        public override void Execute()
        {
            foreach (var change in Changes)
            {
                change.Property.SetValue(ChangedObject, change.NewValue);
            }
            PostExecutionAction.Invoke(false);
        }

        public override void Undo()
        {
            foreach (var change in Changes)
            {
                change.Property.SetValue(ChangedObject, change.OldValue);
            }
            PostExecutionAction.Invoke(true);
        }
    }

    public class CloneMapObjectsAction : Action
    {
        private Map Map;
        private Scene.RenderScene Scene;
        private List<MapObject> Clonables = new List<MapObject>();
        private List<MapObject> Clones = new List<MapObject>();
        private bool SetSelection;

        public CloneMapObjectsAction(Map map, Scene.RenderScene scene, List<MapObject> objects, bool setSelection)
        {
            Map = map;
            Scene = scene;
            Clonables.AddRange(objects);
            SetSelection = setSelection;
        }

        public override void Execute()
        {
            foreach (var obj in Clonables)
            {
                var newobj = obj.Clone();
                newobj.Name = obj.Name + "_1";
                Map.MapObjects.Insert(Map.MapObjects.IndexOf(obj) + 1, newobj);
                newobj.UpdateRenderTransform();
                newobj.RenderSceneMesh.Selectable = newobj;
                Clones.Add(newobj);
            }
            if (SetSelection)
            {
                Selection.ClearSelection();
                foreach (var c in Clones)
                {
                    Selection.AddSelection(c);
                }
            }
        }

        public override void Undo()
        {
            foreach (var clone in Clones)
            {
                Map.MapObjects.Remove(clone);
                clone.RenderSceneMesh.AutoRegister = false;
                clone.RenderSceneMesh.UnregisterWithScene();
            }
            Clones.Clear();
            if (SetSelection)
            {
                Selection.ClearSelection();
                foreach (var c in Clonables)
                {
                    Selection.AddSelection(c);
                }
            }
        }
    }

    public class DeleteMapObjectsAction : Action
    {
        private Map Map;
        private Scene.RenderScene Scene;
        private List<MapObject> Deletables = new List<MapObject>();
        private List<int> RemoveIndices = new List<int>();
        private bool SetSelection;

        public DeleteMapObjectsAction(Map map, Scene.RenderScene scene, List<MapObject> objects, bool setSelection)
        {
            Map = map;
            Scene = scene;
            Deletables.AddRange(objects);
            SetSelection = setSelection;
        }

        public override void Execute()
        {
            foreach (var obj in Deletables)
            {
                RemoveIndices.Add(Map.MapObjects.IndexOf(obj));
                Map.MapObjects.RemoveAt(RemoveIndices.Last());
                obj.RenderSceneMesh.AutoRegister = false;
                obj.RenderSceneMesh.UnregisterWithScene();
            }
            if (SetSelection)
            {
                Selection.ClearSelection();
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < Deletables.Count(); i++)
            {
                Map.MapObjects.Insert(RemoveIndices[i], Deletables[i]);
                Deletables[i].RenderSceneMesh.AutoRegister = true;
                Deletables[i].RenderSceneMesh.RegisterWithScene(Scene);
            }
            if (SetSelection)
            {
                Selection.ClearSelection();
                foreach (var d in Deletables)
                {
                    Selection.AddSelection(d);
                }
            }
        }
    }

    public class CompoundAction : Action
    {
        private List<Action> Actions;

        public CompoundAction(List<Action> actions)
        {
            Actions = actions;
        }

        public override void Execute()
        {
            foreach (var act in Actions)
            {
                act.Execute();
            }
        }

        public override void Undo()
        {
            foreach (var act in Actions)
            {
                act.Undo();
            }
        }
    }
}
