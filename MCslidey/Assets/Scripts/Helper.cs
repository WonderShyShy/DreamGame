using UnityEngine;

public class Helper
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
    static public void Log(object message, bool ifSerialize = false)
    {
        if (ifSerialize)
        {
            // Log(JsonConvert.SerializeObject(message), null);
        }
        else {
            Log(message, null);
        }
        
    }

    [System.Diagnostics.Conditional("LOG")]
    static public void Log(object message, Object context)
    {
        Debug.Log(message, context);
    }

    [System.Diagnostics.Conditional("LOG")]
    static public void LogError(object message)
    {
        LogError(message, null);
    }

    [System.Diagnostics.Conditional("LOG")]
    static public void LogError(object message, Object context)
    {

        Debug.LogError(message, context);
        
    }

    [System.Diagnostics.Conditional("LOG")]
    static public void LogWarning(object message)
    {
        LogWarning(message, null);
    }

    [System.Diagnostics.Conditional("LOG")]
    static public void LogWarning(object message, Object context)
    {

       Debug.LogWarning(message, context);

    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogFormat(Object context, string format, params object[] args) {

        Debug.LogFormat(context, format, args);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogFormat(string format, params object[] args) {

        LogFormat(null, format, args);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogWarningFormat(Object context, string format, params object[] args)
    {

        Debug.LogWarningFormat(context, format, args);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogWarningFormat(string format, params object[] args)
    {

        LogWarningFormat(null, format, args);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogErrorFormat(Object context, string format, params object[] args)
    {

        Debug.LogErrorFormat(context, format, args);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        LogErrorFormat(null, format, args);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogException(System.Exception exception, Object context)
    {

        Debug.LogException(exception, context);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogException(System.Exception exception)
    {
        LogException(exception, null);
    }


}
