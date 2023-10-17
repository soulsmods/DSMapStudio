using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Editor
{
    [Flags]
    public enum ActionEvent
    {
        NoEvent = 0,

        // An object was added or removed from a scene
        ObjectAddedRemoved = 1,
    }

    /// <summary>
    /// Interface for objects that may react to events caused by actions that
    /// happen. Useful for invalidating caches that various editors may have.
    /// </summary>
    public interface IActionEventHandler
    {
        public void OnActionEvent(ActionEvent evt);
    }

    /// <summary>
    /// Manages undo and redo for an editor context
    /// </summary>
    public class ActionManager
    {
        private List<IActionEventHandler> _eventHandlers = new List<IActionEventHandler>();

        private Stack<EditorAction> UndoStack = new Stack<EditorAction>();
        private Stack<EditorAction> RedoStack = new Stack<EditorAction>();

        public void AddEventHandler(IActionEventHandler handler)
        {
            _eventHandlers.Add(handler);
        }

        private void NotifyHandlers(ActionEvent evt)
        {
            if (evt == ActionEvent.NoEvent)
            {
                return;
            }
            foreach (var handler in _eventHandlers)
            {
                handler.OnActionEvent(evt);
            }
        }

        public void ExecuteAction(EditorAction a)
        {
            NotifyHandlers(a.Execute());
            UndoStack.Push(a);
            RedoStack.Clear();
            UICache.ClearCaches();
        }

        public void PushSubManager(ActionManager child)
        {
            List<EditorAction> childList = child.UndoStack.ToList();
            childList.Reverse();
            UndoStack.Push(new CompoundAction(childList));
            RedoStack.Clear();
            UICache.ClearCaches();
        }

        public EditorAction PeekUndoAction()
        {
            if (UndoStack.Count() == 0)
            {
                return null;
            }
            return UndoStack.Peek();
        }

        public void UndoAction()
        {
            if (UndoStack.Count() == 0)
            {
                return;
            }
            var a = UndoStack.Pop();
            NotifyHandlers(a.Undo());
            RedoStack.Push(a);
            UICache.ClearCaches();
        }

        public void RedoAction()
        {
            if (RedoStack.Count() == 0)
            {
                return;
            }
            var a = RedoStack.Pop();
            NotifyHandlers(a.Execute());
            UndoStack.Push(a);
            UICache.ClearCaches();
        }

        public bool CanUndo()
        {
            return UndoStack.Count() > 0;
        }

        public bool CanRedo()
        {
            return RedoStack.Count() > 0;
        }

        public void Clear()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }
    }
}
