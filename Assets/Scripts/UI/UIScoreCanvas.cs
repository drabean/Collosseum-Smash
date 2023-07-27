using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIScoreCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TMP_Score;

    public void Set(int score)
    {
        TMP_Score.text = (score.ToString());
    }
}
