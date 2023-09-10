using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool
{
    //이 풀에서 사용할 원본 프리팹.
    Poolable _prefab;
    Stack<Poolable> _stack;

    //풀로 돌아온 오브젝트를 정리해둘 Object.
    public static GameObject ObjectPoolHolder;


    /// <summary>
    /// 프리팹의 Resources path를 기반으로 초기화.
    /// </summary>
    /// <param name="resourcePath"></param>
    public ObjectPool(string resourcePath)
    {
        //resourcePath 기반으로 프리팹 로딩.
        _prefab = Resources.Load<Poolable>(resourcePath);
        _stack = new Stack<Poolable>();
    }

    /// <summary>
    /// 풀에서 오브젝트를 꺼내어 return.
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        if(_stack.Count == 0)
        {
            //풀에 남아있는 오브젝트가 없다면, 새로 생성하여 반환.
            Poolable newObj = GameObject.Instantiate(_prefab);
            //풀 반환 Delegate Chain
            newObj.ActionPush = Push;
            //해당 오브젝트가 Pop될때 실행되야 할 작업이 있다면, 실행.
            newObj.Pop();

            return newObj.gameObject;
        }
        else
        {
            Poolable obj = _stack.Pop();
            obj.gameObject.SetActive(true);
            //해당 오브젝트가 Pop될때 실행되야 할 작업이 있다면, 실행.
            obj.Pop();

            return obj.gameObject;
        }
    }

    /// <summary>
    /// 풀에 오브젝트를 반환.
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj)
    {
        if (!obj.activeInHierarchy) return;      //이미 반환된 오브젝트를 한번더 반환하는 상황 막음
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
    //resource path를 기반으로 
    Dictionary<string, ObjectPool> dict = new Dictionary<string, ObjectPool>();

    /// <summary>
    /// 오브젝트를 풀로 반환.
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
    /// 해당 path의 오브젝트 풀에서 오브젝트를 꺼내 return.
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
            //해당 path의 오브젝트 풀이 존재하지 않는다면, 생성하여 dict에 추가
            dict.Add(resourcesPath, new ObjectPool(resourcesPath));

            return dict[resourcesPath].Pop();
        }
    }
}
