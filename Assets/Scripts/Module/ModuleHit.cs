using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleHit : MonoBehaviour
{
    SpriteRenderer[] sps;

    GameObject HitEffect;

    public void Awake()
    {
        sps = GetComponentsInChildren<SpriteRenderer>();
        originMat = sps[0].material;
        whiteMat = Resources.Load<Material>("Materials/FlashWhite");
    }
    #region WhiteFlash
    Material originMat;
    Material whiteMat;

    public void FlashWhite(float time)
    {
        StartCoroutine(co_FlashWhite(time))
;    }

    IEnumerator co_FlashWhite(float time)
    {
        for(int i = 0; i < sps.Length; i++)
        {
            sps[i].material = whiteMat;
        }

        yield return new WaitForSeconds(time);

        for (int i = 0; i < sps.Length; i++)
        {
            sps[i].material = originMat;
        }
    }
    #endregion

    #region Togle
    static float togleTime = 0.1f; // 깜박거리는주기
    static Color togleColor = new Color(1, 1, 1, 0.4f);

    public void Togle(float time)
    {
        StartCoroutine(co_Togle(time));
    }
    IEnumerator co_Togle(float time)
    {
        WaitForSeconds waitTogle = new WaitForSeconds(togleTime);

        float timeStart = Time.time;
        while(true)
        {
            for (int i = 0; i < sps.Length; i++)
            {
                sps[i].color = togleColor;
            }
            yield return waitTogle;
            for (int i = 0; i < sps.Length; i++)
            {
                sps[i].color = Color.white;
            }
            yield return waitTogle;

            if (Time.time - timeStart >= time) break;
        }

        for(int i = 0; i < sps.Length; i++)
        {
            sps[i].color = Color.white;
        }

    }
    #endregion
}
