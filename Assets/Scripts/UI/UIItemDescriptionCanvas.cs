using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIItemDescriptionCanvas : MonoBehaviour
{
    public GameObject GroupDescription;
    public TextMeshPro TMPName;
    public TextMeshPro TMPDescription;

    public void ShowDescription(string Name, string Description)
    {
        GroupDescription.SetActive(true);
        TMPName.text = Name;
        TMPDescription.text = Description;
    }
    public void HideDescription()
    {
        GroupDescription.SetActive(false);

    }
}

