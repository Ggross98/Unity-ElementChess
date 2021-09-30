using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T: MonoBehaviour
{
    private static volatile T instance;
    private static object syncRoot = new Object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        T[] instances = FindObjectsOfType<T>();
                        if (instances != null)
                        {
                            instance = instances[0];
                            //DontDestroyOnLoad(instance);
                            return instance;
                        }
                        else
                        {
                            GameObject go = new GameObject();
                            go.name = typeof(T).Name;
                            instance = go.AddComponent<T>();
                            //DontDestroyOnLoad(go);
                        }
                        
                    }
                }
            }
            return instance;
        }
    }
}
