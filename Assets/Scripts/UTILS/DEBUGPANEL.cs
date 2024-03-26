using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DEBUGPANEL : MonoBehaviour
{
    [SerializeField] Slider SliderStage;
    [SerializeField] Slider SliderCharacter;

    [SerializeField] TextMeshProUGUI stage;
    [SerializeField] TextMeshProUGUI character;

    int curStageIdx;
    int curCharacterIdx;
    bool startFromBoss = false;

    CharacterInfo info;

    private void OnEnable()
    {
        SliderStage.onValueChanged.AddListener(changeStage);
        SliderCharacter.onValueChanged.AddListener(changeCharacter);
    }


    void changeStage(float value)
    {
        curStageIdx =(int)( value * 2);
        stage.text = curStageIdx.ToString();
    }
    
    void changeCharacter(float value)
    {
        curCharacterIdx = (int)(value * 2);
        character.text = (curCharacterIdx).ToString();
    }

    public void TogleStartFromBoss(bool value)
    {
        startFromBoss = value;
    }

    public void GameStart()
    {
        RunData data = new RunData(curCharacterIdx, new List<int>());
        info = LoadedData.Inst.getCharacterInfoByID(curCharacterIdx);

        for (int i = 0; i < info.playerItems.Count; i++)
        {
            data.item.Add(info.playerItems[i].ID);
        }
        data.curHP = 10;
        data.reviveCount = 1;
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.REVIVE)) data.reviveCount++;

        switch(curStageIdx)
        {
            case 0:
                data.curStageID = 31;
                data.curDifficulty = DIFFICULTY.EASY;
                break;
            case 1:
                data.curStageID = 22;
                data.curDifficulty = DIFFICULTY.NORMAL;
                break;
            case 2:
                data.curStageID = 100;
                data.curDifficulty = DIFFICULTY.FINALE;
                break;
        }


        ItemMgr.Inst.InitNormalEquipPool(data);

        for(int i = 0; i < curStageIdx; i++)
        {
            data.item.Add(ItemMgr.Inst.GetNormalEquip().ID);
        }
        data.isTutorial = false;
        data.isBoss = startFromBoss;
        Debug.Log("STAGESTART!"+curCharacterIdx+"+"+ curStageIdx);

        //하드모드 열려있으면 강제로 하드모드 진입
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.NORMALCLEAR)) data.isHardMode = true;
        UTILS.SaveRunData(data);

        LoadSceneMgr.LoadSceneAsync("Main");
    }

    public void PanelClose()
    {
        gameObject.SetActive(false);
    }

    public void Btn_MoneyCheat()
    {
        LoadedSave.Inst.save.Coin += 200;
        LoadedSave.Inst.SyncSaveData();
    }

    public void Btn_RemoveAllData()
    {
        UTILS.DeleteRunData();
        UTILS.DeleteSaveData();
        UTILS.DeleteSettingData();
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void Btn_EnableHardmode()
    {
        LoadedSave.Inst.save.ClearAchivement(ACHIEVEMENT.NORMALCLEAR);
        LoadedSave.Inst.save.NormalKill += 100;
        LoadedSave.Inst.save.BossKill += 100;
        LoadedSave.Inst.SyncSaveData();
    }
}
