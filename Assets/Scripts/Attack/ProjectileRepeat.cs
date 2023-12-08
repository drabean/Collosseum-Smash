using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRepeat : Projectile
{
    public Collider2D col;

    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        base.Shoot(startPos, targetPos);
        StartCoroutine(co_ColToggle());
    }

    IEnumerator co_ColToggle()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.3f);
            col.enabled = false;
            col.enabled = true;
        }
        
    }
}
