using System.Collections.Generic;
using Other;
using UnityEngine;

namespace Manager
{
    public static class ManagerDialog
    {
        private static readonly Dictionary<string, GameObject> Dialogs = new Dictionary<string, GameObject>();
        private static GameObject _rootNode = null;
        private static readonly Dictionary<string, GameObject> ShowDialogs = new Dictionary<string, GameObject>();
        
        public static void InitData(GameObject rootNode)
        {
            _rootNode = rootNode;
        }
        
        public static void AddDialog(string dialogName, GameObject dialogPrefab)
        {
            if (dialogPrefab != null)
            {
                if (!Dialogs.ContainsKey(dialogName))
                {
                    Dialogs.Add(dialogName, dialogPrefab);
                }
            }
            else
            {
                DebugEx.LogError("dialogPrefab is null");
            }
        }

        public static GameObject CreateDialog(string dialogName)
        {
            DestroyDialog(dialogName);
            
            if (Dialogs.ContainsKey(dialogName))
            {
                var dialog = Object.Instantiate(Dialogs[dialogName], _rootNode.transform, false);
                dialog.transform.localScale = Vector2.one;
                ShowDialogs.Add(dialogName, dialog);
                return dialog;
            }
            
            DebugEx.Log(dialogName + "未加入缓存");
            return null;
        }

        public static void DestroyDialog(string dialogName)
        {
            if (ShowDialogs.ContainsKey(dialogName))
            {
                Object.Destroy(ShowDialogs[dialogName]);
                ShowDialogs.Remove(dialogName);
            }
        }

        public static bool IsExistDialogRes(string dialogName)
        {
            return Dialogs.ContainsKey(dialogName);
        }

        public static GameObject IsExistDialog(string dialogName)
        {
            if (ShowDialogs.ContainsKey(dialogName))
            {
                return ShowDialogs[dialogName];
            }

            return null;
        }
    }
}
