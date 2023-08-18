using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equip : MonoBehaviour
{
    public  Player owner;

    public virtual void onEquip(Player player)
    {
        owner = player;
        transform.parent = owner.transform;
    }

    public abstract void onUnEquip(Player player);

#if UNITY_EDITOR
    private void Awake()
    {
        onEquip(owner);
    }
#endif

}
