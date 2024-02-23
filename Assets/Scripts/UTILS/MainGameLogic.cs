using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������, ���̺� ������, ���� ���� �ε��ϴ� ��ũ��Ʈ
/// </summary>
public class MainGameLogic : MonoBehaviour
{
    public static bool isInit;
    private void Awake()
    {
        if (!LoadedSave.isInit) LoadedSave.Inst.Init();
        if (!LoadedData.isDataLoaded) LoadedData.Inst.LoadData();
        isInit = true;
    }
}
