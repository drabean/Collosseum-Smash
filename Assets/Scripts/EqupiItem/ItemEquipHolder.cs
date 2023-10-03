using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 각 장비 아이템들을 획득 할 수 있도록, 화면 상에 오브젝트화 시킬 수 있게 하는 스크립트.
/// </summary>
public class ItemEquipHolder : MonoBehaviour
{
    public Equip equip;

    [SerializeField] SpriteRenderer sp;

    public Action onAcquire;
    /// <summary>
    /// 임시로 Start에서 처리
    /// </summary>
    public void Start()
    {
        SetItem(equip);
    }
    public void SetItem(Equip equip)
    {
        this.equip = equip;

        sp.sprite = equip.ItemSprite;
    }
}
