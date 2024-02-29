using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubShooter : MonoBehaviour
{
    [SerializeField] ModuleFollow follow;
    public Attack attack;

    Player target;

    public float attackBeforeTime;
    public float shootInterval;
    GameObject curWarning;
    Coroutine autoShootCoroutine;

    public void Init(Transform owner, Player target)
    {
        this.target = target;
        if(follow != null) follow.Target = owner;
        GetComponent<ModuleHit>().FlashWhite(0.6f);
    }

    public void StartAutoShoot()
    {
        autoShootCoroutine = StartCoroutine(co_AutoShoot());
    }

    public void StopAutoShoot()
    {
        StopCoroutine(autoShootCoroutine);
    }
    IEnumerator co_AutoShoot()
    {
        yield return new WaitForSeconds(2.0f);
        while (true)
        {
            ShootOnce();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    public void ShootOnce()
    {
        StartCoroutine(co_ShootOnce());
    }
    IEnumerator co_ShootOnce()
    {
        Vector3 targetPos = calcPlayerPos(attackBeforeTime);
        curWarning = attack.ShowWarning(transform.position, targetPos, attackBeforeTime);

        yield return new WaitForSeconds(attackBeforeTime);
        Instantiate(attack).Shoot(transform.position, targetPos);
    }
    public Vector3 calcPlayerPos(float time)
    {
        Vector3 playerpos = target.transform.position;

        playerpos += (target.aim.transform.position - target.transform.position) * target.moveSpeed * time;

        return playerpos;

    }

    public void ShootCount(int count)
    {
        StartCoroutine(Shoot(count));
    }

    IEnumerator  Shoot(int count)
    {
        while (count >0)
        {
            ShootOnce();
            yield return new WaitForSeconds(shootInterval);
            count--;
        }
    }

    private void OnDestroy()
    {
        if (curWarning != null) Destroy(curWarning);
    }


}
