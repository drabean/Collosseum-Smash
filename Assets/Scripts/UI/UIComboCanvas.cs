using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIComboCanvas : MonoBehaviour
{
    TextMeshProUGUI comboText;
    public void showCombo(int num)
    {
        comboText.text = num.ToString();
    }
}
