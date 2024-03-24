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
        //TODO: ������ �����û�ð� Ȯ���ؼ� ��Ȱ��ȭ �ϱ�

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
        //���� Ȯ����, ��û�ÿ��� ����
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
    //�гη� ��� �̹� ��ٴ°� �����ֱ�
    //������ ������ �о������
    public void disableBtn()
    {
        DisableGroup.SetActive(true);
    }
}
