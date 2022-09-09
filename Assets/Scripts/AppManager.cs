using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Manager to load scene and fetch playerpref between scenes
    /// </summary>
    public class AppManager : SingletonMonobehaviour<AppManager>
    {
        public const string PLAYER_PREF_SCENE = "PLAYER_PREF_SCENE";

        private async void Start()
        {
            if (IsEnvScene())
            {
                var levelToLoad = GetSelectedLevelName();

                // New env
                if (string.IsNullOrEmpty(levelToLoad))
                    return;

                var s = DataManager.Instance.ReadAndGetScene(levelToLoad);
                await s;

                if (!s.IsCompletedSuccessfully || s.Result == null)
                    return;

                PlatformEditor.SceneManager.Instance.LoadSceneInstance(s.Result);
            }
        }

        /// <summary>
        /// Is scene environment
        /// </summary>
        /// <returns></returns>
        private static bool IsEnvScene()
        {
            return (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1);
        }

        /// <summary>
        /// Load the scene OnBoarding (index 0)
        /// </summary>
        public void LoadOnBoardingScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Load the scene Environment (index 1)
        /// </summary>
        public void LoadEnvironmentScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Define the level selected to be load on the environment scene
        /// </summary>
        /// <param name="filepath"></param>
        public void SelectScene(string filePath)
        {
            PlayerPrefs.SetString(PLAYER_PREF_SCENE, filePath);
        }

        /// <summary>
        /// Return the selected level scene filePath
        /// Return empty string if nothing is selected (New env)
        /// </summary>
        /// <returns></returns>
        public string GetSelectedLevelName()
        {
            return PlayerPrefs.GetString(PLAYER_PREF_SCENE, string.Empty);
        }
    }
}