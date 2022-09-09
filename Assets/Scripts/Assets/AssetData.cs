namespace PlatformEditor
{
    /// <summary>
    /// All information about an asset that can be used for loading on environment and UI
    /// </summary>
    [System.Serializable]
    public class AssetData
    {
        public string NameId;
        public RuntimeItem PrefabReference;
        public string ReadableName;
    }
}