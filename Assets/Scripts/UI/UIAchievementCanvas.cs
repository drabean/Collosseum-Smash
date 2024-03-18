using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAchievementCanvas : MonoBehaviour
{
    public GameObject totalPanel;
    [SerializeField] UIAchievementPrefab AchivementPrefab;
    [SerializeField] Transform AchivementHolder;
    bool isShoptInit = false;

    public void OpenPanel()
    {
        totalPanel.SetActive(true);
        initAchivementPanel();
    }

    void initAchivementPanel()
    {
        if (isShoptInit) return;
        //TODO: 광고 추가하기

        isShoptInit = true;

        AddAchievementBlock(ACHIEVEMENT.TUTORIALCLEAR);
        AddAchievementBlock(ACHIEVEMENT.NORMALCLEAR);
        AddAchievementBlock(ACHIEVEMENT.HARDCLEAR);

        AddAchievementBlock(ACHIEVEMENT.SMASHBOSS1);
        AddAchievementBlock(ACHIEVEMENT.SMASHBOSS2);
        AddAchievementBlock(ACHIEVEMENT.SMASHNORMAL1);
        AddAchievementBlock(ACHIEVEMENT.SMASHNORMAL2);

        AddAchievementBlock(ACHIEVEMENT.FIRSTRETRY);

        #region 보스킬
        AddAchievementBlock(ACHIEVEMENT.KILLTUTORIAL);
        AddAchievementBlock(ACHIEVEMENT.KILLHAIRBALL);
        AddAchievementBlock(ACHIEVEMENT.KILLGOBLIN);
        AddAchievementBlock(ACHIEVEMENT.KILLMUSHROOM);
        AddAchievementBlock(ACHIEVEMENT.KILLCHAMPION);
        AddAchievementBlock(ACHIEVEMENT.KILLSLIME);
        AddAchievementBlock(ACHIEVEMENT.KILLENT);
        AddAchievementBlock(ACHIEVEMENT.KILLLICH);
        AddAchievementBlock(ACHIEVEMENT.KILLBLOCK);
        #endregion

    }
    public void AddAchievementBlock(ACHIEVEMENT Achievement)
    {
        Instantiate(AchivementPrefab, AchivementHolder).Init(LoadedData.Inst.getAchivementInfo(Achievement));
    }

    public void Btn_Exit()
    {
        totalPanel.SetActive(false);
    }

}
