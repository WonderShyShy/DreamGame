using UnityEditor;
using UnityEngine;

namespace Plugins.Editor
{
    public class PrintNode :  UnityEditor.Editor{
        [MenuItem("PrintNode/Print")]
        public static void Print() {
            GameObject obj = Selection.activeGameObject;
            string str = "";
            Check(obj.transform, "", ref str);
            Debug.LogWarning(str);
        }
 
        static void Check(Transform tf, string gap, ref string str) {
            str += gap + tf.name + "\n";
            foreach (Transform item in tf) {
                Check(item, gap + "   ", ref str);
            }
        }
    }
}