using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool
{
    //�� Ǯ���� ����� ���� ������.
    Poolable _prefab;
    Stack<Poolable> _stack;

    //Ǯ�� ���ƿ� ������Ʈ�� �����ص� Object.
    public static GameObject ObjectPoolHolder;


    /// <summary>
    /// �������� Resources path�� ������� �ʱ�ȭ.
    /// </summary>
    /// <param name="resourcePath"></param>
    public ObjectPool(string resourcePath)
    {
        //resourcePath ������� ������ �ε�.
        _prefab = Resources.Load<Poolable>(resourcePath);
        _stack = new Stack<Poolable>();
    }

    /// <summary>
    /// Ǯ���� ������Ʈ�� ������ return.
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        if(_stack.Count == 0)
        {
            //Ǯ�� �����ִ� ������Ʈ�� ���ٸ�, ���� �����Ͽ� ��ȯ.
            Poolable newObj = GameObject.Instantiate(_prefab);
            //Ǯ ��ȯ Delegate Chain
            newObj.ActionPush = Push;
            //�ش� ������Ʈ�� Pop�ɶ� ����Ǿ� �� �۾��� �ִٸ�, ����.
            newObj.Pop();

            return newObj.gameObject;
        }
        else
        {
            Poolable obj = _stack.Pop();
            obj.gameObject.SetActive(true);
            //�ش� ������Ʈ�� Pop�ɶ� ����Ǿ� �� �۾��� �ִٸ�, ����.
            obj.Pop();

            return obj.gameObject;
        }
    }

    /// <summary>
    /// Ǯ�� ������Ʈ�� ��ȯ.
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj)
    {
        if (!obj.activeInHierarchy) return;      //�̹� ��ȯ�� ������Ʈ�� �ѹ��� ��ȯ�ϴ� ��Ȳ ����
        obj.gameObject.SetActive(false);
        if (ObjectPoolHolder == null) ObjectPoolHolder = new GameObject("Holder");

        obj.transform.SetParent(ObjectPoolHolder.transform, false);
        _stack.Push(obj.GetComponent<Poolable>());
    }

    public int GetCount()
    {
        return _stack.Count;
    }
}

public class DictionaryPool : MonoSingleton<DictionaryPool>
{
    //resource path�� ������� 
    Dictionary<string, ObjectPool> dict = new Dictionary<string, ObjectPool>();

    /// <summary>
    /// ������Ʈ�� Ǯ�� ��ȯ.
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj)
    {
        obj.GetComponent<Poolable>().Push();
    }
    public void Push(GameObject obj, float time)
    {
        obj.GetComponent<Poolable>().Push(time);
    }

    /// <summary>
    /// �ش� path�� ������Ʈ Ǯ���� ������Ʈ�� ���� return.
    /// </summary>
    /// <param name="resourcesPath"></param>
    /// <returns></returns>
    public GameObject Pop(string resourcesPath)
    {
        if (dict.ContainsKey(resourcesPath))
        {
            return dict[resourcesPath].Pop();
        }
        else
        {
            //�ش� path�� ������Ʈ Ǯ�� �������� �ʴ´ٸ�, �����Ͽ� dict�� �߰�
            dict.Add(resourcesPath, new ObjectPool(resourcesPath));

            return dict[resourcesPath].Pop();
        }
    }
}
