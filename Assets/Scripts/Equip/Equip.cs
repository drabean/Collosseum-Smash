using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equip : MonoBehaviour
{
    public int ID;

    public Sprite ItemSprite;
    public string itemName;
    [TextArea]
    public string Description;

    public  Player owner;


    /// <summary>
    /// 아이템 장착 함수. (Base 호출 해야함)
    /// </summary>
    /// <param name="player"></param>
    public virtual void onEquip(Player player)
    {
        owner = player;
        transform.SetParent(owner.transform, false);
    }

    public abstract void onUnEquip(Player player);


}
