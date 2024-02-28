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

    #region ĳ���� �̸����� ����
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

        LockGroup.SetActive(false);  // for Test. remove second when build

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
    /// ȭ�鿡 ǥ�õǴ� ĳ���� ��ȯ (index ��ȯ)
    /// ��ư�� ���� ȣ��
    /// </summary>
    /// <param name="isRight">true�Ͻ� idx++, false�Ͻ� idx--</param>
    public void Btn_ChangeCharacter(bool isRight)
    {
        if (isRight) curIdx++;
        else curIdx--;

        curIdx += LoadedData.Inst.characterInfosCount;
        curIdx %= LoadedData.Inst.characterInfosCount;
        Show();
    }
    /// <summary>
    /// ���ο� �����͸� ���� �� ���� ����
    /// ��ư�� ���� ȣ��
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

        resetCharacterView();
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
