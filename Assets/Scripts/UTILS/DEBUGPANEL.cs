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
        curStageIdx =(int)( value * 6);
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
        RunData data = new RunData(curCharacterIdx, new List<int>(), 0);
        info = LoadedData.Inst.getCharacterInfoByID(curCharacterIdx);

        for (int i = 0; i < info.playerItems.Count; i++)
        {
            data.item.Add(info.playerItems[i].ID);
        }
        data.curHP = info.playerPrefab.Stat.VIT + 1;
        data.stageProgress = curStageIdx;

        ItemMgr.Inst.InitNormalEquipPool(data);

        for(int i = 0; i < curStageIdx; i++)
        {
            data.item.Add(ItemMgr.Inst.GetNormalEquip().ID);
        }

        data.isBoss = startFromBoss;
        Debug.Log("STAGESTART!"+curCharacterIdx+"+"+ curStageIdx);
        UTILS.SaveRunData(data);

        LoadSceneMgr.LoadSceneAsync("Main");
    }

    public void PanelClose()
    {
        gameObject.SetActive(false);
    }

    public void Btn_MoneyCheat()
    {
        SaveDatas.Inst.save.Money += 200;
    }
}
