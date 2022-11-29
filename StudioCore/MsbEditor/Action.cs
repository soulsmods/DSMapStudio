using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SoulsFormats;

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
        abstract public ActionEvent Execute();
        abstract public ActionEvent Undo();
    }

    public class PropertiesChangedAction : Action
    {
        private class PropertyChange
        {
            public PropertyInfo Property;
            public object OldValue;
            public object NewValue;
            public int ArrayIndex;
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
            change.ArrayIndex = -1;
            Changes.Add(change);
        }

        public PropertiesChangedAction(PropertyInfo prop, int index, object changed, object newval)
        {
            ChangedObject = changed;
            var change = new PropertyChange();
            change.Property = prop;
            if (index != -1 && prop.PropertyType.IsArray)
            {
                Array a = (Array)change.Property.GetValue(ChangedObject);
                change.OldValue = a.GetValue(index);
            }
            else
            {
                change.OldValue = prop.GetValue(ChangedObject);
            }
            change.NewValue = newval;
            change.ArrayIndex = index;
            Changes.Add(change);
        }

        public void AddPropertyChange(PropertyInfo prop, object newval, int index = -1)
        {
            var change = new PropertyChange();
            change.Property = prop;
            if (index != -1 && prop.PropertyType.IsArray)
            {
                Array a = (Array)change.Property.GetValue(ChangedObject);
                change.OldValue = a.GetValue(index);
            }
            else
            {
                change.OldValue = prop.GetValue(ChangedObject);
            }
            change.NewValue = newval;
            change.ArrayIndex = index;
            Changes.Add(change);
        }

        public void SetPostExecutionAction(Action<bool> action)
        {
            PostExecutionAction = action;
        }

        public override ActionEvent Execute()
        {
            foreach (var change in Changes)
            {
                if (change.Property.PropertyType.IsArray && change.ArrayIndex != -1)
                {
                    Array a = (Array)change.Property.GetValue(ChangedObject);
                    a.SetValue(change.NewValue, change.ArrayIndex);
                }
                else
                {
                    change.Property.SetValue(ChangedObject, change.NewValue);
                }
            }
            if (PostExecutionAction != null)
            {
                PostExecutionAction.Invoke(false);
            }
            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            foreach (var change in Changes)
            {
                if (change.Property.PropertyType.IsArray && change.ArrayIndex != -1)
                {
                    Array a = (Array)change.Property.GetValue(ChangedObject);
                    a.SetValue(change.OldValue, change.ArrayIndex);
                }
                else
                {
                    change.Property.SetValue(ChangedObject, change.OldValue);
                }
            }
            if (PostExecutionAction != null)
            {
                PostExecutionAction.Invoke(true);
            }
            return ActionEvent.NoEvent;
        }
    }


    /// <summary>
    /// Copies values from one array to another without affecting references.
    /// </summary>
    public class ArrayPropertyCopyAction : Action
    {
        private class PropertyChange
        {
            public Array ChangedObj;
            public object OldVal;
            public object NewVal;
            public int ArrayIndex;
        }

        private List<PropertyChange> Changes = new List<PropertyChange>();
        private Action<bool> PostExecutionAction = null;

        public ArrayPropertyCopyAction(Array source, Array target)
        {
            for (var i = 0; i < target.Length; i++)
            {
                PropertyChange change = new()
                {
                    ChangedObj = target,
                    OldVal = target.GetValue(i),
                    NewVal = source.GetValue(i),
                    ArrayIndex = i
                };
                Changes.Add(change);
            }
        }
        public ArrayPropertyCopyAction(Array source, List<Array> targetList)
        {
            foreach (var target in targetList)
            {
                for (var i = 0; i < target.Length; i++)
                {
                    PropertyChange change = new()
                    {
                        ChangedObj = target,
                        OldVal = target.GetValue(i),
                        NewVal = source.GetValue(i),
                        ArrayIndex = i
                    };
                    Changes.Add(change);
                }
            }
        }

        public void SetPostExecutionAction(Action<bool> action)
        {
            PostExecutionAction = action;
        }

        public override ActionEvent Execute()
        {
            foreach (var change in Changes)
            {
                change.ChangedObj.SetValue(change.NewVal, change.ArrayIndex);
            }
            if (PostExecutionAction != null)
            {
                PostExecutionAction.Invoke(false);
            }
            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            foreach (var change in Changes)
            {
                change.ChangedObj.SetValue(change.OldVal, change.ArrayIndex);
            }
            if (PostExecutionAction != null)
            {
                PostExecutionAction.Invoke(true);
            }
            return ActionEvent.NoEvent;
        }
    }

    public class MultipleEntityPropertyChangeAction : Action
    {
        private class PropertyChange
        {
            public object ChangedObj;
            public PropertyInfo Property;
            public object OldValue;
            public object NewValue;
            public int ArrayIndex;
        }

        public bool UpdateRenderModel = false;
        private List<PropertyChange> Changes = new();
        private HashSet<Entity> ChangedEnts = new();

        public MultipleEntityPropertyChangeAction(PropertyInfo prop, HashSet<Entity> changedEnts, object newval, int index = -1)
        {
            ChangedEnts = changedEnts;
            foreach (var o in changedEnts)
            {
                var propObj = Utils.FindPropertyObject(prop, o.WrappedObject);
                var change = new PropertyChange
                {
                    ChangedObj = propObj,
                    Property = prop,
                    NewValue = newval,
                    ArrayIndex = index
                };
                if (index != -1 && prop.PropertyType.IsArray)
                {
                    Array a = (Array)change.Property.GetValue(propObj);
                    change.OldValue = a.GetValue(index);
                }
                else
                {
                    change.OldValue = prop.GetValue(propObj);
                }

                Changes.Add(change);
            }
        }

        public override ActionEvent Execute()
        {
            foreach (var change in Changes)
            {
                if (change.Property.PropertyType.IsArray && change.ArrayIndex != -1)
                {
                    Array a = (Array)change.Property.GetValue(change.ChangedObj);
                    a.SetValue(change.NewValue, change.ArrayIndex);
                }
                else
                {
                    change.Property.SetValue(change.ChangedObj, change.NewValue);
                }
            }
            foreach (var e in ChangedEnts)
            {
                if (UpdateRenderModel)
                    e.UpdateRenderModel();
                // Clear name cache, forcing it to update.
                e.Name = null;
            }

            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            foreach (var change in Changes)
            {
                if (change.Property.PropertyType.IsArray && change.ArrayIndex != -1)
                {
                    Array a = (Array)change.Property.GetValue(change.ChangedObj);
                    a.SetValue(change.OldValue, change.ArrayIndex);
                }
                else
                {
                    change.Property.SetValue(change.ChangedObj, change.OldValue);
                }
            }
            foreach (var e in ChangedEnts)
            {
                if (UpdateRenderModel)
                    e.UpdateRenderModel();
                // Clear name cache, forcing it to update.
                e.Name = null;
            }

            return ActionEvent.NoEvent;
        }
    }

    public class CloneMapObjectsAction : Action
    {
        private Universe Universe;
        private Scene.RenderScene Scene;
        private List<MapEntity> Clonables = new List<MapEntity>();
        private List<MapEntity> Clones = new List<MapEntity>();
        private List<ObjectContainer> CloneMaps = new List<ObjectContainer>();
        private bool SetSelection;
        private Map MapTarget;

        private static Regex TrailIDRegex = new Regex(@"_(?<id>\d+)$");

        public CloneMapObjectsAction(Universe univ, Scene.RenderScene scene, List<MapEntity> objects, bool setSelection, Map mapTarget = null)
        {
            Universe = univ;
            Scene = scene;
            Clonables.AddRange(objects);
            SetSelection = setSelection;
            MapTarget = mapTarget;
        }

        public override ActionEvent Execute()
        {
            bool clonesCached = Clones.Count() > 0;
            // foreach (var obj in Clonables)

            var objectnames = new Dictionary<string, HashSet<string>>();
            for (int i = 0; i < Clonables.Count(); i++)
            {
                Map? m;
                if (MapTarget != null)
                {
                    m = Universe.GetLoadedMap(MapTarget.Name);
                }
                else
                {
                    m = Universe.GetLoadedMap(Clonables[i].MapID);
                }
                if (m != null)
                {
                    // Get list of names that exist so our duplicate names don't trample over them
                    if (!objectnames.ContainsKey(Clonables[i].MapID))
                    {
                        var nameset = new HashSet<string>();
                        foreach (var n in m.Objects)
                        {
                            nameset.Add(n.Name);
                        }
                        objectnames.Add(Clonables[i].MapID, nameset);
                    }

                    // If this was executed in the past we reused the cloned objects so because redo
                    // actions that follow this may reference the previously cloned object
                    MapEntity newobj = clonesCached ? Clones[i] : (MapEntity)Clonables[i].Clone();

                    // Use pattern matching to attempt renames based on appended ID
                    Match idmatch = TrailIDRegex.Match(Clonables[i].Name);
                    if (idmatch.Success)
                    {
                        var idstring = idmatch.Result("${id}");
                        int id = int.Parse(idstring);
                        string newid = idstring;
                        while (objectnames[Clonables[i].MapID].Contains(Clonables[i].Name.Substring(0, Clonables[i].Name.Length - idstring.Length) + newid))
                        {
                            id++;
                            newid = id.ToString("D" + idstring.Length.ToString());
                        }
                        newobj.Name = Clonables[i].Name.Substring(0, Clonables[i].Name.Length - idstring.Length) + newid;
                        objectnames[Clonables[i].MapID].Add(newobj.Name);
                    }
                    else
                    {
                        var idstring = "0001";
                        int id = int.Parse(idstring);
                        string newid = idstring;
                        while (objectnames[Clonables[i].MapID].Contains(Clonables[i].Name + "_" + newid))
                        {
                            id++;
                            newid = id.ToString("D" + idstring.Length.ToString());
                        }
                        newobj.Name = Clonables[i].Name + "_" + newid;
                        objectnames[Clonables[i].MapID].Add(newobj.Name);
                    }

                    m.Objects.Insert(m.Objects.IndexOf(Clonables[i]) + 1, newobj);
                    if (Clonables[i].Parent != null)
                    {
                        int idx = Clonables[i].Parent.ChildIndex(Clonables[i]);
                        Clonables[i].Parent.AddChild(newobj, idx + 1);
                    }
                    newobj.UpdateRenderModel();
                    if (newobj.RenderSceneMesh != null)
                    {
                        newobj.RenderSceneMesh.SetSelectable(newobj);
                    }
                    if (!clonesCached)
                    {
                        Clones.Add(newobj);
                        CloneMaps.Add(m);
                        m.HasUnsavedChanges = true;
                    }
                    else
                    {
                        if (Clones[i].RenderSceneMesh != null)
                        {
                            Clones[i].RenderSceneMesh.AutoRegister = true;
                            Clones[i].RenderSceneMesh.Register();
                        }
                    }
                }
            }
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var c in Clones)
                {
                    Universe.Selection.AddSelection(c);
                }
            }
            return ActionEvent.ObjectAddedRemoved;
        }

        public override ActionEvent Undo()
        {
            for (int i = 0; i < Clones.Count(); i++)
            {
                CloneMaps[i].Objects.Remove(Clones[i]);
                if (Clones[i] != null)
                {
                    Clones[i].Parent.RemoveChild(Clones[i]);
                }
                if (Clones[i].RenderSceneMesh != null)
                {
                    Clones[i].RenderSceneMesh.AutoRegister = false;
                    Clones[i].RenderSceneMesh.UnregisterWithScene();
                }
            }
            // Clones.Clear();
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var c in Clonables)
                {
                    Universe.Selection.AddSelection(c);
                }
            }
            return ActionEvent.ObjectAddedRemoved;
        }
    }

    public class AddMapObjectsAction : Action
    {
        private Universe Universe;
        private Map Map;
        private Scene.RenderScene Scene;
        private List<MapEntity> Added = new List<MapEntity>();
        private List<ObjectContainer> AddedMaps = new List<ObjectContainer>();
        private bool SetSelection;
        private Entity Parent;

        private static Regex TrailIDRegex = new Regex(@"_(?<id>\d+)$");

        public AddMapObjectsAction(Universe univ, Map map, Scene.RenderScene scene, List<MapEntity> objects, bool setSelection, Entity parent)
        {
            Universe = univ;
            Map = map;
            Scene = scene;
            Added.AddRange(objects);
            SetSelection = setSelection;
            Parent = parent;
        }

        public override ActionEvent Execute()
        {
            for (int i = 0; i < Added.Count(); i++)
            {
                if (Map != null)
                {
                    Map.Objects.Add(Added[i]);
                    Parent.AddChild(Added[i]);
                    Added[i].UpdateRenderModel();
                    if (Added[i].RenderSceneMesh != null)
                    {
                        Added[i].RenderSceneMesh.SetSelectable(Added[i]);
                    }
                    if (Added[i].RenderSceneMesh != null)
                    {
                        Added[i].RenderSceneMesh.AutoRegister = true;
                        Added[i].RenderSceneMesh.Register();
                    }
                    AddedMaps.Add(Map);
                }
                else
                {
                    AddedMaps.Add(null);
                }
            }
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var c in Added)
                {
                    Universe.Selection.AddSelection(c);
                }
            }
            return ActionEvent.ObjectAddedRemoved;
        }

        public override ActionEvent Undo()
        {
            for (int i = 0; i < Added.Count(); i++)
            {
                AddedMaps[i].Objects.Remove(Added[i]);
                if (Added[i] != null)
                {
                    Added[i].Parent.RemoveChild(Added[i]);
                }
                if (Added[i].RenderSceneMesh != null)
                {
                    Added[i].RenderSceneMesh.AutoRegister = false;
                    Added[i].RenderSceneMesh.UnregisterWithScene();
                }
            }
            //Clones.Clear();
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
            }
            return ActionEvent.ObjectAddedRemoved;
        }
    }

    /// <summary>
    /// Deprecated
    /// </summary>
    [Obsolete]
    public class AddParamsAction : Action
    {
        private PARAM Param;
        private string ParamString;
        private List<PARAM.Row> Clonables = new List<PARAM.Row>();
        private List<PARAM.Row> Clones = new List<PARAM.Row>();
        private bool SetSelection = false;

        public AddParamsAction(PARAM param, string pstring, List<PARAM.Row> rows, bool setsel)
        {
            Param = param;
            Clonables.AddRange(rows);
            ParamString = pstring;
            SetSelection = setsel;
        }

        public override ActionEvent Execute()
        {
            foreach (var row in Clonables)
            {
                var newrow = new PARAM.Row(row);
                if (Param[(int) row.ID] == null)
                {
                    newrow.Name = row.Name != null ? row.Name : "";
                    int index = 0;
                    foreach (PARAM.Row r in Param.Rows)
                    {
                        if (r.ID > newrow.ID)
                            break;
                        index++;
                    }
                    Param.Rows.Insert(index, newrow);
                }
                else
                {
                    newrow.Name = row.Name != null ? row.Name + "_1" : "";
                    Param.Rows.Insert(Param.Rows.IndexOf(Param[(int) row.ID]) + 1, newrow);
                }
                Clones.Add(newrow);
            }
            if (SetSelection)
            {
                // EditorCommandQueue.AddCommand($@"param/select/{ParamString}/{Clones[0].ID}");
            }
            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            for (int i = 0; i < Clones.Count(); i++)
            {
                Param.Rows.Remove(Clones[i]);
            }
            Clones.Clear();
            if (SetSelection)
            {
            }
            return ActionEvent.NoEvent;
        }
    }

    public class DeleteMapObjectsAction : Action
    {
        private Universe Universe;
        private Scene.RenderScene Scene;
        private List<MapEntity> Deletables = new List<MapEntity>();
        private List<int> RemoveIndices = new List<int>();
        private List<ObjectContainer> RemoveMaps = new List<ObjectContainer>();
        private List<MapEntity> RemoveParent = new List<MapEntity>();
        private List<int> RemoveParentIndex = new List<int>();
        private bool SetSelection;

        public DeleteMapObjectsAction(Universe univ, Scene.RenderScene scene, List<MapEntity> objects, bool setSelection)
        {
            Universe = univ;
            Scene = scene;
            Deletables.AddRange(objects);
            SetSelection = setSelection;
        }

        public override ActionEvent Execute()
        {
            foreach (var obj in Deletables)
            {
                var m = Universe.GetLoadedMap(obj.MapID);
                if (m != null)
                {
                    RemoveMaps.Add(m);
                    m.HasUnsavedChanges = true;
                    RemoveIndices.Add(m.Objects.IndexOf(obj));
                    m.Objects.RemoveAt(RemoveIndices.Last());
                    if (obj.RenderSceneMesh != null)
                    {
                        obj.RenderSceneMesh.AutoRegister = false;
                        obj.RenderSceneMesh.UnregisterWithScene();
                    }
                    RemoveParent.Add((MapEntity)obj.Parent);
                    if (obj.Parent != null)
                    {
                        RemoveParentIndex.Add(obj.Parent.RemoveChild(obj));
                    }
                    else
                    {
                        RemoveParentIndex.Add(-1);
                    }
                }
                else
                {
                    RemoveMaps.Add(null);
                    RemoveIndices.Add(-1);
                    RemoveParent.Add(null);
                    RemoveParentIndex.Add(-1);
                }
            }
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
            }
            return ActionEvent.ObjectAddedRemoved;
        }

        public override ActionEvent Undo()
        {
            for (int i = 0; i < Deletables.Count(); i++)
            {
                if (RemoveMaps[i] == null || RemoveIndices[i] == -1)
                {
                    continue;
                }
                RemoveMaps[i].Objects.Insert(RemoveIndices[i], Deletables[i]);
                if (Deletables[i].RenderSceneMesh != null)
                {
                    Deletables[i].RenderSceneMesh.AutoRegister = true;
                    Deletables[i].RenderSceneMesh.Register();
                }
                if (RemoveParent[i] != null)
                {
                    RemoveParent[i].AddChild(Deletables[i], RemoveParentIndex[i]);
                }
            }
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var d in Deletables)
                {
                    Universe.Selection.AddSelection(d);
                }
            }
            return ActionEvent.ObjectAddedRemoved;
        }
    }

    /// <summary>
    /// Deprecated
    /// </summary>
    [Obsolete]
    public class DeleteParamsAction : Action
    {
        private PARAM Param;
        private List<PARAM.Row> Deletables = new List<PARAM.Row>();
        private List<int> RemoveIndices = new List<int>();
        private bool SetSelection = false;

        public DeleteParamsAction(PARAM param, List<PARAM.Row> rows)
        {
            Param = param;
            Deletables.AddRange(rows);
        }

        public override ActionEvent Execute()
        {
            foreach (var row in Deletables)
            {
                RemoveIndices.Add(Param.Rows.IndexOf(row));
                Param.Rows.RemoveAt(RemoveIndices.Last());
            }
            if (SetSelection)
            {
            }
            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            for (int i = 0; i < Deletables.Count(); i++)
            {
                Param.Rows.Insert(RemoveIndices[i], Deletables[i]);
            }
            if (SetSelection)
            {
            }
            return ActionEvent.NoEvent;
        }
    }

    public class ReorderContainerObjectsAction : Action
    {
        private Universe Universe;
        private List<Entity> SourceObjects = new List<Entity>();
        private List<int> TargetIndices = new List<int>();
        private List<ObjectContainer> Containers = new List<ObjectContainer>();
        private int[] UndoIndices;
        private bool SetSelection;

        public ReorderContainerObjectsAction(Universe univ, List<Entity> src, List<int> targets, bool setSelection)
        {
            Universe = univ;
            SourceObjects.AddRange(src);
            TargetIndices.AddRange(targets);
            SetSelection = setSelection;
        }

        public override ActionEvent Execute()
        {
            int[] sourceindices = new int[SourceObjects.Count];
            for (int i = 0; i < SourceObjects.Count; i++)
            {
                var m = SourceObjects[i].Container;
                Containers.Add(m);
                m.HasUnsavedChanges = true;
                sourceindices[i] = m.Objects.IndexOf(SourceObjects[i]);
            }
            for (int i = 0; i < sourceindices.Length; i++)
            {
                // Remove object and update indices
                int src = sourceindices[i];
                Containers[i].Objects.RemoveAt(src);
                for (int j = 0; j < sourceindices.Length; j++)
                {
                    if (sourceindices[j] > src)
                    {
                        sourceindices[j]--;
                    }
                }
                for (int j = 0; j < TargetIndices.Count; j++)
                {
                    if (TargetIndices[j] > src)
                    {
                        TargetIndices[j]--;
                    }
                }

                // Add new object
                int dest = TargetIndices[i];
                Containers[i].Objects.Insert(dest, SourceObjects[i]);
                for (int j = 0; j < sourceindices.Length; j++)
                {
                    if (sourceindices[j] > dest)
                    {
                        sourceindices[j]++;
                    }
                }
                for (int j = 0; j < TargetIndices.Count; j++)
                {
                    if (TargetIndices[j] > dest)
                    {
                        TargetIndices[j]++;
                    }
                }
            }
            UndoIndices = sourceindices;
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var c in SourceObjects)
                {
                    Universe.Selection.AddSelection(c);
                }
            }
            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            for (int i = 0; i < TargetIndices.Count; i++)
            {
                // Remove object and update indices
                int src = TargetIndices[i];
                Containers[i].Objects.RemoveAt(src);
                for (int j = 0; j < UndoIndices.Length; j++)
                {
                    if (UndoIndices[j] > src)
                    {
                        UndoIndices[j]--;
                    }
                }
                for (int j = 0; j < TargetIndices.Count; j++)
                {
                    if (TargetIndices[j] > src)
                    {
                        TargetIndices[j]--;
                    }
                }

                // Add new object
                int dest = UndoIndices[i];
                Containers[i].Objects.Insert(dest, SourceObjects[i]);
                for (int j = 0; j < UndoIndices.Length; j++)
                {
                    if (UndoIndices[j] > dest)
                    {
                        UndoIndices[j]++;
                    }
                }
                for (int j = 0; j < TargetIndices.Count; j++)
                {
                    if (TargetIndices[j] > dest)
                    {
                        TargetIndices[j]++;
                    }
                }
            }
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var c in SourceObjects)
                {
                    Universe.Selection.AddSelection(c);
                }
            }
            return ActionEvent.NoEvent;
        }
    }

    public class ChangeEntityHierarchyAction : Action
    {
        private Universe Universe;
        private List<Entity> SourceObjects = new List<Entity>();
        private List<Entity> TargetObjects = new List<Entity>();
        private List<int> TargetIndices = new List<int>();
        private Entity[] UndoObjects;
        private int[] UndoIndices;
        private bool SetSelection;

        public ChangeEntityHierarchyAction(Universe univ, List<Entity> src, List<Entity> targetEnts, List<int> targets, bool setSelection)
        {
            Universe = univ;
            SourceObjects.AddRange(src);
            TargetObjects.AddRange(targetEnts);
            TargetIndices.AddRange(targets);
            SetSelection = setSelection;
        }

        public override ActionEvent Execute()
        {
            int[] sourceindices = new int[SourceObjects.Count];
            for (int i = 0; i < SourceObjects.Count; i++)
            {
                sourceindices[i] = -1;
                if (SourceObjects[i].Parent != null)
                {
                    sourceindices[i] = SourceObjects[i].Parent.ChildIndex(SourceObjects[i]);
                }
            }
            for (int i = 0; i < sourceindices.Length; i++)
            {
                // Remove object and update indices
                int src = sourceindices[i];
                if (src != -1)
                {
                    SourceObjects[i].Parent.RemoveChild(SourceObjects[i]);
                    for (int j = 0; j < sourceindices.Length; j++)
                    {
                        if (SourceObjects[i].Parent == SourceObjects[j].Parent && sourceindices[j] > src)
                        {
                            sourceindices[j]--;
                        }
                    }
                    for (int j = 0; j < TargetIndices.Count; j++)
                    {
                        if (SourceObjects[i].Parent == TargetObjects[j] && TargetIndices[j] > src)
                        {
                            TargetIndices[j]--;
                        }
                    }
                }

                // Add new object
                int dest = TargetIndices[i];
                TargetObjects[i].AddChild(SourceObjects[i], dest);
                for (int j = 0; j < sourceindices.Length; j++)
                {
                    if (TargetObjects[i] == SourceObjects[j].Parent && sourceindices[j] > dest)
                    {
                        sourceindices[j]++;
                    }
                }
                for (int j = 0; j < TargetIndices.Count; j++)
                {
                    if (TargetObjects[i] == TargetObjects[j] && TargetIndices[j] > dest)
                    {
                        TargetIndices[j]++;
                    }
                }
            }
            UndoIndices = sourceindices;
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var c in SourceObjects)
                {
                    Universe.Selection.AddSelection(c);
                }
            }
            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            /*for (int i = 0; i < TargetIndices.Count; i++)
            {
                // Remove object and update indices
                int src = TargetIndices[i];
                Containers[i].Objects.RemoveAt(src);
                for (int j = 0; j < UndoIndices.Length; j++)
                {
                    if (UndoIndices[j] > src)
                    {
                        UndoIndices[j]--;
                    }
                }
                for (int j = 0; j < TargetIndices.Count; j++)
                {
                    if (TargetIndices[j] > src)
                    {
                        TargetIndices[j]--;
                    }
                }

                // Add new object
                int dest = UndoIndices[i];
                Containers[i].Objects.Insert(dest, SourceObjects[i]);
                for (int j = 0; j < UndoIndices.Length; j++)
                {
                    if (UndoIndices[j] > dest)
                    {
                        UndoIndices[j]++;
                    }
                }
                for (int j = 0; j < TargetIndices.Count; j++)
                {
                    if (TargetIndices[j] > dest)
                    {
                        TargetIndices[j]++;
                    }
                }
            }
            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var c in SourceObjects)
                {
                    Universe.Selection.AddSelection(c);
                }
            }*/
            return ActionEvent.NoEvent;
        }
    }


    public class ChangeMapObjectType : Action
    {
        private Universe Universe;
        private Queue<object> UndoQueue = new();
        private List<MapEntity> ModifiedObjects = new();
        private List<MapEntity> Entities = new();
        private Type MsbType;
        private string[] SourceTypes;
        private string[] TargetTypes;
        private bool SetSelection;
        private string MsbParamstr;

        /// <summary>
        /// Change selected map objects from one type to another. Only works for map objects of the same overarching type, such as Parts or Regions.
        /// Data for properties absent in targeted type will be lost, but will be restored for undo/redo.
        /// </summary>
        public ChangeMapObjectType(Universe univ, Type msbclass, List<MapEntity> selectedEnts, string[] sourceTypes, string[] targetTypes, string msbParamStr ,bool setSelection)
        {

            Universe = univ;
            MsbType = msbclass;
            Entities.AddRange(selectedEnts);
            SourceTypes = sourceTypes;
            TargetTypes = targetTypes;
            SetSelection = setSelection;
            MsbParamstr = msbParamStr;
        }

        public override ActionEvent Execute()
        {
            for (var iTypes = 0; iTypes < SourceTypes.Length; iTypes++)
            {
                var sourceType = MsbType.GetNestedType(MsbParamstr).GetNestedType(SourceTypes[iTypes]); //get desired msbparam type for the current MSB
                var targetType = MsbType.GetNestedType(MsbParamstr).GetNestedType(TargetTypes[iTypes]); //get desired msbparam type for the current MSB
                var partType = MsbType.GetNestedType(MsbParamstr);

                for (var i = 0; i < Entities.Count; i++)
                {
                    var ent = Entities[i];

                    var currentType = ent.WrappedObject.GetType();
                    if (currentType == sourceType)
                    {
                        var m = Universe.GetLoadedMap(ent.MapID);
                        m.HasUnsavedChanges = true;
                        UndoQueue.Enqueue(ent.DeepCopyObject(ent.WrappedObject)); //store backup of wrappedObj in queue for undoing

                        var source = ent.WrappedObject;
                        var target = targetType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());

                        // Go through properties of source type and set them to target type (if they exist under the same name)
                        foreach (PropertyInfo property in sourceType.GetProperties().Where(p => p.CanWrite)) //public set properties
                        {
                            var targetProp = target.GetType().GetProperty(property.Name);
                            if (targetProp != null) //make sure target type has this property (happens for Assets vs DummyAssets)
                            {
                                targetProp.SetValue(target, property.GetValue(source, null), null); // Copy every (writable) value to/from dummy/nondummy. this may be too risky in the future!
                            }
                        }
                        foreach (PropertyInfo property in sourceType.GetProperties().Where(p => !p.CanWrite)) //private set properties
                        {
                            var targetProp = target.GetType().GetProperty(property.Name);
                            if (targetProp != null) //make sure target type has this property (happens for Assets vs DummyAssets)
                            {
                                var prop = targetProp.DeclaringType.GetProperty(property.Name);
                                prop.SetValue(target, property.GetValue(source, null), BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                                // Pretty good chance this will explode in some circumstances!
                            }
                        }

                        //assign new dummied/undummied wrappedObj to entity
                        ent.WrappedObject = target;
                        ModifiedObjects.Add(ent);
                    }
                }
            }

            //if (SetSelection) {}
            return ActionEvent.ObjectAddedRemoved;
        }

        public override ActionEvent Undo()
        {
            for (var iTypes = 0; iTypes < SourceTypes.Length; iTypes++)
            {
                Type sourceType = MsbType.GetNestedType(MsbParamstr).GetNestedType(TargetTypes[iTypes]); //inverted for undo
                //Type targetType = MsbType.GetNestedType(MsbParamstr).GetNestedType(SourceTypes[iTypes]); //inverted for undo

                for (var i = 0; i < ModifiedObjects.Count; i++)
                {
                    var ent = ModifiedObjects[i];

                    if (ent.Type == MapEntity.MapEntityType.Part)
                    {
                        var currentType = ent.WrappedObject.GetType();
                        if (currentType == sourceType)
                        {
                            var m = Universe.GetLoadedMap(ent.MapID);
                            m.HasUnsavedChanges = true;

                            ent.WrappedObject = UndoQueue.Dequeue(); //retrieve backup object from queue
                        }
                    }
                }
            }

            if (SetSelection)
            {
                Universe.Selection.ClearSelection();
                foreach (var d in Entities)
                {
                    Universe.Selection.AddSelection(d);
                }
            }
            return ActionEvent.ObjectAddedRemoved;
        }
    }

    public class CompoundAction : Action
    {
        private List<Action> Actions;

        private Action<bool> PostExecutionAction = null;

        public CompoundAction(List<Action> actions)
        {
            Actions = actions;
        }

        public void SetPostExecutionAction(Action<bool> action)
        {
            PostExecutionAction = action;
        }

        public override ActionEvent Execute()
        {
            var evt = ActionEvent.NoEvent;
            foreach (var act in Actions)
            {
                if (act != null)
                {
                    evt |= act.Execute();
                }
            }
            if (PostExecutionAction != null)
            {
                PostExecutionAction.Invoke(false);
            }
            return evt;
        }

        public override ActionEvent Undo()
        {
            var evt = ActionEvent.NoEvent;
            foreach (var act in Actions)
            {
                if (act != null)
                {
                    evt |= act.Undo();
                }
            }
            if (PostExecutionAction != null)
            {
                PostExecutionAction.Invoke(true);
            }
            return evt;
        }
    }
}
