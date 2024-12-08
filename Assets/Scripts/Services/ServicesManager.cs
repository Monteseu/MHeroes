using System;
using System.Collections.Generic;
using UnityEngine;
/*
  Note: While on <Development>, this object will instatiate itself from anywhere.
 * In <Production>, remember to put this guy ONLY in the first scene or whenever we intend to start loading the game.
 */
public class ServicesManager : MonoBehaviour
{
    static ServicesManager instance;
    public static ServicesManager Get() => instance;
    private Dictionary<Type, BaseService> registeredServices = new Dictionary<Type, BaseService>();

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoInit()
    {
        if (instance != null)
            return;
        // We also could load it through addressables, but since this is just for Development I'll stick with the classic.
        instance = Instantiate(Resources.Load<ServicesManager>("ServicesManager"));
        instance.name = "Services Manager";
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(instance);
    }
    protected virtual void Awake() => instance = this;

#else
   protected virtual void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(instance);
    }
#endif

    public void RegisterService(BaseService service)
    {
        var type = service.GetType();

        if (!registeredServices.ContainsKey(type))
        {
            registeredServices.Add(type, service);
            Debug.Log($"Service {type} registered");
        }
        else
            Debug.LogWarning($"Service {type} is already registered");
    }

    public T GetService<T>() where T : BaseService
    {
        var type = typeof(T);
        if (registeredServices.TryGetValue(type, out BaseService service))
            return service as T;
        else
        {
            Debug.LogError($"Service {type} is not registered");
            return null;
        }
    }
}