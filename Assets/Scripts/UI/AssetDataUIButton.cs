using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Behaviour holding reference to UI related to Asset data button
    /// </summary>
    public class AssetDataUIButton : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI Text;

        [SerializeField]
        private UnityEngine.UI.Button Button;

        [SerializeField]
        private UnityEngine.UI.Image Outline;

        [SerializeField]
        private Color Selected;

        [SerializeField]
        private Color NotSelected;

        public AssetData Data { get; private set; }
        private AssetReferenceDisplayer _displayer;

        public void Initialize(AssetData d, AssetReferenceDisplayer displayer)
        {
            Data = d;
            _displayer = displayer;
            Button?.onClick.RemoveAllListeners();
            Button?.onClick.AddListener(OnClick);

            Text.text = Data.ReadableName;
            //Todo more if texture
        }

        public void SetIsToggle(bool enabled)
        {
            Outline.color = enabled ? Selected : NotSelected;
        }

        private void OnClick()
        {
            _displayer.OnClick(this);
        }
    }
}