using PlatformEditor;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PlatformEditor
{
    /// <summary>
    /// This a simple monobehaviour that hold prefab references and act like an IAssetLoader
    /// Debug purposes
    /// </summary>
    public class SimplePrefabReferenceLoader : IAssetLoaderBehaviour
    {
        [SerializeField]
        private List<AssetDataListScriptableObject> AssetDatas = new List<AssetDataListScriptableObject>();

        public override Task<AssetLoadingResponse> LoadAssetReferences(ref List<AssetData> assets)
        {
            if (assets == null)
                assets = new List<AssetData>();

            foreach (var dataSet in AssetDatas)
            {
                assets.AddRange(dataSet.Assets);
            }

            // fake little latency on loading
            return Task.Run(() =>
            {
                Thread.Sleep(100);
                return new AssetLoadingResponse()
                {
                    Success = true,
                    ResponseCode = 200,
                    Message = string.Empty
                };
            });
        }
    }
}