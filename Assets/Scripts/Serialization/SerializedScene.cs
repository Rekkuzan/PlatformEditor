using System.Collections.Generic;

namespace PlatformEditor
{
    [System.Serializable]
    public class SerializedScene
    {
        public string Name;
        public string FilePath;
        public List<SerializedItem> Items = new List<SerializedItem>();
    }
}