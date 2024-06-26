using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDefeatedCanvas : MonoBehaviour
{
    public GameObject totalPanel;

    public RectTransform ItemHolder;
    public UIEquipHolder EquipHolderPrefab;

    public TextMeshProUGUI TMP_Title;
    public TextMeshProUGUI TMP_Name;
    public TextMeshProUGUI TMP_DeathMessage;
    public TextMeshProUGUI TMP_Coin;

    public Image CharacterDeathImage;
    public Image ShadowImage;


    int coin;

    [SerializeField] Sprite TrophyImage;
    [SerializeField] Sprite HardTrophyImage;
    /// <summary>
    /// 게임오버 / 게임 클리어 시 보여주는 패널.
    /// </summary>
    public void OpenDefeatPanel()
    {
        SoundMgr.Inst.Play("StageStart");
        totalPanel.SetActive(true);
    }

    public void Init(StageInfo stage, RunData data)
    {
        CharacterInfo character = LoadedData.Inst.getCharacterInfoByID(data.characterInfoIdx);

        TMP_Name.text = character.characterName;
        CharacterDeathImage.sprite = character.DeathSprite;

        TMP_DeathMessage.text = stage.DeathMessage;

        TMP_Coin.text = data.totalCoinCount.ToString();
        coin = data.totalCoinCount;
        for (int i = 0; i < data.item.Count; i++)
        {
            Equip curEquip = LoadedData.Inst.getEquipByID(data.item[i]);
            UIEquipHolder temp = Instantiate(EquipHolderPrefab);
            temp.SetEquip(curEquip);
            temp.transform.SetParent(ItemHolder.transform, false);
        }

    }

    public void InitClear(RunData data, bool isHardMode)
    {
        CharacterInfo character = LoadedData.Inst.getCharacterInfoByID(data.characterInfoIdx);
        TMP_Name.text = character.characterName;
        if (!isHardMode) CharacterDeathImage.sprite = TrophyImage;
        else CharacterDeathImage.sprite = HardTrophyImage;
        Destroy(ShadowImage);
        TMP_DeathMessage.text = "You have stood at the top of the Colosseum! \nHowever, new challenge await...";
        TMP_Title.text = "CLEAR";

        TMP_Coin.text = data.totalCoinCount.ToString();
        coin = data.totalCoinCount;
        for (int i = 0; i < data.item.Count; i++)
        {
            Equip curEquip = LoadedData.Inst.getEquipByID(data.item[i]);
            UIEquipHolder temp = Instantiate(EquipHolderPrefab);
            temp.SetEquip(curEquip);
            temp.transform.SetParent(ItemHolder.transform, false);
        }


    }
    public void Btn_AdBtn()
    {
        AdMgr.Inst.SetInterstitialAdEvent(rewardAction);
        //광고 확인후, 시청시에만 적용
        AdMgr.Inst.ShowInterstialAd();
    }


    public void rewardAction()
    {
        //TODO: 광고 연동 후 시청 여부 체크
        //광고시청
        SoundMgr.Inst.Play("Purchase");
        LoadedSave.Inst.save.Coin += coin;
        LoadedSave.Inst.SyncSaveData();


        UTILS.DeleteRunData();
        LoadSceneMgr.LoadSceneAsync("Start");
    }
    public void Btn_GoToTitle()
    {
        UTILS.DeleteRunData();
        LoadSceneMgr.LoadSceneAsync("Start");
    }
}
