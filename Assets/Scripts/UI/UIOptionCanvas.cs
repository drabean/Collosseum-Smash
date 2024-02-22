using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIOptionCanvas : MonoBehaviour
{
    public GameObject totalPanel;

    bool isOptionInit;
    [SerializeField] Slider SliderBGM;
    [SerializeField] TextMeshProUGUI TMPBGMVolume;
    [SerializeField] Slider SliderSFX;
    [SerializeField] TextMeshProUGUI TMPSFXVolume;
    [SerializeField] TextMeshProUGUI TMPBtnExplain;
    public void Init()
    {
        SliderBGM.value = SaveDatas.Inst.setting.BGMVolume;
        TMPBGMVolume.text = "Volume : " + (int)(SaveDatas.Inst.setting.BGMVolume * 100);
        SliderSFX.value = SaveDatas.Inst.setting.SFXVolume;
        TMPSFXVolume.text = "Volume : " + (int)(SaveDatas.Inst.setting.SFXVolume * 100);
        if (SaveDatas.Inst.setting.isJoystickFloating) TMPBtnExplain.text = "Jostick will now start from where you touch.";
        else TMPBtnExplain.text = "Joystick will now start from the center.";

        if (isOptionInit) return;
        isOptionInit = true;
        SliderBGM.onValueChanged.AddListener(changeBGMVolume);
        SliderSFX.onValueChanged.AddListener(changeSFXVolume);
    }

    public void changeBGMVolume(float value)
    {
        TMPBGMVolume.text = "Volume : " + (int)(value * 100);
        SoundMgr.Inst.ChangeBGMVolume(value);
        SaveDatas.Inst.setting.BGMVolume = value;
    }


    public void changeSFXVolume(float value)
    {
        TMPSFXVolume.text = "Volume : " + (int)(value * 100);
        SoundMgr.Inst.ChangeSFXVolume(value);
        SaveDatas.Inst.setting.SFXVolume = value;
    }

    public void Btn_FloatingJoystick()
    {
        TMPBtnExplain.text = "Jostick will now start from where you touch.";
        SaveDatas.Inst.setting.isJoystickFloating = true;
    }

    public void Btn_FixedJoystick()
    {
        TMPBtnExplain.text = "Joystick will now start from the center.";
        SaveDatas.Inst.setting.isJoystickFloating = false;
    }

    public void CloseOption()
    {
        SaveDatas.Inst.SyncSetting();
        totalPanel.SetActive(false);
    }

}
