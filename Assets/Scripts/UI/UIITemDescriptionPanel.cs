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

    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI TMP_Name;
    [SerializeField] TextMeshProUGUI TMP_Description;


    public static void ShowEquip(Equip equip)
    {
        UIItemDescriptionPanel ui = GameObject.FindObjectOfType<UIItemDescriptionPanel>();
        if(ui == null) ui = Instantiate(Resources.Load<UIItemDescriptionPanel>("Prefabs/UI/ItemDescriptionPanel")); // 없다면 새로 생성

        ui.transform.SetParent(GameObject.Find("MainCanvas").transform, false);
        ui.ShowPanel(equip);
    }
    public void ShowPanel(Equip equip)
    {
        gameObject.SetActive(true);
        Debug.Log(equip);
        Debug.Log(equip.ItemSprite);
        Debug.Log(itemImage);
        itemImage.sprite = equip.ItemSprite;
        TMP_Name.text = equip.itemName;
        TMP_Description.text = equip.Description;

    }

    public void Btn_ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
