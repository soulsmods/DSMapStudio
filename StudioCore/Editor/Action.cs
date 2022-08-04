using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SoulsFormats;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using FSParam;

namespace StudioCore.Editor
{
    /// <summary>
    /// An action that can be performed by the user in the editor that represents
    /// a single atomic editor action that affects the state of the map. Each action
    /// should have enough information to apply the action AND undo the action, as
    /// these actions get pushed to a stack for undo/redo
    /// </summary>
    public abstract class EditorAction
    {
        abstract public ActionEvent Execute();
        abstract public ActionEvent Undo();
    }

    public class PropertiesChangedAction : EditorAction
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

    public class AddParamsAction : EditorAction
    {
        private Param Param;
        private string ParamString;
        private List<Param.Row> Clonables = new List<Param.Row>();
        private List<Param.Row> Clones = new List<Param.Row>();
        private List<int> RemovedIndex = new List<int>();
        private List<Param.Row> Removed = new List<Param.Row>();
        private bool appOnly = false;
        private bool replParams = false;
        private bool useIDAsIndex = false;

        public AddParamsAction(Param param, string pstring, List<Param.Row> rows, bool appendOnly, bool replaceParams, bool useIDasIndex)
        {
            Param = param;
            Clonables.AddRange(rows);
            ParamString = pstring;
            appOnly = appendOnly;
            replParams = replaceParams;
            useIDAsIndex = useIDasIndex;
        }

        public override ActionEvent Execute()
        {
            foreach (var row in Clonables)
            {
                var newrow = new Param.Row(row);
                if (useIDAsIndex)
                {
                    Param.Rows.Insert(newrow.ID, newrow);
                }
                else
                {
                    if (Param[(int) row.ID] != null)
                    {
                        if (replParams)
                        {
                            Param.Row existing = Param[(int) row.ID];
                            RemovedIndex.Add(Param.Rows.IndexOf(existing));
                            Removed.Add(existing);
                            Param.Rows.Remove(existing);
                        }
                        else
                        {
                            newrow.Name = row.Name != null ? row.Name + "_1" : "";
                            int newID = row.ID + 1;
                            while (Param[newID] != null)
                            {
                                newID++;
                            }
                            newrow.ID = newID;
                            Param.Rows.Insert(Param.Rows.IndexOf(Param[(int) newID - 1]) + 1, newrow);
                        }
                    }
                    if (Param[(int) row.ID] == null)
                    {
                        newrow.Name = row.Name != null ? row.Name : "";
                        if (appOnly)
                        {
                            Param.Rows.Add(newrow);
                        }
                        else
                        {
                            int index = 0;
                            foreach (Param.Row r in Param.Rows)
                            {
                                if (r.ID > newrow.ID)
                                    break;
                                index++;
                            }
                            Param.Rows.Insert(index, newrow);
                        }
                    }
                }
                Clones.Add(newrow);
            }
            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            for (int i = 0; i < Clones.Count(); i++)
            {
                Param.Rows.Remove(Clones[i]);
            }
            for (int i = Removed.Count()-1; i >= 0; i--)
            {
                Param.Rows.Insert(RemovedIndex[i], Removed[i]);
            }
            
            Clones.Clear();
            RemovedIndex.Clear();
            Removed.Clear();
            return ActionEvent.NoEvent;
        }
    }

    
    public class DuplicateFMGEntryAction : EditorAction
    {
        //Not implemented ATM: needs to update FMG active entry, and force FMG to refresh the cache (+ on undo/redo)
        private FMG Fmg = new();
        private FMG.Entry Entry;
        private KeyValuePair<FMG, FMG.Entry> Clone;

        public DuplicateFMGEntryAction(FMG fmg, FMG.Entry entry)
        {
            Fmg = fmg;
            Entry = entry;
        }

        public override ActionEvent Execute()
        {

            FMG.Entry newentry = new(Entry.ID, Entry.Text);

            do
            {
                newentry.ID++; //get an unused ID
            }
            while (Fmg.Entries.Find(e => e.ID == newentry.ID) != null);

            Fmg.Entries.Insert(Fmg.Entries.IndexOf(Entry) + 1, newentry);

            Clone = new(Fmg, newentry);

            return ActionEvent.NoEvent;
        }

        public override ActionEvent Undo()
        {
            Fmg = Clone.Key;
            Entry = Clone.Value;

            Fmg.Entries.Remove(Entry);
            Clone = new();

            return ActionEvent.NoEvent;
        }
    }
    

    public class DeleteParamsAction : EditorAction
    {
        private Param Param;
        private List<Param.Row> Deletables = new List<Param.Row>();
        private List<int> RemoveIndices = new List<int>();
        private bool SetSelection = false;

        public DeleteParamsAction(Param param, List<Param.Row> rows)
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
            for (int i = Deletables.Count() -1 ; i >= 0 ; i--)
            {
                Param.Rows.Insert(RemoveIndices[i], Deletables[i]);
            }
            if (SetSelection)
            {
                
            }
            return ActionEvent.NoEvent;
        }
    }

    public class CompoundAction : EditorAction
    {
        private List<EditorAction> Actions;

        private Action<bool> PostExecutionAction = null;

        public CompoundAction(List<EditorAction> actions)
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
            for (int i = 0; i < Actions.Count; i++)
            {
                var act = Actions[i];
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
            for (int i = Actions.Count - 1; i >= 0; i--)
            {
                var act = Actions[i];
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
