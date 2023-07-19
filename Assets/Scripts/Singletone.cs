using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Inst
    {
        get
        {
            if (instance == null)
            {
                // Scene에서 이미 존재하는 인스턴스 찾기
                instance = FindObjectOfType<T>();

                // Scene에서 찾을 수 없으면 새로운 인스턴스 생성
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