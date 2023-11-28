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

    private void Awake()
    {
        //Awake가 아닌, 다른 스크립트에서 일괄적으로 하도록 바뀌어야 함.
        if(!LoadedData.Inst.isDataLoaded) LoadedData.Inst.LoadData();

        //세이브 데이터 확인 및 배경 캐릭터 동기화
        runData lastRunData = UTILS.GetRunData();

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
        LoadSceneMgr.LoadSceneAsync("Tutorial");
    }

    public void Btn_Exit()
    {
        Application.Quit();
    }

    public void ShowItemDescription(Equip equip)
    {
        ItemDescription.ShowPanel(equip);
        ItemDescription.gameObject.SetActive(true);
    }
}
