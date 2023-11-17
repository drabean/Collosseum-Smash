using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICharacterSelectCanvas : MonoSingleton<UICharacterSelectCanvas>
{
    [SerializeField] GameObject GroupCharacterSelect;

    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] UIStatIcon[] stats;
    [SerializeField] RectTransform EquipHolder;
    [SerializeField] UIEquipHolder[] equips;
    public CharacterInfo info;

    GameObject equipHolderPrefab;

    int curIdx = 0;

    private void Awake()
    {
        equipHolderPrefab = Resources.Load<GameObject>("Prefabs/UI/EquipHolder");
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
        resetCharacterView();

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

        StartCoroutine(co_SpawnNextBot());
    }

    WaitForSeconds botSpawnWait = new WaitForSeconds(3.0f);


    void onBotKill(Vector3 hitPos)
    {
        curSpawnRoutine = StartCoroutine(co_SpawnNextBot());
    }

    IEnumerator co_SpawnNextBot()
    {
        yield return botSpawnWait;
        botSpawnIdx++;
        botSpawnIdx %= botSpawnPos.Count;

        curDoll = Instantiate(botPrefab, botSpawnPos[botSpawnIdx].position, Quaternion.identity);
        curDoll.onDeath += onBotKill;
    }
    void Show()
    {
        info = LoadedData.Inst.characterInfos[curIdx];


        changePlayer();
        characterName.text = info.characterName;
        description.text = info.description;
        stats[0].SetStat(info.stat.STR);
        stats[1].SetStat(info.stat.VIT);
        stats[2].SetStat(info.stat.SPD);

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
        curIdx++;
        curIdx %= LoadedData.Inst.characterInfos.Length;
        Show();
    }
    #endregion

}
