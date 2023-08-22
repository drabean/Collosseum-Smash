using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaSword : Equip
{
    public LayerMask layer;
    public int reqCombo;
    Attack daggerPrefab;
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.onCombo += throwDagger;
        daggerPrefab = Resources.Load<Attack>("Prefabs/Attack/NinjaDagger");
    }

    public override void onUnEquip(Player player)
    {
        owner.onCombo -= throwDagger;
    }

    void throwDagger(int combo)
    {
        if (combo % reqCombo == 0 && combo > 0)
        {
            //CircleCast�� ���� �ֺ� ��� Enemy Layer ������Ʈ �˻�
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 4.0f, Vector3.forward, 0f, layer);

            if (hits.Length == 0) return;

            int targetCount = hits.Length < 3 ? hits.Length : 3;
            
            for (int i = 0; i < targetCount; i++)
            {

                Attack dagger = Instantiate<Attack>(daggerPrefab);
                dagger.Shoot(transform.position, hits[i].transform.position);
            }
        }
    }
}
