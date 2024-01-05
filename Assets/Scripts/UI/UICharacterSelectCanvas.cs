using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICharacterSelectCanvas : MonoBehaviour
{
    [SerializeField] GameObject GroupCharacterSelect;

    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] UIStatIcon[] stats;
    [SerializeField] RectTransform EquipHolder;
    [SerializeField] UIEquipHolder[] equips;
    public CharacterInfo info;

    public int curIdx = 0;


    [SerializeField] GameObject LockGroup;
    [SerializeField] TextMeshProUGUI TMPLockProgress;

    public void Init(int saveIdx)
    {
        curIdx = saveIdx;
    }
    public void OpenCharacterSelect(int idx)
    {
        //TODO: Save�� ����� ������ �÷��� ĳ���Ͱ� ���̵��� ��
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

        if (curIdx > StartSceneMgr.Inst.saveData.ProgressLV)
        {
            LockGroup.SetActive(true);
        }
        else
        {
            LockGroup.SetActive(false);
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
        RunData data = new RunData(curIdx, 0, new List<int>(), 0);
        info = LoadedData.Inst.getCharacterInfoByID(curIdx);

        for (int i = 0; i < info.playerItems.Count; i++)
        {
            data.item.Add(info.playerItems[i].ID);
        }
        UTILS.SaveRunData(data);

        resetCharacterView();
        LoadSceneMgr.LoadSceneAsync("Main");
    }
    #endregion

}
