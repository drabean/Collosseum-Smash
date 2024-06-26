using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionArea : MonoBehaviour
{
    [SerializeField] ItemEquipHolder owner;

    public void ShowDescription()
    {
        UIMgr.Inst.itemDescription.ShowDescription(owner.equip);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) ShowDescription();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) UIMgr.Inst.itemDescription.HideDescription();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) ShowDescription();
    }
}
