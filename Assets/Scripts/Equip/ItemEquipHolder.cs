using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �� ��� �����۵��� ȹ�� �� �� �ֵ���, ȭ�� �� ������Ʈȭ ��ų �� �ְ� �ϴ� ��ũ��Ʈ.
/// </summary>
public class ItemEquipHolder : MonoBehaviour
{
    public Equip equip;

    [SerializeField] SpriteRenderer sp;
    /// <summary>
    /// �ӽ÷� Start���� ó��
    /// </summary>
    public void Start()
    {
        SetItem(equip);
    }
    public void SetItem(Equip equip)
    {
        sp.sprite = equip.ItemSprite;
    }
    public void ShowDescription()
    {
        UIMgr.Inst.itemDescription.ShowDescription(equip.Description);
    }
    public void GetItem()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) ShowDescription();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) UIMgr.Inst.itemDescription.HideDescription();
    }
}
