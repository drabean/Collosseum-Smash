using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 아이템을 설명하는 Panel
/// </summary>
public class UIItemDescriptionPanel : MonoBehaviour
{
    public GameObject GroupItemDescription;

    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI TMP_Name;
    [SerializeField] TextMeshProUGUI TMP_Description;
    
    
    public void ShowPanel(Equip equip)
    {
        Debug.Log(equip);
        Debug.Log(equip.ItemSprite);
        Debug.Log(itemImage);
        itemImage.sprite = equip.ItemSprite;
        TMP_Name.text = equip.itemName;
        TMP_Description.text = equip.Description;

    }

    public void Btn_ClosePanel()
    {
        GroupItemDescription.SetActive(false);
    }
}
