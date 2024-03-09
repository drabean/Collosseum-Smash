using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIAdPrefab : MonoBehaviour
{
    public Action onPurchase;
    public GameObject DisableGroup;

    public void Init()
    {
        //TODO: 마지막 광고시청시간 확인해서 비활성화 하기
    }
    public void Btn_Purchase()
    {
        onPurchase?.Invoke();
        SoundMgr.Inst.Play("Purchase");
        LoadedSave.Inst.save.Coin += 50;
        LoadedSave.Inst.SyncSaveData();
        disableBtn();

    }

    //패널로 덮어서 이미 샀다는걸 보여주기
    //순서를 밑으로 밀어버리기
    public void disableBtn()
    {
        DisableGroup.SetActive(true);
    }
}
