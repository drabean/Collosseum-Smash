using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIPauseCanvas : MonoBehaviour
{
    int curPageIdx = 0;
    //0:ItemPage, 1:OptionPage
    public GameObject[] Pages;

    public TextMeshProUGUI TMPTitle;

    public GameObject totalPanel;

    public GameObject BtnGroup;
    public GameObject OptionGroup;
    public GameObject ItemListGroup;

    public void OpenPausePanel()
    {
        Debug.Log("PAUSED");
        TMPTitle.text = "Pause";
        totalPanel.SetActive(true);
        Time.timeScale = 0;
        GameMgr.Inst.isPause = true;
        OpenPage(1);
    }
    public void ClosePausePanel()
    {
        Time.timeScale = 1;

        LoadedSave.Inst.SyncSetting();
        totalPanel.SetActive(false);
        GameMgr.Inst.isPause = false;
    }


    public void Btn_OpenOption()
    {

    }
    public void Btn_Move(bool isRight)
    {
        Pages[curPageIdx].SetActive(false);
        if (isRight) curPageIdx++; else curPageIdx--;
        if(curPageIdx < 0 ) curPageIdx += Pages.Length;
        curPageIdx %= Pages.Length;

        Pages[curPageIdx].SetActive(true);

        switch(curPageIdx)
        {
            case 0:
                OpenItemPage();
                break;
            case 1:
                OpenOptionPage();
                break;
        }
    }
    void OpenPage(int idx)
    {
        Pages[curPageIdx].SetActive(false);
        curPageIdx = idx;
        Pages[idx].SetActive(true);

        switch (idx)
        {
            case 0:
                OpenItemPage();
                break;
            case 1:
                OpenOptionPage();
                break;
        }
    }
    #region 아이템 관련
    public RectTransform ItemHolder;
    public UIEquipHolder EquipHolderPrefab;
    bool isItemInit;

    public void OpenItemPage()
    {
        TMPTitle.text = "Equips";

        if (isItemInit) return;
        isItemInit = true;
        RunData curRunData = GameMgr.Inst.curRunData;

        for(int i = 0; i < curRunData.item.Count; i++)
        {
            Equip curEquip = LoadedData.Inst.getEquipByID (curRunData.item[i]);
            UIEquipHolder temp = Instantiate(EquipHolderPrefab);
            temp.SetEquip(curEquip);
            temp.transform.SetParent(ItemHolder.transform, false);
        }
    }
    #endregion

    #region 옵션 관련
    bool isOptionInit;
    [SerializeField]Slider SliderBGM;
    [SerializeField] TextMeshProUGUI TMPBGMVolume;
    [SerializeField] Slider SliderSFX;
    [SerializeField] TextMeshProUGUI TMPSFXVolume;
    [SerializeField] TextMeshProUGUI TMPBtnExplain;
    public void OpenOptionPage()
    {
        TMPTitle.text = "Options";
        if (isOptionInit) return;
        isOptionInit = true;
        SliderBGM.onValueChanged.AddListener(changeBGMVolume);
        SliderSFX.onValueChanged.AddListener(changeSFXVolume);

        SliderBGM.value = LoadedSave.Inst.setting.BGMVolume;
        SliderSFX.value = LoadedSave.Inst.setting.SFXVolume;
        if(LoadedSave.Inst.setting.isJoystickFloating) TMPBtnExplain.text = "Jostick will now start from where you touch.";
        else TMPBtnExplain.text = "Joystick will now start from the center.";
    }

    public void changeBGMVolume(float value)
    {
        TMPBGMVolume.text = "Volume : " + (int)(value * 100);
        SoundMgr.Inst.ChangeBGMVolume(value);
        LoadedSave.Inst.setting.BGMVolume = value;
    }


    public void changeSFXVolume(float value)
    {
        TMPSFXVolume.text = "Volume : " + (int)(value * 100);
        SoundMgr.Inst.ChangeSFXVolume(value);
        LoadedSave.Inst.setting.SFXVolume = value;
    }

    public void Btn_FloatingJoystick()
    {
        TMPBtnExplain.text = "Jostick will now start from where you touch.";
        UIMgr.Inst.joystick.isFixed = false;
        LoadedSave.Inst.setting.isJoystickFloating = true;
    }

    public void Btn_FixedJoystick()
    {
        TMPBtnExplain.text = "Joystick will now start from the center.";
        UIMgr.Inst.joystick.isFixed = true;
        LoadedSave.Inst.setting.isJoystickFloating = false;
    }


    #endregion
    public void Btn_BackToTitle()
    {
        LoadedSave.Inst.SyncSetting();
        SoundMgr.Inst.BGMFadeout();
        LoadSceneMgr.LoadSceneAsync("Start");
    }
}
