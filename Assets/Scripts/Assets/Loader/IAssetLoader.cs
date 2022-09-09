using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// Abstract in order to load asset database (From network, asset bundles, addressable, debug fixed prefb holder etc.)
    /// </summary>
    public abstract class IAssetLoaderBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Load and insert all asset datas into assets list
        /// </summary>
        public abstract Task<AssetLoadingResponse> LoadAssetReferences(ref List<AssetData> assets);
    }

    /// <summary>
    /// Basic generic response after loading assets
    /// </summary>
    public struct AssetLoadingResponse
    {
        public bool Success;
        public int ResponseCode;
        public string Message;
    }
}