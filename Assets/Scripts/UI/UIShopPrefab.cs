using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class UIShopPrefab : MonoBehaviour
{
    public Action onPurchase;
    public GameObject DisableGroup;


    public TextMeshProUGUI TMP_Cost;
    public TextMeshProUGUI TMP_Title;
    public TextMeshProUGUI TMP_Description;

    [SerializeField] int cost;
    [SerializeField] UNLOCK unlock;


    public void Init(int cost, UNLOCK unlock, string title, string description)
    {
        this.cost = cost;
        this.unlock = unlock;
        TMP_Cost.text = cost.ToString();

        TMP_Title.text = title;
        TMP_Description.text = description;

    }
    public void Btn_Purchase()
    {
        onPurchase?.Invoke();
        if(LoadedSave.Inst.save.Coin > cost)
        {
            SoundMgr.Inst.Play("Purchase");
            LoadedSave.Inst.save.Coin -= cost;
            LoadedSave.Inst.save.BuyUnlock(unlock);
            disableBtn();
            LoadedSave.Inst.SyncSaveData();
        }
    }

    //패널로 덮어서 이미 샀다는걸 보여주기
    //순서를 밑으로 밀어버리기
    public void disableBtn()
    {
        DisableGroup.SetActive(true);
    }
}