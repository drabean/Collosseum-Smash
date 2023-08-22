using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigHammer : Equip
{
    public int reqCombo;
    Attack projectile;
    public LayerMask layer;
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.onCombo += smash;
        projectile = Resources.Load<Attack>("Prefabs/Attack/HolySlash");
    }

    public override void onUnEquip(Player player)
    {
        owner.onCombo -= smash;
    }

    void smash(int combo)
    {
        if (combo % reqCombo == 0 && combo > 0)
        {
            GameMgr.Inst.Shake(0.7f, 9f, 0.2f, 0f, true);
            //CircleCast를 통해 주변 모든 Enemy Layer 오브젝트 검색
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 20.0f, Vector3.forward, 0f, layer);

            if (hits.Length == 0) return;

            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].transform.gameObject.GetComponent<CharacterBase>().Stun(transform);
            }
        }
    }

}
