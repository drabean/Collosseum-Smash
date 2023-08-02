using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Inst
    {
        get
        {
            if (instance == null)
            {
                // Scene���� �̹� �����ϴ� �ν��Ͻ� ã��
                instance = FindObjectOfType<T>();

                // Scene���� ã�� �� ������ ���ο� �ν��Ͻ� ����
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
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
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class Singleton<T> where T : class, new()
{
    private static object _syncobj = new object();
    private static volatile T _instance = null;
    public static T Inst
    {
        get
        {
            if (_instance == null)
            {
                lock (_syncobj)
                {
                    _instance = new T();
                }
            }
            return _instance;
        }
    }
}
