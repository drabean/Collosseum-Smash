using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameClearMgr : MonoBehaviour
{
    Item ClearTrophy;
    Animator TrophyHolder;

    TextMeshPro progressTMP;

    bool isHardMode;
    public void Init(RunData runData,TextMeshPro progressTMP)
    {
        isHardMode = runData.isHardMode;

        if(!isHardMode)ClearTrophy = Resources.Load<Item>("Prefabs/Trophy/ClearTrophyItem");
        else ClearTrophy = Resources.Load<Item>("Prefabs/Trophy/HardmodeTrophyItem");
        this.progressTMP = progressTMP;
        this.isHardMode = runData.isHardMode;

        TrophyHolder = Instantiate(Resources.Load<Animator>("Prefabs/TrophyHolder"));
        TrophyHolder.transform.position = Vector3.up * 0.5f;
    }

    public void SpawnTrophy()
    {
        Instantiate(ClearTrophy, Vector3.up * 0.8f, Quaternion.identity).onAcquire += GameClear;
    }

    public void StartClearRoutine()
    {
        StartCoroutine(co_ClearRoutine());
    }

    IEnumerator co_ClearRoutine()
    {
        progressTMP.text = "";
        yield return new WaitForSeconds(2.0f);
        SoundMgr.Inst.PlayBGM("WIN");
        if (!isHardMode) progressTMP.text = "You have smashed every enemy!";
        else progressTMP.text = "You have smashd countless, \nevil Things! ";
        yield return new WaitForSeconds(3.0f);
        if(!isHardMode)progressTMP.text = "Receive the trophy, \nGlorious champion!";
        else progressTMP.text = "You are a true \nchampion of colloseum!";
        yield return new WaitForSeconds(1.0f);
        TrophyHolder.SetTrigger("Show");
        yield return new WaitForSeconds(1.0f);
        SpawnTrophy();

    }
    public IEnumerator ShowClearPanel()
    {
        yield return new WaitForSeconds(3.0f);
        UIMgr.Inst.defeated.InitClear(UTILS.GetRunData(), isHardMode);
        UIMgr.Inst.defeated.OpenDefeatPanel();
    }
    public void GameClear()
    {
        // rundata 삭제 및 업적 클리어하기
        if (!isHardMode)
        {
            LoadedSave.Inst.TryAddAchievement(ACHIEVEMENT.NORMALCLEAR);
        }
        else
        {
            LoadedSave.Inst.TryAddAchievement(ACHIEVEMENT.HARDCLEAR);
        }
        LoadedSave.Inst.SyncSaveData();

        StartCoroutine(ShowClearPanel());
    }
}
