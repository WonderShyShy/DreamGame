using System;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEditor;
using UnityEngine;

public class ClipListSO : ScriptableObject
{
    public List<HapticClipWithName> clips;
}

[Serializable]
public class HapticClipWithName
{ 
    public HapticClip clip;
    public string name;
}
    
#if UNITY_EDITOR
public class HapticClipFinder
{
    public static List<HapticClipWithName> GetHapticClipsInFolder(string folderPath)
    {
        List<HapticClipWithName> clips = new List<HapticClipWithName>();

        // 搜索指定文件夹中的所有 HapticClip 类型的资源
        string[] guids = AssetDatabase.FindAssets("t:HapticClip", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            HapticClip clip = AssetDatabase.LoadAssetAtPath<HapticClip>(assetPath);

            if (clip != null)
            {
                clips.Add(new HapticClipWithName()
                {
                    clip = clip,
                    name = clip.name
                });
            }
        }

        return clips;
    }
}
#endif