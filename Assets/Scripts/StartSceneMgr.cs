using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneMgr : MonoSingleton<StartSceneMgr>
{
    [SerializeField] UICharacterSelectCanvas CharacterSelectGroup;
    [SerializeField] UIItemDescriptionPanel ItemDescription;
    [SerializeField] RectTransform ButtonHolder;
    [SerializeField] UIButton ButtonPrefab;
    [SerializeField] UIOptionCanvas Option;
    [SerializeField] UIShopCanvas Shop;
    [SerializeField] GameObject OptionPanel;
    [SerializeField] GameObject DebugPanel;


    public bool isTestMode;

    private IEnumerator Start()
    {
        while (!MainGameLogic.isInit) yield return null; // 데이터 초기화까지 대기

        //세이브 데이터 확인 및 배경 캐릭터 동기화
        RunData lastRunData = UTILS.GetRunData();

        if(lastRunData != null)
        {
            CharacterSelectGroup.Init(lastRunData.characterInfoIdx);
        }
        else
        {
            CharacterSelectGroup.Init(0);
        }

        #region 버튼 생성
        //Instantiate Button
        UIButton newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Start";
        newBtn.btn.onClick.AddListener(Btn_Start);

        if(lastRunData != null)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Continue"; ;
            newBtn.btn.onClick.AddListener(Btn_Continue);

        }

        //옵션
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Shop";
        newBtn.btn.onClick.AddListener(Btn_Shop);

        //옵션
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Option";
        newBtn.btn.onClick.AddListener(Btn_Option);
        
        //디버그
        if(isTestMode)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Debug";
            newBtn.btn.onClick.AddListener(Btn_Debug);
        }

        //게임종료
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Exit";
        newBtn.btn.onClick.AddListener(Btn_Exit);

        //버튼 묶음 위치 조절
        ButtonHolder.GetComponent<RectTransform>().anchoredPosition = Vector3.right * -10 + Vector3.up * (-300 + (50) * ButtonHolder.transform.childCount);
        #endregion
    }
    public void Btn_Start()
    {
        Debug.Log("START");

        CharacterSelectGroup.gameObject.SetActive(true);
        CharacterSelectGroup.OpenCharacterSelect(0);
    }
    public void Btn_Continue()
    {
        LoadSceneMgr.LoadSceneAsync("Main");
    }
    public void Btn_Shop()
    {
        Shop.OpenPanel();
    }
    public void Btn_Option()
    {
        Debug.Log("OpenOption");
        OptionPanel.SetActive(true);
        Option.Init();
    }
    public void Btn_Debug( )
    {
        Debug.Log("OpenDebug");
        DebugPanel.SetActive(true);
        
    }
    public void Btn_Exit()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
