using UnityEngine;

public class Singleton<T> where T : new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = new();
            return instance;
        }
    }
}

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool isDontDestroyOnLoad;

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    Debug.LogError($"[MonoSingleton] No instance of {typeof(T)} found in the scene.");
                    return null;
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // 이미 인스턴스가 존재하면 새로 생긴 건 파괴
            Destroy(gameObject);
        }
    }
}
