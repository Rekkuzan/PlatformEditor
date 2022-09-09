using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PlatformEditor
{
    /// <summary>
    /// Behaviour holding data reference on the scene and loading them on Start()
    /// </summary>
    public class AssetReferenceHandler : MonoBehaviour
    {
        [SerializeField]
        private IAssetLoaderBehaviour Loader;

        [SerializeField]
        private AssetReferenceDisplayer ShortcutUI;

        [SerializeField]
        private AssetReferenceDisplayer AllAssetListUI;


        [SerializeField]
        private List<AssetData> AssetDatas = new List<AssetData>();

        [SerializeField]
        private List<AssetData> ShortcutElements = new List<AssetData>();

        /// <summary>
        /// true is AssetsData are loaded from the loader
        /// </summary>
        public bool AssetLoaded { get; private set; } = false;

        private async void Start()
        {
            var t = Loader.LoadAssetReferences(ref AssetDatas);
            await t;

            if (!t.Result.Success)
            {
                // TODO display proper UI to notify the user an issue happened
                Debug.LogError($"Error while loading assets {t.Result.ResponseCode} {t.Result.Message}");
                return;
            }

            if (ShortcutUI)
                ShortcutUI.Initialize(GetShortCutAsset, SelectAsset, IsSelected);
            if (AllAssetListUI)
                AllAssetListUI.Initialize(GetAllAssets, ToggleAssetShortcut, IsShortcut);

            AssetLoaded = true;
        }

        /// <summary>
        /// Return true if the asset is currently used in the shortcut list
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool IsShortcut(AssetData d)
        {
            return ShortcutElements.Contains(d);
        }

        /// <summary>
        /// Will toggle the asset from the shortcut list 
        /// </summary>
        /// <param name="d"></param>
        public void ToggleAssetShortcut(AssetData d)
        {
            if (d == null)
                return;

            if (ShortcutElements.Contains(d))
                ShortcutElements.Remove(d);
            else
                ShortcutElements.Add(d);

            if (ShortcutUI)
                ShortcutUI.RefreshShortcutList();
        }

        /// <summary>
        /// Will select an asset data for the placement
        /// </summary>
        /// <param name="d"></param>
        public void SelectAsset(AssetData d)
        {
            ObjectSelectionTransform.Instance.SelectNewItem(d);
            ShortcutUI.RefreshToggleState();
        }

        /// <summary>
        /// Is the asset data currently selected
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool IsSelected(AssetData d)
        {
            return 
                ObjectSelectionTransform.Instance.CurrentSelectedData != null 
                && d != null
                && d.NameId == ObjectSelectionTransform.Instance.CurrentSelectedData.NameId;
        }

        /// <summary>
        /// Return the corresponding AssetData from nameId
        /// </summary>
        /// <param name="nameId"></param>
        /// <returns></returns>
        public AssetData GetAssetByNameId(string nameId)
        {
            var result = AssetDatas.FirstOrDefault(x => x.NameId == nameId);

            if (result == null)
            {
                Debug.LogWarning($"No asset with nameid {nameId} were foudn");
                return null;
            }

            return result;
        }

        /// <summary>
        /// Return list of AssetDatas contained on the shortlist
        /// </summary>
        /// <returns></returns>
        public List<AssetData> GetShortCutAsset()
        {
            return ShortcutElements;
        }

        /// <summary>
        /// Return all asset data
        /// </summary>
        /// <returns></returns>
        public List<AssetData> GetAllAssets()
        {
            return AssetDatas;
        }
    }
}