using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatIcon : MonoBehaviour
{
    [SerializeField] List<Image> bars;

    [SerializeField] Sprite on;
    [SerializeField] Sprite off;
    public void SetStat(int stat)
    {
        for(int i = 0; i < 4; i++)
        {
            if (i < stat) bars[i].sprite = on;
            else bars[i].sprite = off;
        }
    }
}
