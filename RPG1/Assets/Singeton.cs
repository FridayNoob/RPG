using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singeton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static Transform managerParent;

    public static T Instance
    {  
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject($"{typeof(T).Name}Manager");
                _instance = singletonObject.AddComponent<T>();
                singletonObject.transform.parent = managerParent;
                DontDestroyOnLoad(singletonObject );
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
