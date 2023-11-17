using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipHolder : MonoBehaviour
{
    Equip equip;

    [SerializeField] Image itemImage;
    public  string description;

    public void SetEquip(Equip equip)
    {
        this.equip = equip;
        itemImage.sprite = equip.ItemSprite;
        description = equip.Description;
    }
    public void ShowDescription()
    {

    }

    public void HideDescription()
    {

    }
}
