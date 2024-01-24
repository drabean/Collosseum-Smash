using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipHolder : MonoBehaviour
{
    Equip equip;

    [SerializeField] Image itemImage;

    public void SetEquip(Equip equip)
    {
        this.equip = equip;
        itemImage.sprite = equip.ItemSprite;
    }
    public void ShowItemDescription()
    {
        Debug.Log("SHOWITEM");
        UIItemDescriptionPanel.ShowEquip(equip);
    }
}
