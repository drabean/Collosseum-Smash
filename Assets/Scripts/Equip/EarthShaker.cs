using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthShaker : Equip
{
    public int stack;
    public Attack ShockWave;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.onAttack += onAttack;
        owner.onMovement += EarthShake;
    }
    public override void onUnEquip(Player player)
    {
        owner.onAttack -= onAttack;
        owner.onMovement -= EarthShake;
    }

    void onAttack()
    {
        if(stack < 5)stack+=2;
    }
    void EarthShake()
    {
        if(stack > 0)
        {
            stack--;
            Attack shockWave = Instantiate(ShockWave);
            shockWave.Shoot(transform.position, transform.position);
            shockWave.gameObject.layer = LayerMask.NameToLayer("AllyAttack");
        }
    }
}
