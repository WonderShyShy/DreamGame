#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HapticClipMenu
{
    private static string lastSearchPath = "Assets";
    private static string lastSoPath = "Assets/Resources";

    [MenuItem("Tools/Haptics/Save Clips to SO (Advanced)")]
    private static void SaveClipsToSOAdvanced()
    {
        // 选择搜索文件夹
        string selectedFolder = EditorUtility.OpenFolderPanel("Select Clip Folder", lastSearchPath, "");
        if (string.IsNullOrEmpty(selectedFolder)) return;
        
        // 转换路径为 Unity 相对路径
        string searchFolder = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
        lastSearchPath = searchFolder;

        // 选择 SO 保存路径
        string soPath = EditorUtility.SaveFilePanelInProject(
            "Save Clip List", 
            "HapticClipsList", 
            "asset", 
            "Select SO Save Location",
            lastSoPath);

        if (string.IsNullOrEmpty(soPath)) return;
        lastSoPath = System.IO.Path.GetDirectoryName(soPath);

        // 执行保存操作
        var clips = HapticClipFinder.GetHapticClipsInFolder(searchFolder);
        SaveToSO(clips, soPath);
    }

    private static void SaveToSO(List<HapticClipWithName> clips, string path)
    {
        ClipListSO clipList = AssetDatabase.LoadAssetAtPath<ClipListSO>(path);
        if (clipList == null)
        {
            clipList = ScriptableObject.CreateInstance<ClipListSO>();
            AssetDatabase.CreateAsset(clipList, path);
        }

        clipList.clips = clips;
        EditorUtility.SetDirty(clipList);
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog("Success", 
            $"Saved {clips.Count} clips to\n{path}", 
            "OK");
    }
}
#endif