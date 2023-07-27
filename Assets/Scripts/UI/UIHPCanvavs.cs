using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHPCanvavs : MonoBehaviour
{
    [SerializeField] RectTransform heartTr;
    [SerializeField] Queue<GameObject> heartQueue = new Queue<GameObject>();
    GameObject heartPrefab;


    private void Awake()
    {
        heartPrefab = Resources.Load<GameObject>("Prefabs/HPBlock");
    }
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
}
