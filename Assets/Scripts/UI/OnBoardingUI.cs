using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace PlatformEditor
{
    /// <summary>
    /// Behaviour to handle UI of the onBoarding scene
    /// </summary>
    public class OnBoardingUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject EmptyWorldUI;

        [SerializeField]
        private GameObject NonEmptyWorldUI;


        [SerializeField]
        private SceneListDisplayer ListUI;

        private async void Start()
        {
            await Task.Yield();
            DataManager.Instance.LoadScenes();
        }

        private void OnEnable()
        {
            DataManager.OnDataUpdated += OnDataUpdated;
        }

        private void OnDisable()
        {
            DataManager.OnDataUpdated -= OnDataUpdated;
        }

        private void OnDataUpdated()
        {
            ListUI.RefreshList();
            EmptyWorldUI.SetActive(DataManager.Instance.SerializedSceneCount == 0);
            NonEmptyWorldUI.SetActive(DataManager.Instance.SerializedSceneCount > 0);
        }
    }
}