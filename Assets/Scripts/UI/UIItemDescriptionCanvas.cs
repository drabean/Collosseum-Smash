using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemDescriptionCanvas : MonoSingleton<UIItemDescriptionCanvas>
{
    public GameObject GroupDescription;
    public TextMeshPro TMPName;
    public TextMeshPro TMPDescription;

    public void ShowDescription(Equip equip)
    {
        GroupDescription.SetActive(true);
        TMPName.text = equip.itemName;
        TMPDescription.text = equip.Description;
    }
    public void HideDescription()
    {
        GroupDescription.SetActive(false);
    }
}

