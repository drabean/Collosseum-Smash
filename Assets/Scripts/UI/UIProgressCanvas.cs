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

    public void HideAll()
    {
        GroupStageStart.SetActive(false);
        if(GroupNormal.activeInHierarchy) GroupNormal.GetComponent<ModuleUI>().FadeOut(0.5f);
        if(GroupBoss.activeInHierarchy) GroupBoss.GetComponent<ModuleUI>().FadeOut(0.5f);
    }

    public void ShowStageStart()
    {
        GroupStageStart.SetActive(true);
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

    public void SetBossHP(float  curHp, float maxHP) 
    {
        if (!GroupBoss.activeInHierarchy) ShowBossUI();
        BossHPBar.fillAmount = (float) curHp / maxHP; 
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
