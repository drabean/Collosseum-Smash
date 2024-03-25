using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHPCanvavs : MonoBehaviour
{
    [SerializeField] RectTransform heartTr;
    [SerializeField] RectTransform heartBackTr;
    [SerializeField] Queue<GameObject> heartQueue = new Queue<GameObject>();
    [SerializeField] Queue<GameObject> heartBackQueue = new Queue<GameObject>();
    GameObject heartPrefab;
    GameObject heartBackPrefab;


    private void Awake()
    {
        heartPrefab = Resources.Load<GameObject>("Prefabs/HPBlock");
        heartBackPrefab = Resources.Load<GameObject>("Prefabs/HPBlockBack");
    }

    #region 현재체력
    public void Set(int HP)
    {
        int count = HP - heartQueue.Count;

        if (count > 0)
        {
            for (int i = 0; i < count; i++) addHeart();

        }
        else if(count < 0)
        {
            for (int i = 0; i < Mathf.Abs(count); i++) deleteHeart();
        }
    }

    void addHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(heartTr, false);
        heartQueue.Enqueue(newHeart);
    }

    void deleteHeart()
    {
        if (heartQueue.Count == 0) return;
        Destroy(heartQueue.Dequeue());
    }
    #endregion

    #region 뒷판
    public void SetMaxHP(int maxHP)
    {
        int count = maxHP - heartBackQueue.Count;

        if (count > 0)
        {
            for (int i = 0; i < count; i++) addHeartBack();

        }
        else if (count < 0)
        {
            for (int i = 0; i < Mathf.Abs(count); i++) deleteHeartBack();
        }
    }

    void addHeartBack()
    {
        GameObject newHeart = Instantiate(heartBackPrefab);
        newHeart.transform.SetParent(heartBackTr, false);
        heartBackQueue.Enqueue(newHeart);
    }

    void deleteHeartBack()
    {
        if (heartBackQueue.Count == 0) return;
        Destroy(heartBackQueue.Dequeue());
    }
    #endregion
}
