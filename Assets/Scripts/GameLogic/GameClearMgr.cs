using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameClearMgr : MonoBehaviour
{
    Item ClearTrophy;
    Animator TrophyHolder;

    TextMeshPro progressTMP;

    public void Init( RunData runData,TextMeshPro progressTMP)
    {
        ClearTrophy = Resources.Load<Item>("Prefabs/Trophy/ClearTrophy");
        this.progressTMP = progressTMP;

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
        yield return new WaitForSeconds(3.0f);
        progressTMP.text = "You bravely defeated \ncountless foes.";
        yield return new WaitForSeconds(3.0f);
        progressTMP.text = "Everyone will \nremember your  valor!";
        yield return new WaitForSeconds(3.0f);
        progressTMP.text = "However, there seems \nto be another challenge \n still awaits...";
        yield return new WaitForSeconds(3.0f);
        progressTMP.text = "Receive the trophy, \nglorious champion!";
        TrophyHolder.SetTrigger("Show");
        yield return new WaitForSeconds(1.0f);
        SpawnTrophy();

    }
    public IEnumerator ShowClearPanel()
    {
        yield return new WaitForSeconds(3.0f);
        UIMgr.Inst.defeated.InitClear(UTILS.GetRunData());
        UIMgr.Inst.defeated.OpenDefeatPanel();
    }
    public void GameClear()
    {
        // rundata 삭제 및 업적 클리어하기
        LoadedSave.Inst.save.ClearAchivement(ACHIEVEMENT.NORMALCLEAR);
        StartCoroutine(ShowClearPanel());
    }
}
