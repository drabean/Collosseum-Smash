using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIProgressCanvas : MonoBehaviour
{
    [SerializeField] GameObject GroupBoss;
    [SerializeField] GameObject GroupNormal;
    [SerializeField] GameObject GroupDemo;
    [SerializeField] GameObject GroupDie;

    [SerializeField] Image EnemyProgressBar;
    [SerializeField] Image BossHPBar;

    public void HideAll()
    {
        GroupNormal.SetActive(false);
        GroupBoss.SetActive(false);
    }
    public void SetProgress(int curCount, int maxCount) 
    {
        if (!GroupNormal.activeInHierarchy) ShowNormalUI();
        EnemyProgressBar.fillAmount =  (float) curCount / maxCount; 
    }
    public void ShowNormalUI()
    {
        GroupBoss.SetActive(false);
        GroupNormal.SetActive(true);
    }

    public void SetBossHP(float  curHp, float maxHP) 
    {
        if (!GroupBoss.activeInHierarchy) ShowBossUI();
        BossHPBar.fillAmount = (float) curHp / maxHP; 
    }
    public void ShowBossUI()
    {
        GroupNormal.SetActive(false);
        GroupBoss.SetActive(true);
    }

    public void Clear()
    {
        HideAll();
        GroupDemo.SetActive(true);
    }

    public void Die()
    {
        HideAll();
        GroupDie.SetActive(true);
    }
}
