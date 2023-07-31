using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ModuleHit : MonoBehaviour
{
    SpriteRenderer[] sps;

    GameObject[] hitEffect = new GameObject[2];

    TextMeshPro _dmgTxt;

    public void Awake()
    {
        sps = GetComponentsInChildren<SpriteRenderer>();
        originMat = sps[0].material;
        whiteMat = Resources.Load<Material>("Materials/FlashWhite");
        hitEffect[0] = Resources.Load<GameObject>("Prefabs/HitEffect");
        hitEffect[1] = Resources.Load<GameObject>("Prefabs/HitEffect2");
        _dmgTxt = Resources.Load<TextMeshPro>("Prefabs/DmgTxt");

    }

    #region HitEffect

    public void HitEffect(Vector3 hitVec)
    {
        Instantiate(hitEffect[0], transform.position , (hitVec * (-1)).ToQuaternion()).transform.localScale = Vector3.one * Random.Range(1.7f, 2.6f);
        Instantiate(hitEffect[1], transform.position, (hitVec * (-1)).ToQuaternion()).transform.localScale = Vector3.one * Random.Range(1.7f, 2.6f);
    }
    #endregion

    #region DmgTxt
    public void DmgTxt(int combo)
    {
        TextMeshPro dmgTxt = Instantiate(_dmgTxt, transform.position + Vector3.up * 0.8f, Quaternion.identity);
        dmgTxt.text = combo + "combo!";
        Destroy(dmgTxt, 0.2f);

    }
    #endregion

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
