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
        GameMgr.Inst.AttackEffectCircle(transform.position, 1.55f, explosionTime);
    }
    public void doExplode()
    {
        GetComponent<Animator>().SetTrigger("doExplosion");
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
