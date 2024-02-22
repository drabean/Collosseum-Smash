using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICoinCanvas : MonoSingleton<UICoinCanvas>
{
    public TextMeshProUGUI TMP;

    public void SyncCoin()
    {
        TMP.text = SaveDatas.Inst.save.Coin.ToString();
    }

    private void Update()
    {
        SyncCoin();
    }
}
