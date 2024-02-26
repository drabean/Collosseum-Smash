using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichSubSkul : MonoBehaviour
{
    [SerializeField] ModuleFollow follow;
    public Attack attack;

    Player target;

    public float attackBeforeTime;
    public float shootInterval;
    GameObject curWarning;


    public void Init(Transform owner, Player target)
    {
        this.target = target;
        StartCoroutine(co_Shoot());
        follow.Target = owner;
    }
    IEnumerator co_Shoot()
    {
        while (true)
        {


            Vector3 targetPos = calcPlayerPos(1.0f);

            curWarning = attack.ShowWarning(transform.position, targetPos, 1.5f);

            yield return new WaitForSeconds(attackBeforeTime);
            Instantiate(attack).Shoot(transform.position, targetPos);
            yield return new WaitForSeconds(shootInterval);
        }
    }
    public Vector3 calcPlayerPos(float time)
    {
        Vector3 playerpos = target.transform.position;

        playerpos += (target.aim.transform.position - target.transform.position) * target.moveSpeed * time;

        return playerpos;

    }

    private void OnDestroy()
    {
        if (curWarning != null) Destroy(curWarning);
    }
}
