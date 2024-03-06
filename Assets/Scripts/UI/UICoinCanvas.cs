using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICoinCanvas : MonoSingleton<UICoinCanvas>
{
    public TextMeshProUGUI TMP;

    private IEnumerator Start()
    {
        TMP.gameObject.SetActive(false);
        while (!LoadedSave.isInit) yield return null;
        TMP.gameObject.SetActive(true);

    }
    public void SyncCoin()
    {
        TMP.text = LoadedSave.Inst.save.Coin.ToString();
    }

    private void Update()
    {
        if (LoadedSave.isInit) SyncCoin();
    }
}
