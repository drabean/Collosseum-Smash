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
        //TODO: ������ �����û�ð� Ȯ���ؼ� ��Ȱ��ȭ �ϱ�
    }
    public void Btn_Purchase()
    {
        onPurchase?.Invoke();
        SoundMgr.Inst.Play("Purchase");
        LoadedSave.Inst.save.Coin += 50;
        LoadedSave.Inst.SyncSaveData();
        disableBtn();

    }

    //�гη� ��� �̹� ��ٴ°� �����ֱ�
    //������ ������ �о������
    public void disableBtn()
    {
        DisableGroup.SetActive(true);
    }
}
