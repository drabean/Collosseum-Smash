using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;


public class AdMgr : MonoSingleton<AdMgr>
{
    public static bool isInit = false;

    public void Init()
    {
        MobileAds.Initialize(initStatus => { });

        LoadInterstitialAd();


        DontDestroyOnLoad(gameObject);
        isInit = true;
    }


    #region Àü¸é±¤°í
#if UNITY_ANDROID
    private string _intersititialAdID = "ca-app-pub-3317633089740042/1758431422";
#else
  private string _intersititialAdID  = "unused";
#endif

    private InterstitialAd _interstitialAd;

  /// <summary>
  /// Loads the interstitial ad.
  /// </summary>
  public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_intersititialAdID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }

    public void SetInterstitialAdEvent(Action action)
    {
        rewardAction = action;

        _interstitialAd.OnAdFullScreenContentClosed += () => {rewardFlag = true; };
    }
    public void ShowInterstialAd()
    {
        StartCoroutine(co_ShowInterstial());
    }

    IEnumerator co_ShowInterstial()
    {

        while (_interstitialAd != null && !_interstitialAd.CanShowAd()) yield return new WaitForSeconds(0.2f);

        _interstitialAd.Show();

        yield return null;
    }

    public bool CheckCanShow()
    {
        return _interstitialAd.CanShowAd();
    }

    Action rewardAction;
    bool rewardFlag;


    private void Update()
    {
        if(rewardFlag && rewardAction != null)
        {
            rewardFlag = false;
            rewardAction?.Invoke();
            rewardAction = null;

            LoadInterstitialAd();
        }
    }
    #endregion




}