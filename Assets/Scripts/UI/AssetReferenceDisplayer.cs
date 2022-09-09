using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Behaviour to display list of asset data element
    /// </summary>
    public class AssetReferenceDisplayer : MonoBehaviour
    {

        [SerializeField]
        private AssetDataUIButton PrefabButton;

        [SerializeField]
        private RectTransform ParentList;


        private readonly List<AssetData> _datas = new List<AssetData>();
        private readonly List<AssetDataUIButton> _buttons = new List<AssetDataUIButton>();

        private System.Action<AssetData> OnClickAction;
        private System.Func<List<AssetData>> OnRefreshDataList;
        private System.Func<AssetData, bool> OnButtonIsToggle;

        /// <summary>
        /// Initialize the view by referencing the AssetReferenceHandler, list of data it should use, and action on click
        /// </summary>
        public void Initialize( 
            System.Func<List<AssetData>> refreshData, 
            System.Action<AssetData> actionOnClick,
            System.Func<AssetData, bool> onButtonIsToggle)
        {
            OnClickAction = actionOnClick;
            OnRefreshDataList = refreshData;
            OnButtonIsToggle = onButtonIsToggle;

            RefreshShortcutList();
        }

        /// <summary>
        /// Will clear the current scrollview and refresh the list
        /// </summary>
        public void RefreshShortcutList()
        {
            ClearList();

            _datas.Clear();
            var result = OnRefreshDataList?.Invoke();
            if (result != null)
            {
                _datas.AddRange(result);
                foreach (var d in _datas)
                {
                    var button = Instantiate(PrefabButton, ParentList);
                    button.Initialize(d, this);

                    if (OnButtonIsToggle != null)
                    {
                        button.SetIsToggle(OnButtonIsToggle.Invoke(d));
                    }
                    _buttons.Add(button);
                }
            }
        }

        /// <summary>
        /// Will refresh all state is Toggle on the list
        /// </summary>
        public void RefreshToggleState()
        {
            if (OnButtonIsToggle != null)
            {
                foreach (var b in _buttons)
                {
                    b.SetIsToggle(OnButtonIsToggle.Invoke(b.Data));
                }
            }
        }

        /// <summary>
        /// Destroy all UI elements instantiated
        /// </summary>
        public void ClearList()
        {
            while (_buttons.Count > 0)
            {
                Destroy(_buttons[0].gameObject);
                _buttons.RemoveAt(0);
            }
        }


        public void OnClick(AssetDataUIButton button)
        {
            OnClickAction?.Invoke(button.Data);
            if (OnButtonIsToggle != null)
            {
                button.SetIsToggle(OnButtonIsToggle.Invoke(button.Data));
            }
        }
    }
}