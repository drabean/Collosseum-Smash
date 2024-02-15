using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIOptionCanvas : MonoBehaviour
{
    public GameObject totalPanel;

    bool isOptionInit;
    Settings curSetting;
    [SerializeField] Slider SliderBGM;
    [SerializeField] TextMeshProUGUI TMPBGMVolume;
    [SerializeField] Slider SliderSFX;
    [SerializeField] TextMeshProUGUI TMPSFXVolume;
    [SerializeField] TextMeshProUGUI TMPBtnExplain;
    public void Init()
    {
        curSetting = UTILS.GetSettingData();
        SliderBGM.value = curSetting.BGMVolume;
        TMPBGMVolume.text = "Volume : " + (int)(curSetting.BGMVolume * 100);
        SliderSFX.value = curSetting.SFXVolume;
        TMPSFXVolume.text = "Volume : " + (int)(curSetting.SFXVolume * 100);
        if (curSetting.isJoystickFloating) TMPBtnExplain.text = "Jostick will now start from where you touch.";
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
        curSetting.BGMVolume = value;
    }


    public void changeSFXVolume(float value)
    {
        TMPSFXVolume.text = "Volume : " + (int)(value * 100);
        SoundMgr.Inst.ChangeSFXVolume(value);
        curSetting.SFXVolume = value;
    }

    public void Btn_FloatingJoystick()
    {
        TMPBtnExplain.text = "Jostick will now start from where you touch.";
        curSetting.isJoystickFloating = true;
    }

    public void Btn_FixedJoystick()
    {
        TMPBtnExplain.text = "Joystick will now start from the center.";
        curSetting.isJoystickFloating = false;
    }

    public void CloseOption()
    {
        UTILS.SaveSettingData(curSetting);
        totalPanel.SetActive(false);
    }

}
