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
        enem.onDeath += spawnOnDeath;
    }

    void spawnOnDeath()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
