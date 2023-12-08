using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStaff : Equip
{
    GameObject crystal;

    public GameObject CrystalPrefab;
    public Attack StaffProjectile;
    public float ShootTime;
    float shootTimeLeft;

    public LayerMask layer;
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        crystal = Instantiate(CrystalPrefab, this.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        crystal.transform.parent = player.transform;

        player.onAttack += onAttack;
        StartCoroutine(co_ShootMagicMissile());
    }
    public override void onUnEquip(Player player)
    {
        Destroy(crystal);
    }

    IEnumerator co_ShootMagicMissile()
    {
        shootTimeLeft = ShootTime;
        while(true)
        {
            shootTimeLeft -= Time.deltaTime;

            if(shootTimeLeft <= 0)
            {
                shootTimeLeft = ShootTime;

                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 6.0f, Vector3.forward, 0f, layer);

                if (hits.Length == 0) continue;

                Instantiate(StaffProjectile).Shoot(crystal.transform.position, hits[Random.Range(0, hits.Length)].transform.position);

            }
            yield return null;
        }
    }

    
    void onAttack()
    {
        shootTimeLeft -= 3.0f;
    }
}
