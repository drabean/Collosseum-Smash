using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 데이터, 세이브 데이터, 세팅 등을 로딩하는 스크립트
/// </summary>
public class MainGameLogic : MonoBehaviour
{
    public static bool isInit;
    private IEnumerator Start()
    {
        yield return null;
        //데이터 로딩 화면 보여주기


        if (!LoadedSave.isInit) LoadedSave.Inst.Init();
        if (!LoadedData.isDataLoaded) LoadedData.Inst.LoadData();
        if (!AdMgr.isInit) AdMgr.Inst.Init();
        isInit = true;
    }
}
