using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float explosionTime;
    [SerializeField] GameObject explosion;

    private void Awake()
    {
        Invoke("doExplode", explosionTime);
        GameMgr.Inst.AttackEffectCircle(transform.position, 1.5f, explosionTime);
    }
    public void doExplode()
    {
        GetComponent<Animator>().SetTrigger("doExplosion");
    }

    public void Explode()
    {
        DictionaryPool.Inst.Pop("Prefabs/Explosion").transform.position = transform.position;

        Destroy(gameObject);
    }
}
