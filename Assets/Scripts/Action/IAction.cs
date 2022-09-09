using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Abstract class for any action
    /// </summary>
    public abstract class IAction
    {
        public IAction(AssetData data, RuntimeItem item = null)
        {
            AssetDataRelated = data;
            ItemRelated = item;
        }

        public abstract string Name { get; }
        public RuntimeItem ItemRelated { get; protected set; }
        public AssetData AssetDataRelated { get; protected set; }


        /// <summary>
        /// Applying the action and adding it to the stack of ActionRequestHandler
        /// </summary>
        public virtual void Apply()
        {
            ActionRequestHandler.AddAction(this);
        }

        /// <summary>
        /// Undo the action and removing it from the stack of ActionRequestHandler
        /// </summary>
        public abstract void Undo();

    }
}
