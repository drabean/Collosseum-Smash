using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������, ���̺� ������, ���� ���� �ε��ϴ� ��ũ��Ʈ
/// </summary>
public class MainGameLogic : MonoBehaviour
{
    public static bool isInit;
    private IEnumerator Start()
    {
        yield return null;
        //������ �ε� ȭ�� �����ֱ�


        if (!LoadedSave.isInit) LoadedSave.Inst.Init();
        if (!LoadedData.isDataLoaded) LoadedData.Inst.LoadData();
        if (!AdMgr.isInit) AdMgr.Inst.Init();
        isInit = true;
    }
}
