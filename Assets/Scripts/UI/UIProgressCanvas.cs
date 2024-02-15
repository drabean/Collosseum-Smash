using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIProgressCanvas : MonoBehaviour
{
    [SerializeField] GameObject GroupStageStart;
    [SerializeField] GameObject GroupBoss;
    [SerializeField] GameObject GroupNormal;
    [SerializeField] GameObject GroupDemo;
    [SerializeField] GameObject GroupDie;

    [SerializeField] Image EnemyProgressBar;
    [SerializeField] Image BossHPBar;
    [SerializeField] TextMeshProUGUI TMPProgress;

    public void HideAll()
    {
        GroupStageStart.SetActive(false);
        if(GroupNormal.activeInHierarchy) GroupNormal.GetComponent<ModuleUI>().FadeOut(0.5f);
        if(GroupBoss.activeInHierarchy) GroupBoss.GetComponent<ModuleUI>().FadeOut(0.5f);
    }

    public void ShowStageStart(string text = "Fight!")
    {
        GroupStageStart.SetActive(true);
        TMPProgress.text = text;
        SoundMgr.Inst.Play("StageStart");
    }
    public void SetProgress(int curCount, int maxCount) 
    {
        if (!GroupNormal.activeInHierarchy) ShowNormalUI();
        EnemyProgressBar.fillAmount =  (float) curCount / maxCount; 
    }
    public void ShowNormalUI()
    {
        GroupNormal.GetComponent<ModuleUI>().FadeIn(0.5f);
    }
    public float bossCurHP;
    public float bossMaxHP;

    public void SetBossHP(float  curHP, float maxHP) 
    {
        bossCurHP = curHP;
        bossMaxHP = maxHP;
        if (!GroupBoss.activeInHierarchy) ShowBossUI();
        BossHPBar.fillAmount = (float) curHP / maxHP; 
    }
    public void ShowBossUI()
    {
        GroupBoss.GetComponent<ModuleUI>().FadeIn(0.5f);
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
