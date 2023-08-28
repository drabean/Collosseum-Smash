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


    private void Start()
    {
        owner = GameObject.FindObjectOfType<Player>();
        onEquip(owner);
    }


}
