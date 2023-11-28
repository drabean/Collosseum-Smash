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
        //Awake�� �ƴ�, �ٸ� ��ũ��Ʈ���� �ϰ������� �ϵ��� �ٲ��� ��.
        if(!LoadedData.Inst.isDataLoaded) LoadedData.Inst.LoadData();

        //���̺� ������ Ȯ�� �� ��� ĳ���� ����ȭ
        runData lastRunData = UTILS.GetRunData();

        if(lastRunData != null)
        {
            CharacterSelectGroup.Init(lastRunData.characterInfoIdx);
        }
        else
        {
            CharacterSelectGroup.Init(0);
        }

        #region ��ư ����
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
