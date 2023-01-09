using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Similar to a singleton, but instead of destroying new instances, it overrides the current instance.
/// Handy for resetting the state and saves you doing it manually.
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance {get; private set;}
    protected virtual void Awake() => Instance = this as T;
    
    protected virtual void OnApplicationQuit(){
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// Transforms the static instance into a basic singleton. This will destroy any new
/// versions created, leaving the original intact.
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake(){
        if(Instance != null){
            Destroy(gameObject);
        }
        base.Awake();
    }
}

/// <summary>
/// Persistent version of the singleton. Will survive through scene loads. Perfect for system
/// classes which require stateful, persistent data. Or uadio sources where music plays through
/// loading screens, etc.
/// </summary>
public abstract class PersistantSingleton<T> : Singleton<T> where T: MonoBehaviour {
    protected override void Awake(){
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
