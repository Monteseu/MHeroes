using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//Note: We load the data libraries via addressables and add it to this services so they're accesible from anywhere (Shop, Game, Inventory, etc.) 
// Library scritable objects must be added as addressable with the label ModelDataLibrary.

public class ModelDataService : BaseService
{
    private Dictionary<Type, ScriptableObject> scriptableObjectDictionary = new Dictionary<System.Type, ScriptableObject>();
    private bool allLibrariesLoaded = false;
    private const string DATA_LIBRARIES_LABEL = "ModelDataLibrary";
    protected override void Awake()
    {
        base.Awake();
        Addressables.LoadAssetsAsync<ScriptableObject>(DATA_LIBRARIES_LABEL, OnLibraryLoaded).Completed += OnLoadCompleted;
    }

    private void OnLibraryLoaded(ScriptableObject library)
    {
        if (library != null)
        {
            var type = library.GetType();
            if (!scriptableObjectDictionary.ContainsKey(type))
            {
                Debug.Log($"Added library {type}");
                scriptableObjectDictionary.Add(type, library);
            }
        }
    }

    private void OnLoadCompleted(AsyncOperationHandle<IList<ScriptableObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("All data libraries loaded successfully");
            allLibrariesLoaded = true;
        }
        else
            Debug.LogError("Failed to load data libraries");
    }
    public bool AreLibrariesReady() => allLibrariesLoaded;
    public T GetLibrary<T>() where T : ScriptableObject
    {
        var type = typeof(T);
        if (scriptableObjectDictionary.TryGetValue(type, out ScriptableObject so))
            return so as T;
        else
        {
            Debug.LogError($"Library {type} is not registered");
            return null;
        }
    }
}