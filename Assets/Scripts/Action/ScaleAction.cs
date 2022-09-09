using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Action to scale an item
    /// </summary>
    public class ScaleAction : IAction
    {
        public ScaleAction(RuntimeItem item, Vector3 startScale, Vector3 endScale) : base(null, item)
        {
            _startScale = startScale;
            _endScale = endScale;
        }

        private Vector3 _startScale;
        private Vector3 _endScale;

        public override string Name => "ScaleAction";

        /// <summary>
        /// Apply final scale to ItemRelated
        /// </summary>
        public override void Apply()
        {
            ItemRelated.ApplyScale(_endScale);
            base.Apply();
        }

        /// <summary>
        /// Reapply the scale before the action on ItemRelated
        /// </summary>
        public override void Undo()
        {
            ItemRelated.ApplyScale(_startScale);
        }

        /// <summary>
        /// Create a ScaleAction
        /// </summary>
        /// <param name="item"></param>
        /// <param name="startScale"></param>
        /// <param name="endScale"></param>
        /// <returns></returns>
        public static ScaleAction Create(RuntimeItem item, Vector3 startScale, Vector3 endScale)
        {
            return new ScaleAction(item, startScale, endScale);
        }
    }
}