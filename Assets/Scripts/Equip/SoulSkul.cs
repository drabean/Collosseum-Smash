using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSkul : Equip
{
    public TargetedProjecitile lichSoul;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.actionSmash += SoulRelease;
    }
    public override void onUnEquip(Player player)
    {
        owner.actionSmash -= SoulRelease;
    }
    void SoulRelease()
    {
        CharacterBase target = FindTarget();
        if (target == null) return;
        Debug.Log(target.gameObject.name);
        TargetedProjecitile tp = Instantiate(lichSoul);
        tp.Shoot(owner.transform.position, this.gameObject);
        tp.Target = target.gameObject;

        tp.onTouch +=  () => hitTarget(target);
    }
    void hitTarget(CharacterBase target)
    {
        target.onHit(owner.transform, 2);
    }
   public  LayerMask layer;
    CharacterBase FindTarget()
    {
        //CircleCast를 통해 주변 모든 Enemy Layer 오브젝트 검색
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 5.0f, Vector3.forward, 0f, layer);


        if (hits.Length == 0) return null;

        RaycastHit2D target = hits[0];
        float minLength = int.MinValue;
        target = hits[0];

        minLength = Vector3.Distance(transform.position, target.transform.position);


        for (int i = 1; i < hits.Length; i++)
        {

            if (minLength < Vector3.Distance(transform.position, hits[i].transform.position))
            {
                target = hits[i];
                minLength = Vector3.Distance(transform.position, hits[i].transform.position);
            }
        }
      

        if (target.transform.TryGetComponent<CharacterBase>(out CharacterBase cb))
        {
            return cb;
        }
        else
        {
            return null;
        }

    }
}
