using Newtonsoft.Json;
using UnityEngine;

namespace Other
{
    public static class DebugEx
    {
    
        [System.Diagnostics.Conditional("LOG")]
        public static void Log(params object[] message)
        {
            var s = "";
            foreach (var tmpS in message)
            {
                s += tmpS + "\t";
            }
            Debug.Log(s);
        }
        
        [System.Diagnostics.Conditional("LOG")]
        public static void LogObject(params object[] message)
        {
            var s = "";
            foreach (var tmpS in message)
            {
                s += JsonConvert.SerializeObject(tmpS) + "\t";
            }
            Debug.Log(s);
        }
        
        [System.Diagnostics.Conditional("LOG")]
        public static void LogWarning(params object[] message) {
            var s = "";
            foreach (var tmpS in message)
            {
                s += tmpS + "\t";
            }
            Debug.LogWarning(s);
        }

        [System.Diagnostics.Conditional("LOG")]
        public static void LogError(object message, Object obj = null) {
            Debug.LogError(message, obj);
        }
    }
}
