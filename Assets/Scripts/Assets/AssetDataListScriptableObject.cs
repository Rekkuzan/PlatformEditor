using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSet", menuName = "AssetData/Create", order = 1)]
public class AssetDataListScriptableObject : ScriptableObject
{
    public List<PlatformEditor.AssetData> Assets = new List<PlatformEditor.AssetData>();
}
