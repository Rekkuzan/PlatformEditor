using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    public class SceneListItem : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI Title;

        private SerializedScene _scene;

        /// <summary>
        /// Initialize the element with the SerializedScene (Set title text)
        /// </summary>
        /// <param name="scene"></param>
        public void Initialize(SerializedScene scene)
        {
            _scene = scene;
            Title.text = _scene.Name;
        }

        /// <summary>
        /// Will select the level to be load on environment Scene
        /// </summary>
        public void OnSelect()
        {
            AppManager.Instance.SelectScene(_scene.FilePath);
            AppManager.Instance.LoadEnvironmentScene();
        }

        /// <summary>
        /// Will request the DataManager to delete associated file 
        /// </summary>
        public void DeleteScene()
        {
            DataManager.Instance.DeleteScene(_scene.FilePath);
        }
    }
}