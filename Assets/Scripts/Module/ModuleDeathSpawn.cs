using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDeathSpawn : MonoBehaviour
{
    public GameObject prefab;

    private void Awake()
    {
        Enemy enem = GetComponent<Enemy>();
        if(enem == null)
        {
            Debug.Log("Enemy script not found");
            Destroy(this);
        }
        enem.ActionOnDeath += spawnOnDeath;
    }

    void spawnOnDeath(Vector3 pos)
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
