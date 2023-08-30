using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigHammer : EquipWeapon
{
    public LayerMask layer;

    protected override void specialAttack()
    {
        base.specialAttack();
        GameMgr.Inst.Shake(0.7f, 9f, 0.2f, 0f, true);
        //CircleCast를 통해 주변 모든 Enemy Layer 오브젝트 검색
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 20.0f, Vector3.forward, 0f, layer);

        if (hits.Length == 0) return;

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].transform.gameObject.GetComponent<CharacterBase>().onHit(transform, 0, 1.0f);
        }

    }

}
