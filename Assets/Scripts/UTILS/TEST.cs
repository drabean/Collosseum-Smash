using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ObjectPooler))]
public class ObjectPoolerEditor : Editor
{
    const string INFO =
        " Ǯ���� ������Ʈ�� OnDisable() �ȿ� ������ ��������~ " +
        "\nvoid OnDisable()\n" +
        "{\nObjectPooler.ReturnToPool(gameobject); \n" +
        "CancelInvoke(); //invoke �Լ��� ����ϴ� ��������ּ���\n}";
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(INFO, MessageType.Info);
        base.OnInspectorGUI();
    }
}
#endif

[System.Serializable]
public class Pool
{
    public string Name;
    public GameObject Prefab;
    public int Number;
    public bool IsUi;
}
public class ObjectPooler : MonoBehaviour
{
    private static ObjectPooler s_inst;
    private void Awake()
    {
        s_inst = this;
    }
    [SerializeField] private RectTransform _uiCanvasPooler;
    [SerializeField] private Pool[] _pools;
    private List<GameObject> _spawnObjects;
    private Dictionary<string, Queue<GameObject>> _dictionaryPool;
    //���߿� �ʿ�� �ʱ�ȭ ������ ������ �� ����
    readonly string INFO =
        " Ǯ���� ������Ʈ�� OnDisable() �ȿ� ������ ��������~ " +
        "\nvoid OnDisable()\n" +
        "{\nObjectPooler.ReturnToPool(gameobject); \n" +
        "CancelInvoke(); //invoke �Լ��� ����ϴ� ��������ּ���\n}";

    private void Start()
    {
        _spawnObjects = new List<GameObject>();
        _dictionaryPool = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in _pools)
        {
            _dictionaryPool.Add(pool.Name, new Queue<GameObject>());
            for (int i = 0; i < pool.Number; i++)
            {
                GameObject obj;
                //Ingame ������Ʈ�� UI ������Ʈ ����
                if (pool.IsUi == false)
                    obj = CreateNewObject(pool.Name, pool.Prefab);
                else
                    obj = CreateNewObject(pool.Name, pool.Prefab, _uiCanvasPooler);

                if (pool.IsUi == false)
                    ArrangePool(obj);
                else
                    ArrangePool(obj, _uiCanvasPooler);
            }
            //OnDisable�� ReturnToPool ���� �����ߺ� ���� �˻�
            if (_dictionaryPool[pool.Name].Count <= 0)
                Debug.LogError($"{pool.Name}{INFO}");
            else if (_dictionaryPool[pool.Name].Count != pool.Number)
                Debug.LogError($"{pool.Name}�� returnToPool�� �ߺ��˴ϴ�");
        }
    }

    //������ ���� �����ε�
    public static GameObject SpawnFromPool(string name, Vector3 position) =>
        s_inst._SpawnFromPool(name, position, Quaternion.identity);
    public static GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation) =>
        s_inst._SpawnFromPool(name, position, rotation);

    public static T SpawnFromPool<T>(string name, Vector3 position) where T : Component
    {
        GameObject obj = s_inst._SpawnFromPool(name, position, Quaternion.identity);
        if (obj.TryGetComponent(out T component))
            return component;
        else
        {
            obj.SetActive(false);
            throw new Exception($"Component not found");
        }
    }
    public static T SpawnFromPool<T>(string name, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject obj = s_inst._SpawnFromPool(name, position, rotation);
        if (obj.TryGetComponent(out T component))
            return component;
        else
        {
            obj.SetActive(false);
            throw new Exception($"Component not found");
        }
    }
    private GameObject _SpawnFromPool(string name, Vector3 position, Quaternion rotation)
    {
        if (!_dictionaryPool.ContainsKey(name))
            throw new Exception($"pool with name {name} doesn't exist");
        //ť�� ������ ���� �߰�
        Queue<GameObject> poolQueue = _dictionaryPool[name];
        if (poolQueue.Count == 0)
        {
            Pool pool = Array.Find(_pools, x => x.Name == name);
            GameObject obj;
            if (pool.IsUi == false)
                obj = CreateNewObject(pool.Name, pool.Prefab);
            else
                obj = CreateNewObject(pool.Name, pool.Prefab, _uiCanvasPooler);
            if (pool.IsUi == false)
                ArrangePool(obj);
            else
                ArrangePool(obj, _uiCanvasPooler);

        }
        //ť���� ������ ���
        GameObject objectToSpawn = poolQueue.Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);
        return objectToSpawn;
    }
    public static List<GameObject> GetAllPools(string name)
    {
        if (!s_inst._dictionaryPool.ContainsKey(name))
            throw new Exception($"Pool with name {name} doesn't exist");

        return s_inst._spawnObjects.FindAll(x => x.name == name);
    }

    public static List<T> GetAllPools<T>(string name) where T : Component
    {
        List<GameObject> objects = GetAllPools(name);
        if (objects[0].TryGetComponent(out T component))
            throw new Exception("Component not Fount");
        return objects.ConvertAll(x => x.GetComponent<T>());
    }
    public static void ReturnToPool(GameObject obj)
    {
        if (s_inst._dictionaryPool.ContainsKey(obj.name) == false)
            throw new Exception($"Pool with name{obj.name} dosen't exist");

        s_inst._dictionaryPool[obj.name].Enqueue(obj);
    }

    //Ǯ�� ������Ʈ ������ �������ؼ� �����Ϳ� ��ư ����� ��
    [ContextMenu("GetSpawnObjectsInfo")]
    private void GetSpawnObjectInfo()
    {
        foreach (var pool in _pools)
        {
            int count = _spawnObjects.FindAll(x => x.name == pool.Name).Count; //����Ʈ�� ���� 
            Debug.Log($"{pool.Name} Count : {count}");
        }
    }

    private GameObject CreateNewObject(string name, GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.name = name;
        obj.SetActive(false); //��Ȱ��ȭ�� ReturnToPool�� �ϱ� ������ enqueue�ȴ�.
        return obj;
    }
    private GameObject CreateNewObject(string name, GameObject prefab, RectTransform trans)
    {
        var obj = Instantiate(prefab, trans);
        obj.name = name;
        obj.SetActive(false); //��Ȱ��ȭ�� ReturnToPool�� �ϱ� ������ enqueue�ȴ�.
        return obj;
    }
    #region ArrangePool overroding
    /// <summary>
    /// ������Ʈ Ǯ���� ���� ������ ��ü���� �̸����� ���̾��Ű�信 �������ִ� �Լ�
    /// </summary>
    /// <param name="obj"></param>
    private void ArrangePool(GameObject obj)
    {
        //�߰��� ������Ʈ ��� ����
        bool isFind = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == transform.childCount - 1) //�������� ���
            {
                obj.transform.SetSiblingIndex(i);
                _spawnObjects.Insert(i, obj);
                break;
            }
            else if (transform.GetChild(i).name == obj.name)
                isFind = true;
            else if (isFind)
            {
                obj.transform.SetSiblingIndex(i);
                _spawnObjects.Insert(i, obj);
            }
        }
    }

    private void ArrangePool(GameObject obj, RectTransform parent)
    {
        //�߰��� ������Ʈ ��� ����
        bool isFind = false;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (i == parent.transform.childCount - 1)
            {
                obj.transform.SetSiblingIndex(i);
                _spawnObjects.Insert(i, obj);
                break;
            }
            else if (parent.transform.GetChild(i).name == obj.name)
                isFind = true;
            else if (isFind)
            {
                obj.transform.SetSiblingIndex(i);
                _spawnObjects.Insert(i, obj);
            }
        }
    }
    #endregion
}