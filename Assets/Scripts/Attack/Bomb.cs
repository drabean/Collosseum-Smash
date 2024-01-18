using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float explosionTime;
    [SerializeField] Impact explosion;

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
        Instantiate(explosion, transform.position, Quaternion.identity).Shoot(transform.position, transform.position);
        GameMgr.Inst.MainCam.Shake(0.15f, 30, 0.25f, 0f);
        Destroy(gameObject);
    }
}
