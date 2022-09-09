using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class PrefabPlatformCreatorWindows : EditorWindow
{
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Tools/PrefabCreator")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow<PrefabPlatformCreatorWindows>("Prefab Creator");
    }

    public PrefabSettings Settings = new PrefabSettings();

    Editor editor;

    void OnGUI()
    {
        GUILayout.Space(20);
        GUILayout.Label("Start creating your prefab by filling GameObject prefabs on the list", EditorStyles.boldLabel);
        GUILayout.Space(20);
        if (!editor) { editor = Editor.CreateEditor(this); }
        if (editor) { editor.OnInspectorGUI(); }

        if (GUILayout.Button("Generate"))
        {
            Debug.Log("BUtton clicked");
            GeneratePrefabs();
        }
    }

    void OnInspectorUpdate() { Repaint(); }

    private void GeneratePrefabs()
    {
        List<GameObject> Items = new List<GameObject>();

        foreach (var g in Settings.Prefabs)
        {
            Debug.Log($"Asset is {g.name}");

            var parentPrefab = (PlatformEditor.RuntimeItem)PrefabUtility.InstantiatePrefab(Settings.DefaultPrefab);

            var prefabType = PrefabUtility.GetPrefabAssetType(g);

            GameObject newObject;
            if (prefabType != PrefabAssetType.NotAPrefab)
            {
                newObject = (GameObject)PrefabUtility.InstantiatePrefab(g);
            }
            else
            {
                newObject = Instantiate(g);
            }

            newObject.name = $"{g.name}-original";
            parentPrefab.name = g.name;

            Transform lowestChildParent = parentPrefab.transform;
            // Find the lowest child
            while (lowestChildParent.childCount > 0)
                lowestChildParent = lowestChildParent.GetChild(0);

            newObject.transform.parent = lowestChildParent;
            parentPrefab.AssignObject(newObject);
            var prefabSaved = SaveGameObjectAsNewPrefab(parentPrefab.gameObject);
            if (prefabSaved != null)
                Items.Add(prefabSaved);
            DestroyImmediate(parentPrefab);
        }

        CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        TextInfo textInfo = cultureInfo.TextInfo;
        var s = ScriptableObject.CreateInstance<AssetDataListScriptableObject>();
        foreach (var i in Items)
        {
            var readableName = textInfo.ToTitleCase(i.name.Replace('-', ' '));
            readableName = readableName.Replace('_', ' ');

            s.Assets.Add(new PlatformEditor.AssetData
            {
                PrefabReference = i.GetComponent<PlatformEditor.RuntimeItem>(),
                NameId = i.name,
                ReadableName = readableName
            }); 
        }

        if (!Directory.Exists("Assets/AssetsData"))
            AssetDatabase.CreateFolder("Assets", "AssetsData");
        string localPath = "Assets/AssetsData/" + Settings.NameOfList + ".asset";
        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        AssetDatabase.CreateAsset(s, localPath);
        AssetDatabase.SaveAssets();
    }

    private GameObject SaveGameObjectAsNewPrefab(GameObject t)
    {
        // Create folder Prefabs and set the path as within the Prefabs folder,
        // and name it as the GameObject's name with the .Prefab format
        if (!Directory.Exists("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        string localPath = "Assets/Prefabs/" + t.name + ".prefab";

        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        // Create the new Prefab and log whether Prefab was saved successfully.
        bool prefabSuccess;
        var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(t, localPath, InteractionMode.UserAction, out prefabSuccess);
        if (!prefabSuccess)
        {
            Debug.LogError("Prefab failed to save" + prefabSuccess);
            return null;
        }
        return prefab;
    }
}


[System.Serializable]
public class PrefabSettings
{
    public List<GameObject> Prefabs = new List<GameObject>();
    public PlatformEditor.RuntimeItem DefaultPrefab;
    public string NameOfList = "Data Set";
}

[CustomEditor(typeof(PrefabPlatformCreatorWindows), true)]
public class ListTestEditorDrawer : Editor
{

    public override void OnInspectorGUI()
    {
        var list = serializedObject.FindProperty("Settings");
        EditorGUILayout.PropertyField(list, new GUIContent("Prefabs to create"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
