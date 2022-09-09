using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Action to translate an item
    /// </summary>
    public class TranslateAction : IAction
    {
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        public TranslateAction(RuntimeItem item, Vector3 startPos, Vector3 endPos) : base(null, item)
        {
            _startPosition = startPos;
            _endPosition = endPos;
        }

        public override string Name => "TranslationAction";

        public override void Apply()
        {
            ItemRelated.ApplyPosition(_endPosition);
            base.Apply();
        }

        public override void Undo()
        {
            ItemRelated.ApplyPosition(_startPosition);
        }

        /// <summary>
        /// Create a TranslateAction
        /// </summary>
        /// <param name="item"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
        public static TranslateAction Create(RuntimeItem item, Vector3 startPos, Vector3 endPos)
        {
            return new TranslateAction(item, startPos, endPos);
        }
    }
}