using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIProgressCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_difficulty;
    [SerializeField] Animator panelMovement;

    public void showDifficulty(int difficulty)
    {
        tmp_difficulty.text = difficulty.ToString();
        panelMovement.SetTrigger("Show");
    }

}
