using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Linq;

/// Note: This may be hard to understand at start but is pretty verstatile. 
/// Note2: I implemented this on Enemies and Projectiles to show how it can be used in totally different contexts.

/// <summary>
/// Dynamic pool for managing reusable objects. 
/// </summary>
/// <typeparam name="T">Type of the objects to pool.</typeparam>
/// <typeparam name="TContext">Type of the context data to pass during instantiation and retrieve.</typeparam>
public class DynamicPool<T, TContext> where T : MonoBehaviour
{
    // Delegate for asynchronously creating a new instance of T using context.
    private readonly Func<TContext, AsyncOperationHandle<GameObject>> createAsyncFunc;
    // Action to perform when an object is retrieved from the pool.
    private readonly Action<T, TContext> onGet;
    // Action to perform when an object is released back into the pool.
    private readonly Action<T> onRelease;
    private readonly Stack<T> availableObjects;
    private readonly HashSet<T> activeObjects;
    public int TotalObjectCount { get; private set; }

    // Note: Check the references as examples.
    public DynamicPool(
        Func<TContext, AsyncOperationHandle<GameObject>> createAsyncFunc,
        Action<T, TContext> onGet = null,
        Action<T> onRelease = null)
    {
        this.createAsyncFunc = createAsyncFunc;
        this.onGet = onGet;
        this.onRelease = onRelease;

        availableObjects = new Stack<T>();
        activeObjects = new HashSet<T>();
    }

    /// Get (Or create) an object from the pool asynchronously with its context.
    public IEnumerator GetOrCreate(TContext context, Action onComplete = null)
    {
        if (availableObjects.Count > 0)
        {
            T obj = availableObjects.Pop();
            activeObjects.Add(obj);
            onGet?.Invoke(obj, context);
            onComplete?.Invoke();
        }
        else
        {
            var handle = createAsyncFunc(context);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject objGameObject = handle.Result;
                T obj = objGameObject.GetComponent<T>();
                if (obj == null)
                {
                    Debug.LogError($"The instantiated object does not have a component of type {typeof(T)}.");
                    Addressables.ReleaseInstance(objGameObject);
                    onComplete?.Invoke();
                    yield break;
                }

                TotalObjectCount++;
                activeObjects.Add(obj);
                onGet?.Invoke(obj, context);
            }
            else
            {
                Debug.LogError("Failed to instantiate object via Addressables");
            }
            onComplete?.Invoke();
        }
    }

    // Preloads X number of objects so they are ready in the pool.
    public IEnumerator Preload(int count, TContext context, Action onFinished = null)
    {
        for (int i = 0; i < count; i++)
        {
            var handle = createAsyncFunc(context);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject objGameObject = handle.Result;
                objGameObject.SetActive(false);
                T obj = objGameObject.GetComponent<T>();
                if (obj == null)
                {
                    Debug.LogError($"Preload failed for type {typeof(T)}");
                    Addressables.ReleaseInstance(objGameObject);
                    continue;
                }

                TotalObjectCount++;
                availableObjects.Push(obj);
            }
            else
                Debug.LogError("Failed to preload object via Addressables");
        }

        onFinished?.Invoke();
    }

    public void Release(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (!activeObjects.Contains(obj))
        {
            Debug.LogWarning("Attempted to release an object that is not in use by the pool.");
            return;
        }

        activeObjects.Remove(obj);
        onRelease?.Invoke(obj);
        availableObjects.Push(obj);
    }

    public void Dispose()
    {
        foreach (var obj in activeObjects)
        {
            if (obj != null)
            {
                onRelease?.Invoke(obj);
                Addressables.ReleaseInstance(obj.gameObject);
            }
        }
        activeObjects.Clear();

        while (availableObjects.Count > 0)
        {
            T obj = availableObjects.Pop();
            onRelease?.Invoke(obj);
            if (obj != null)
                Addressables.ReleaseInstance(obj.gameObject);
        }
    }

    public IEnumerable<T> ActiveObjects => activeObjects;
}
