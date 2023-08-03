using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool
{
    Poolable _prefab;
    Stack<Poolable> _stack;

    public static GameObject ObjectPoolHolder;

    public ObjectPool(string resourcePath)
    {
        _prefab = Resources.Load<Poolable>(resourcePath);
        _stack = new Stack<Poolable>();
    }

    public GameObject Pop()
    {
        if(_stack.Count == 0)
        {
            Poolable newObj = GameObject.Instantiate(_prefab);
            newObj.ActionPush = Push;
            newObj.ActionPop?.Invoke();
            return newObj.gameObject;
        }
        else
        {
            Poolable obj = _stack.Pop();
            obj.gameObject.SetActive(true);
            obj.ActionPop?.Invoke();
            return obj.gameObject;
        }
    }

    public void Push(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        if (ObjectPoolHolder == null) ObjectPoolHolder = new GameObject("Holder");

        obj.transform.SetParent(ObjectPoolHolder.transform, false);
        _stack.Push(obj.GetComponent<Poolable>());
    }
}

public class DictionaryPool : MonoSingleton<DictionaryPool>
{
    Dictionary<string, ObjectPool> dict = new Dictionary<string, ObjectPool>();

    public void Push(GameObject obj)
    {
        obj.GetComponent<Poolable>().Push();
    }
    public void Push(GameObject obj, float time)
    {
        obj.GetComponent<Poolable>().Push(time);
    }


    public GameObject Pop(string resourcesPath)
    {
        if (dict.ContainsKey(resourcesPath))
        {
            return dict[resourcesPath].Pop();
        }
        else
        {
            dict.Add(resourcesPath, new ObjectPool(resourcesPath));

            return dict[resourcesPath].Pop();
        }
    }
}
