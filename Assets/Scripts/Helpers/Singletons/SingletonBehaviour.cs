using UnityEngine;

public abstract class SingletonBehaviour <T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null || !Instance.isActiveAndEnabled)
        {
            Instance = (T) this;
            //DontDestroyOnLoad(transform.root.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }
}
