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
    [SerializeField] GameObject OptionPanel;
    [SerializeField] GameObject DebugPanel;


    public bool isTestMode;

    private IEnumerator Start()
    {
        //Awake가 아닌, 다른 스크립트에서 일괄적으로 하도록 바뀌어야 함.
        while (!MainGameLogic.isInit) yield return null;
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
        newBtn.btn.onClick.AddListener(BtnStart);

        if(lastRunData != null)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Continue"; ;
            newBtn.btn.onClick.AddListener(BtnContinue);

        }

        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Option";
        newBtn.btn.onClick.AddListener(Btn_Option);
        
        if(isTestMode)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Debug";
            newBtn.btn.onClick.AddListener(Btn_Debug);
        }

        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Exit";
        newBtn.btn.onClick.AddListener(Btn_Exit);

        #endregion
    }
    public void BtnStart()
    {
        Debug.Log("START");
        //SceneManager.LoadScene("Main");
        //runData = new runData()
        //UTILS.SaveRunData();

        CharacterSelectGroup.gameObject.SetActive(true);
        CharacterSelectGroup.OpenCharacterSelect(0);

      //  LoadSceneMgr.LoadSceneAsync("Main");
    }
    public void BtnContinue()
    {
        LoadSceneMgr.LoadSceneAsync("Main");
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
