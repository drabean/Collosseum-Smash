using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressCanvas : MonoBehaviour
{
    [SerializeField] Image timerImage;
    [SerializeField] Image progressBar;

    public void SetTimer(float total, float cur)
    {
        timerImage.fillAmount = cur / total;
    }

    public void SetProgress(float total, float cur)
    {
        progressBar.fillAmount = cur / total;
    }

    public void TimerAlert()
    {

    }

}
