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

    [SerializeField]  TextMeshProUGUI enemyLeft;
    [SerializeField] Image HPBar;

    public void HideAll()
    {
        GroupNormal.SetActive(false);
        GroupBoss.SetActive(false);
    }
    public void setEnemyLeft(int num) 
    {
        if (!GroupNormal.activeInHierarchy) ShowNormalUI();
        enemyLeft.text = num.ToString(); 
    }
    public void ShowNormalUI()
    {
        GroupBoss.SetActive(false);
        GroupNormal.SetActive(true);
    }

    public void SetBossHP(float  curHp, float maxHP) 
    {
        if (!GroupBoss.activeInHierarchy) ShowBossUI();
        HPBar.fillAmount = (float) curHp / maxHP; 
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
