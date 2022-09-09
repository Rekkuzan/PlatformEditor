using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlatformEditor
{
    /// <summary>
    /// Action to soft delete an item on the environment (Disabling it)
    /// </summary>
    public class DeleteAction : IAction
    {

        public DeleteAction(AssetData data, RuntimeItem item) : base(data, item)
        {
        }

        public override string Name => "DeleteAction";

        public override void Apply()
        {
            if (ItemRelated == null)
                return;

            // this is a soft delete
            ItemRelated.gameObject.SetActive(false);

            SceneManager.Instance.UnregisterItem(ItemRelated.gameObject);
            base.Apply();
        }

        public override void Undo()
        {
            ItemRelated.gameObject.SetActive(true);
            SceneManager.Instance.RegisterItem(ItemRelated.gameObject);
        }

        /// <summary>
        /// Create a delete action
        /// </summary>
        /// <returns></returns>
        public static DeleteAction Create(AssetData data, RuntimeItem item)
        {
            return new DeleteAction(data, item);
        }
    }
}