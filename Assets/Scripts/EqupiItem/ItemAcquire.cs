using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemAcquire : MonoBehaviour
{
    [SerializeField]ItemEquipHolder owner;

    public void GetItem()
    {
        owner.onAcquire?.Invoke(owner.index);
        SoundMgr.Inst.Play("GetItem");
        GameMgr.Inst.curRunData.item.Add(owner.equip.ID);
        Equip curEq = Instantiate<Equip>(owner.equip);

        curEq.onEquip(GameObject.FindObjectOfType<Player>());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetItem();
    }
}
