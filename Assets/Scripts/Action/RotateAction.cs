using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Action to rotate the object
    /// </summary>
    public class RotateAction : IAction
    {
        private Quaternion _startRotation;
        private Quaternion _endRotation;

        public RotateAction(RuntimeItem item, Quaternion startRot, Quaternion endRot) : base(null, item)
        {
            _startRotation = startRot;
            _endRotation = endRot;
        }

        public override string Name => "RotateAction";

        public override void Apply()
        {
            ItemRelated.ApplyRotation(_endRotation);
            base.Apply();
        }

        public override void Undo()
        {
            ItemRelated.ApplyRotation(_startRotation);
        }

        /// <summary>
        /// Create the rotationAction
        /// </summary>
        /// <param name="item"></param>
        /// <param name="startRot"></param>
        /// <param name="endRot"></param>
        /// <returns></returns>
        public static RotateAction Create(RuntimeItem item, Quaternion startRot, Quaternion endRot)
        {
            return new RotateAction(item, startRot, endRot);
        }
    }
}