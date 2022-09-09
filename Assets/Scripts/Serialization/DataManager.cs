using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Manager to read/write data scene serialized to disk
    /// </summary>
    public class DataManager : SingletonMonobehaviour<DataManager>
    {
        [SerializeField]
        private string FolderName = "Scenes";

        private string _finalPath;
        private readonly List<SerializedScene> serializedScenes = new List<SerializedScene>();

        public int SerializedSceneCount => serializedScenes.Count;
        public SerializedScene GetSerializedSceneIndex(int index) => serializedScenes[index];

        public static event System.Action OnDataUpdated;

        /// <summary>
        /// Ensure directory exist and _finalPath variable is constructed
        /// </summary>
        private void EnsureDirectoryCreated()
        {
            _finalPath = Path.Combine(Application.persistentDataPath, FolderName);
            if (!Directory.Exists(_finalPath))
                Directory.CreateDirectory(_finalPath);
        }

        /// <summary>
        /// Will read eveyfile from persistant directory and load all serialized scene data
        /// </summary>
        /// <param name="OnEnd"></param>
        public async void LoadScenes()
        {
            EnsureDirectoryCreated();
            serializedScenes.Clear();

            var allfiles = Directory.GetFiles(_finalPath);
            foreach (var file in allfiles)
            {
                var data = File.ReadAllTextAsync(file);

                await data;

                if (!data.IsCompletedSuccessfully)
                {
                    Debug.LogWarning($"Error while reading {file}, skipping it.");
                    continue;
                }

                var sceneSerialized = JsonUtility.FromJson<SerializedScene>(data.Result);
                // ensure the filepath saved is always the right one (In case the file has its filepath changed externally)
                sceneSerialized.FilePath = file;

                serializedScenes.Add(sceneSerialized);
            }

            OnDataUpdated?.Invoke();
        }


        /// <summary>
        /// Will serialize the scene and save it to persistant file
        /// OnEnd is called when operation over
        /// </summary>
        /// <param name="name"></param>
        /// <param name="OnEnd"></param>
        public async void SaveScene(string name, System.Action<bool> OnEnd = null)
        {
            if (SceneManager.Instance == null)
                return;

            EnsureDirectoryCreated();

            if (string.IsNullOrEmpty(name))
                name = "empty";

            var scene = SceneManager.Instance.GetSerializedScene();
            scene.Name = name;

            var fileName = $"{name.RemoveSpecialCharacters()}.json";

            // Avoid having name with special characters to have the same filepath
            int incremental = 1;
            while (File.Exists(fileName))
            {
                fileName = $"{name.RemoveSpecialCharacters()}-{incremental}.json";
                await Task.Yield();
            }

            var filePath = Path.Combine(_finalPath, fileName);

            scene.FilePath = filePath;
            var data = JsonUtility.ToJson(scene);

            var t = File.WriteAllTextAsync(filePath, data);
            await t;

            OnEnd?.Invoke(t.IsCompletedSuccessfully);
        }

        /// <summary>
        /// Will delete the scene file
        /// </summary>
        /// <param name="name"></param>
        public void DeleteScene(string filePath)
        {
            var index = serializedScenes.FindIndex(x => x.FilePath == filePath);

            if (index == -1 || index >= serializedScenes.Count)
            {
                Debug.LogWarning($"Scene {filePath} not found");
                return;
            }

            if (File.Exists(serializedScenes[index].FilePath))
                File.Delete(serializedScenes[index].FilePath);

            serializedScenes.RemoveAt(index);
            OnDataUpdated?.Invoke();
        }

        /// <summary>
        /// Will try to read the associated filepath and load the serialized file
        /// Will return null if file doesn't exist or an issue occured while decoding file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<SerializedScene> ReadAndGetScene(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var data = File.ReadAllTextAsync(filePath);

            await data;

            if (!data.IsCompletedSuccessfully)
            {
                Debug.LogWarning($"Error while reading {filePath}, skipping it.");
                return null;
            }

            return JsonUtility.FromJson<SerializedScene>(data.Result);
        }

    }
}