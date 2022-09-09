using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Behaviour UI to request scene name
    /// </summary>
    public class SavingSceneUI : MonoBehaviour
    {
        public TMPro.TMP_InputField NameSceneInputField;
        public UnityEngine.UI.Button SavingButton;

        public TMPro.TextMeshProUGUI ExtraText;

        private void OnEnable()
        {
            ResetState();

            SavingButton.onClick.AddListener(() =>
            {

                var name = NameSceneInputField.text;
                if (string.IsNullOrEmpty(name))
                {
                    NameSceneInputField.image.color = Color.red;
                    return;
                }

                DataManager.Instance.SaveScene(name, (bool success) =>
                {
                    if (!success)
                    {
                        ExtraText.text = "An issue occured while saving the scene";
                        return;
                    }

                    this.gameObject.SetActive(false);
                });
            });
        }

        private void OnDisable()
        {
            ResetState();
        }

        /// <summary>
        /// Reset all state from the saving scene UI
        /// </summary>
        private void ResetState()
        {
            NameSceneInputField.text = string.Empty;
            ExtraText.text = string.Empty;
            NameSceneInputField.image.color = Color.white;
            SavingButton.onClick.RemoveAllListeners();
        }
    }
}