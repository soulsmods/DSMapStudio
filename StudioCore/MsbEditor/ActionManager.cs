using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.MsbEditor
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

        private Stack<Action> UndoStack = new Stack<Action>();
        private Stack<Action> RedoStack = new Stack<Action>();

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

        public void ExecuteAction(Action a)
        {
            NotifyHandlers(a.Execute());
            UndoStack.Push(a);
            RedoStack.Clear();
        }

        public Action PeekUndoAction()
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
