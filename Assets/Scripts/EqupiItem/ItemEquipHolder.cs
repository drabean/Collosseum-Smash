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
    /// <summary>
    /// 이 아이템이 스테이지에 소환된 아이템 중 몇번쨰 index의 아이템인지 저장하는 변수
    /// </summary>
    public int index;

    [SerializeField] SpriteRenderer sp;

    public Action<int> onAcquire;
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
