using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PlatformEditor
{
    public class SceneManager : SingletonMonobehaviour<SceneManager>
    {
        [SerializeField]
        private AssetReferenceHandler AssetReferences;

    private SerializedScene _serializedScene = new SerializedScene()
    {
        Items = new List<SerializedItem>()
    };

        private readonly List<RuntimeItem> _allItems = new List<RuntimeItem>();

        public object ItemRelated { get; private set; }

        /// <summary>
        /// Register gameobject for saving state
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterItem(GameObject obj)
        {
            var runtimeItem = obj.GetComponent<RuntimeItem>();
            if (_allItems.Contains(runtimeItem))
                return;
            _allItems.Add(obj.GetComponent<RuntimeItem>());
        }


        /// <summary>
        /// Unregister gameobject
        /// </summary>
        /// <param name="obj"></param>
        public void UnregisterItem(GameObject obj)
        {
            _allItems.Remove(obj.GetComponent<RuntimeItem>());
        }

        /// <summary>
        /// Will remove all item permanentely
        /// </summary>
        public void RemoveAll()
        {
            foreach (var i in _allItems)
                Destroy(i.gameObject);
            _allItems.Clear();
        }

        /// <summary>
        /// Return the current scene in a serialized object
        /// </summary>
        /// <returns></returns>
        public SerializedScene GetSerializedScene()
        {
            _serializedScene.Items.Clear();

            foreach (var i in _allItems)
            {
                _serializedScene.Items.Add(i.GetSerializedItem());
            }

            return _serializedScene;
        }

        /// <summary>
        /// Will load the SerializedScene
        /// </summary>
        /// <param name="scene"></param>
        public async void LoadSceneInstance(SerializedScene scene)
        {
            while (!AssetReferences.AssetLoaded)
                await Task.Delay(100);

            foreach (var i in scene.Items)
            {
                var data = AssetReferences.GetAssetByNameId(i.AssetNameId);

                if (data == null)
                    continue;

                var runtimeItem = Instantiate(data.PrefabReference, i.Position, Quaternion.identity);
                runtimeItem.ApplyPosition(i.Position);
                runtimeItem.ApplyRotation(Quaternion.Euler(i.Rotation));
                runtimeItem.ApplyScale(i.Scale);

                runtimeItem.Data = data;

                this.RegisterItem(runtimeItem.gameObject);
            }
        }
    }
}