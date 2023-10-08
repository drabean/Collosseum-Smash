using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// �� ��� �����۵��� ȹ�� �� �� �ֵ���, ȭ�� �� ������Ʈȭ ��ų �� �ְ� �ϴ� ��ũ��Ʈ.
/// </summary>
public class ItemEquipHolder : MonoBehaviour
{
    public Equip equip;
    /// <summary>
    /// �� �������� ���������� ��ȯ�� ������ �� ����� index�� ���������� �����ϴ� ����
    /// </summary>
    public int index;

    [SerializeField] SpriteRenderer sp;

    public Action<int> onAcquire;
    /// <summary>
    /// �ӽ÷� Start���� ó��
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
