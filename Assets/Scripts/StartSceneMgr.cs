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
        while (!MainGameLogic.isInit) yield return null; // ������ �ʱ�ȭ���� ���

        //���̺� ������ Ȯ�� �� ��� ĳ���� ����ȭ
        RunData lastRunData = UTILS.GetRunData();

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
        newBtn.btn.onClick.AddListener(Btn_Start);

        if(lastRunData != null)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Continue"; ;
            newBtn.btn.onClick.AddListener(Btn_Continue);

        }

        //�ɼ�
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Shop";
        newBtn.btn.onClick.AddListener(Btn_Shop);

        //�ɼ�
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Option";
        newBtn.btn.onClick.AddListener(Btn_Option);
        
        //�����
        if(isTestMode)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Debug";
            newBtn.btn.onClick.AddListener(Btn_Debug);
        }

        //��������
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Exit";
        newBtn.btn.onClick.AddListener(Btn_Exit);

        //��ư ���� ��ġ ����
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
