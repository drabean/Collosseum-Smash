using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
//using GoogleMobileAds.Api;


public class UIAdPrefab : MonoBehaviour
{
    public Action onPurchase;
    public GameObject DisableGroup;
    public TextMeshProUGUI TMPTimeSpan;
    public TextMeshProUGUI TMPDestription;

    int ADCoolTime = 10;
    int reward = 40;
    public void Init()
    {
        //TODO: 마지막 광고시청시간 확인해서 비활성화 하기

        bool canWatchAD;

        string lastADTime = PlayerPrefs.GetString("LastADBtnTime");

        TMPDestription.text = "Watch Ads to get "+reward +" Coin! \n(Available every "+ ADCoolTime.ToString() +" minutes)";
        if(lastADTime.Equals(""))
        {
            Debug.Log("FIRST");
            canWatchAD = true;
        }
        else
        {
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse(lastADTime);

            if(timeSpan.TotalMinutes >= ADCoolTime)
            {
                canWatchAD = true;
            }
            else
            {
                canWatchAD = false;
                double timeLeft = ADCoolTime - (timeSpan.Minutes);
                string timeLeftString = timeLeft.ToString("F0");
                TMPTimeSpan.text = "Can Watch \nAfter " + timeLeftString + " Minutes";
            }
        }


        if (!canWatchAD)
        {
            disableBtn();
        }
    }
    public void Btn_Purchase()
    {
        //광고 확인후, 시청시에만 적용
        AdMgr.Inst.LoadRewardedAd();
        AdMgr.Inst.ShowRewardedAd();


    }

    public void rewardAction()
    {
        SoundMgr.Inst.Play("Purchase");
        LoadedSave.Inst.save.Coin += reward;
        LoadedSave.Inst.SyncSaveData();

        PlayerPrefs.SetString("LastADBtnTime", DateTime.Now.ToString());
        disableBtn();
        TMPTimeSpan.text = "Can Watch \nAfter " + ADCoolTime.ToString() + " Minutes";
    }
    //패널로 덮어서 이미 샀다는걸 보여주기
    //순서를 밑으로 밀어버리기
    public void disableBtn()
    {
        DisableGroup.SetActive(true);
    }
}
