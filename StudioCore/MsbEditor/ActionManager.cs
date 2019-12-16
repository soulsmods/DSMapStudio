using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Manages undo and redo for an editor context
    /// </summary>
    public class ActionManager
    {
        private Stack<Action> UndoStack = new Stack<Action>();
        private Stack<Action> RedoStack = new Stack<Action>();

        public void ExecuteAction(Action a)
        {
            a.Execute();
            UndoStack.Push(a);
            RedoStack.Clear();
        }

        public void UndoAction()
        {
            if (UndoStack.Count() == 0)
            {
                return;
            }
            var a = UndoStack.Pop();
            a.Undo();
            RedoStack.Push(a);
        }

        public void RedoAction()
        {
            if (RedoStack.Count() == 0)
            {
                return;
            }
            var a = RedoStack.Pop();
            a.Execute();
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
