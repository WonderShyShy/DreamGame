using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvent 
{
   
    private static GameEvent _instance;
    public static GameEvent Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEvent();
            }
            return _instance;
        }
    }
    
    public UnityEvent clearLine = new UnityEvent();
    
    public static void ResetInstance()
    {
        if (_instance != null)
        {
            _instance.clearLine?.RemoveAllListeners();
        }
        _instance = null;
    }
}