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

        if(LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.TUTORIALCLEAR))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("First Step", "Cleared Totorial.", "10 Coin");
        }

        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.NORMALCLEAR))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("Champion", "Beat the game in normal mode.", "Hard Mode\nUnlock");
        }      
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.HARDCLEAR))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("True Champion", "Beat the game in hard mode.", "Amazing!");
        }

        #region 보스킬
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.KILLTUTORIAL))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("Novice Gladiator", "Smashed Huge Training Doll.", "20 Coin");
        }
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.KILLHAIRBALL))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("Beast Hunt", "Smashed Huge Hairball.", "20 Coin");
        }
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.KILLGOBLIN))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("Goblin Slayer", "Smashed Goblin King.", "20 Coin");
        }
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.KILLCHAMPION))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("Veteran Gladiator", "Smashed Champion.", "20 Coin");
        }
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.KILLSLIME))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("Slimy Hero", "Samshed Huge Slime.", "20 Coin");
        }
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.KILLLICH))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("Exorcism", "Smashed Lich.", "20 Coin");
        }
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.KILLBLOCK))
        {
            UIAchievementPrefab prefab = Instantiate(AchivementPrefab, AchivementHolder);
            prefab.Init("Indiana Jones", "Smashed Ancient Machine.", "20 Coin");
        }

        #endregion

    }

    public void Btn_Exit()
    {
        totalPanel.SetActive(false);
    }

}
