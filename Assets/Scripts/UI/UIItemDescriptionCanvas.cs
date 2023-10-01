using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIItemDescriptionCanvas : MonoBehaviour
{
    public GameObject GroupDescription;
    public TextMeshProUGUI TMPDescription;

    public void ShowDescription(string Description)
    {
        GroupDescription.SetActive(true);
        TMPDescription.text = Description;
    }
    public void HideDescription()
    {
        GroupDescription.SetActive(false);

    }
}
