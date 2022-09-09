using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Handler stacking all actions performed by users
    /// Possibility to clear, add and undo action
    /// </summary>
    public class ActionRequestHandler : SingletonMonobehaviour<ActionRequestHandler>
    {
        private readonly Stack<IAction> _actions = new Stack<IAction>();

        /// <summary>
        /// Will add the action to the stack
        /// </summary>
        /// <param name="a"></param>
        public static void AddAction(IAction a)
        {
            Instance._actions.Push(a);
        }

        /// <summary>
        /// Will remove the last action from the stack
        /// Trigger the Undo method from this action
        /// </summary>
        public void UndoLastAction()
        {
            if (_actions.Count == 0)
                return;

            var action = _actions.Pop();
            action.Undo();
        }

        /// <summary>
        /// Clear all actions without executing them
        /// </summary>
        public void ClearAllActions()
        {
            _actions.Clear();
        }
    }
}
