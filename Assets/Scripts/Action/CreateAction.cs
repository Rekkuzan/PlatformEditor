using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{ 
    /// <summary>
    /// Action to create an item on the environment
    /// </summary>
    public class CreateAction : IAction
    {
        public CreateAction(AssetData data, Vector3 position) : base(data, null)
        {
            PositionSpawn = position;
        }

        public override string Name => "CreateAction";

        public Vector3 PositionSpawn { get; set; }

        /// <summary>
        /// This event is called whenever the action CreateAction is applied
        /// </summary>
        public static event System.Action<GameObject> OnCreateEvent;

        /// <summary>
        /// Apply a create action with instantiate the prefab from data to item
        /// Register the new item to ObjectSelectionHandler
        /// </summary>
        public override void Apply()
        {
            if (AssetDataRelated == null || AssetDataRelated.PrefabReference == null)
            {
                Debug.LogError($"{Name} failed because AssetDataRelated null or Prefab is null");
                return;
            }

            if (ItemRelated != null)
            {
                Debug.LogWarning($"{Name} with an already instantiated item. Destroying it, require investigation.");
                GameObject.Destroy(ItemRelated.gameObject);
            }

            ItemRelated = GameObject.Instantiate(AssetDataRelated.PrefabReference, PositionSpawn, Quaternion.identity);
            ItemRelated.Data = AssetDataRelated;

            SceneManager.Instance.RegisterItem(ItemRelated.gameObject);

            OnCreateEvent?.Invoke(ItemRelated.gameObject);
            base.Apply();
        }

        /// <summary>
        /// Destroyed the instantiated item and unregister it from selection if this is the case
        /// </summary>
        public override void Undo()
        {
            if (ItemRelated == null)
            {
                Debug.LogWarning($"{Name} cannot be undo because no item is instantiated.");
                return;
            }

            SceneManager.Instance.UnregisterItem(ItemRelated.gameObject);
            GameObject.Destroy(ItemRelated.gameObject);
        }

        /// <summary>
        /// Create the action with data to instantiated
        /// </summary>
        /// <param name="data"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static CreateAction Create(AssetData data, Vector3 position)
        {
            return new CreateAction(data, position);
        }
    }
}