using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on editor, null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
/// </summary>
/// <typeparam name="T">Type of the singleton</typeparam>

public abstract class SingletonScriptableObject<T> : SingletonScriptableObjectBase where T : SingletonScriptableObject<T>
{
    protected static T _instance = null;
    protected static LoadHandle _instanceHandle;

    public class LoadHandle
    {
        public Task<T> Task { get { return task; } }
        public T Result { get; private set; }

        public event Action<LoadHandle> Completed
        {
            add
            {
                if (Task.IsCompleted)
                    value.Invoke(this);
                else
                    onComplete += value;
            }
            remove
            {
                onComplete -= value;
            }
        }


        readonly Task<T> task;
        AsyncOperationHandle<T> handle;
        Action<LoadHandle> onComplete;

        public LoadHandle(AsyncOperationHandle<T> handle)
        {
            this.handle = handle;
            task = LoadTask();
        }

        async Task<T> LoadTask()
        {
            await handle.Task;

            Result = handle.Result;
            _instance = Result;

            await _instance.OnSingletonLoad();

            onComplete?.Invoke(this);

            return _instance;
        }
    }



    public static LoadHandle LoadAsync()
    {
        if (_instance || _instanceHandle != null)
        {
            return _instanceHandle;
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(string.Format(ADDRESSABLES_KEY_FORMAT, typeof(T).Name));

        _instanceHandle = new LoadHandle(handle);
        return _instanceHandle;
    }

    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                LoadAsync();
            }

            return _instance;
        }
    }

}

public abstract class SingletonScriptableObjectBase : ScriptableObject
{
    public const string ADDRESSABLES_LOCATION_KEY = "Assets/Singletons";
    public static readonly string ADDRESSABLES_KEY_FORMAT = string.Concat(ADDRESSABLES_LOCATION_KEY, "/{0}.asset");
    protected virtual Task OnSingletonLoad() { return Task.CompletedTask; }
}

[System.Serializable]
public class AssetReferenceSingleton : AssetReferenceT<SingletonScriptableObjectBase>
{
    public AssetReferenceSingleton(string guid) : base(guid) { }
}
