using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Refreshing / Cleaning scrollview of Scene scerialized list
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.ScrollRect))]
    public class SceneListDisplayer : MonoBehaviour
    {

        [SerializeField]
        private SceneListItem UIListElement;

        [SerializeField]
        private Transform ParentList;

        private readonly List<SceneListItem> _listElements = new List<SceneListItem>();

        /// <summary>
        /// Refresh the list based on DataManager.SerializedScenes
        /// </summary>
        public void RefreshList()
        {
            CleanList();
            for (int i = 0; i < DataManager.Instance.SerializedSceneCount; i++)
            {
                var elem = Instantiate(UIListElement, ParentList);

                var s = DataManager.Instance.GetSerializedSceneIndex(i);
                elem.Initialize(s);

                _listElements.Add(elem);
            }
        }

        /// <summary>
        /// Destropy and empty list of elements contain in scrollview
        /// </summary>
        public void CleanList()
        {
            foreach (var item in _listElements)
                Destroy(item.gameObject);
            _listElements.Clear();
        }
    }
}