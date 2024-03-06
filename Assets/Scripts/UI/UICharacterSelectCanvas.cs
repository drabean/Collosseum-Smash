using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICharacterSelectCanvas : MonoBehaviour
{
    [SerializeField] GameObject GroupCharacterSelect;

    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] UIStatIcon[] stats;
    [SerializeField] RectTransform EquipHolder;
    [SerializeField] UIEquipHolder[] equips;
    [SerializeField] Toggle TutorialToggle;
    [SerializeField] GameObject HardToggleGroup;
    [SerializeField] Toggle HardmodeToggle;
    public CharacterInfo info;

    public int curIdx = 0;


    [SerializeField] GameObject LockGroup;
    [SerializeField] TextMeshProUGUI TMPLockProgress;

    public void Init(int saveIdx)
    {
        curIdx = saveIdx;
        info = LoadedData.Inst.getCharacterInfoByID(curIdx);
        changePlayer();
        if (!LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.TUTORIALCLEAR))
        {
            isTutorial = true;
            TutorialToggle.isOn = true;
        }
        else
        {
            isTutorial = false;
            TutorialToggle.isOn = false;
        }



        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.NORMALCLEAR))
        {
            HardmodeToggle.gameObject.SetActive(true);
            HardmodeToggle.isOn = true;
            isHardMode = true;
        }
        else
        {
            HardmodeToggle.gameObject.SetActive(false);
            isHardMode = false;
        }
    }
    public void OpenCharacterSelect(int idx)
    {
        GroupCharacterSelect.SetActive(true);

        curIdx = idx;
        Show();
    }

    public void CloseCharacterSelect()
    {
        //resetCharacterView();

        GroupCharacterSelect.SetActive(false);
    }

    #region 캐릭터 미리보기 관련
    GameObject curPlayer;
    public Transform playerSpawnPos;
    public List<Transform> botSpawnPos;

    public EnemyDoll botPrefab;
    EnemyDoll curDoll;
    int botSpawnIdx = 0;
    Coroutine curSpawnRoutine;

    public void resetCharacterView()
    {
        if (curDoll != null) curDoll.Despawn();
        if (curPlayer != null) Destroy(curPlayer);
        if (curSpawnRoutine != null) StopCoroutine(curSpawnRoutine);
    }

    public void changePlayer()
    {
        resetCharacterView();


        GameObject smoke = DictionaryPool.Inst.Pop("Prefabs/Smoke");
        smoke.transform.position = playerSpawnPos.position;
        smoke.GetComponent<Poolable>().Push(2.0f);
        curPlayer = Instantiate(info.playerPrefab, playerSpawnPos.position, Quaternion.identity).gameObject;

        curSpawnRoutine = StartCoroutine(co_SpawnBotCoroutine());
    }

    WaitForSeconds botSpawnWait = new WaitForSeconds(1.5f);



    IEnumerator co_SpawnBotCoroutine()
    {
        while (true)
        {
            if (curDoll == null)
            {
                yield return botSpawnWait;
                botSpawnIdx++;
                botSpawnIdx %= botSpawnPos.Count;

                curDoll = Instantiate(botPrefab, botSpawnPos[botSpawnIdx].position, Quaternion.identity);
            }
            yield return null;
        }
    }
    void Show()
    {
        info = LoadedData.Inst.getCharacterInfoByID(curIdx);


        switch(curIdx) // 캐릭터별 Unlock 구별
        {
            case 0:
                {
                    LockGroup.SetActive(false);
                    break;
                }
            case 1:
                if(LoadedSave.Inst.save.Exp >= 5)
                {
                    LockGroup.SetActive(false); 
                }
                else
                {
                    LockGroup.SetActive(true);
                    TMPLockProgress.text = "Unlock After Deafeating " + (5 - LoadedSave.Inst.save.Exp) + " Boss!";
                }
                break;
            case 2:
                if (LoadedSave.Inst.save.Exp >= 10)
                {
                    LockGroup.SetActive(false);
                }
                else
                {
                    LockGroup.SetActive(true);
                    TMPLockProgress.text = "Unlock After Deafeating " + (10 - LoadedSave.Inst.save.Exp) + " Boss!";
                }
                break;
        }



        changePlayer();
        characterName.text = info.characterName;
        description.text = info.description;
        stats[0].SetStat(info.playerPrefab.Stat.STR);
        stats[1].SetStat(info.playerPrefab.Stat.VIT);
        stats[2].SetStat(info.playerPrefab.Stat.SPD);

        for (int i = 0; i < 2; i++)
        {
            equips[i].SetEquip(info.playerItems[i]);
        }

    }

    /// <summary>
    /// 화면에 표시되는 캐릭터 변환 (index 순환)
    /// 버튼을 통해 호출
    /// </summary>
    /// <param name="isRight">true일시 idx++, false일시 idx--</param>
    public void Btn_ChangeCharacter(bool isRight)
    {
        if (isRight) curIdx++;
        else curIdx--;

        curIdx += LoadedData.Inst.characterInfosCount;
        curIdx %= LoadedData.Inst.characterInfosCount;
        Show();
    }
    /// <summary>
    /// 새로운 데이터를 생성 후 게임 시작
    /// 버튼을 통해 호출
    /// </summary>
    public void Btn_StartGame()
    {
        RunData data = new RunData(curIdx, new List<int>(), 0);
        info = LoadedData.Inst.getCharacterInfoByID(curIdx);

        for (int i = 0; i < info.playerItems.Count; i++)
        {
            data.item.Add(info.playerItems[i].ID);
        }
        data.curHP =10;
        data.reviveCount = 1;
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.REVIVE)) data.reviveCount++;
        data.isTutorial = this.isTutorial;
        data.isHardMode = this.isHardMode;
        UTILS.SaveRunData(data);

        SoundMgr.Inst.BGMFadeout();

        LoadSceneMgr.LoadSceneAsync("Main");
    }

    bool isTutorial = false;
    bool isHardMode = false;
    public void Toggle_tutorial(bool isTrue)
    {
        isTutorial = isTrue;
    }
    public void Toggle_Hard(bool isTrue)
    {
        isHardMode = isTrue;
    }
    #endregion

}
