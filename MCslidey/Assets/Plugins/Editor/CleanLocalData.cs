using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CleanLocalData : Editor
{
    [MenuItem("Tools/CleanLocalData")]
    static public void Menu1()
    {
        PlayerPrefs.DeleteAll();
    }
}
