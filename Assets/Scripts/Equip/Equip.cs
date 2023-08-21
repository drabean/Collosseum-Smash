using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equip : MonoBehaviour
{
    public  Player owner;

    public abstract void onEquip(Player player);

    public abstract void onUnEquip(Player player);

#if UNITY_EDITOR
    private void Awake()
    {
        onEquip(owner);
    }
#endif
}
