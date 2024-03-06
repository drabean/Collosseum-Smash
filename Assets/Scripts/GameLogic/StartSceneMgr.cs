using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    [SerializeField] Animator title;
    [SerializeField] TextMeshPro ProgressTMP;
    public bool isTestMode;

    WaitForSeconds waitForBtn = new WaitForSeconds(0.1f);
    private IEnumerator Start()
    {
        while (!MainGameLogic.isInit) yield return null; // ������ �ʱ�ȭ���� ���


        yield return new WaitForSeconds(0.5f);
        ProgressTMP.text = "Done!";
        SoundMgr.Inst.PlayBGM("StartScene");
        yield return new WaitForSeconds(1.0f);
        ProgressTMP.text = "";
        //���� �� ���� ���� �ִϸ��̼� �����ֱ�
        title.SetTrigger("doSpawn");
        //���̺� ������ Ȯ�� �� ��� ĳ���� ����ȭ
        RunData lastRunData = UTILS.GetRunData();
        if(lastRunData != null)
        {
            CharacterSelectGroup.Init(lastRunData.characterInfoIdx);
            if (lastRunData.isGameOver)
            {
                UTILS.DeleteRunData();
                lastRunData = null;
            }
        }
        else
        {
             CharacterSelectGroup.Init(0);
        }

        List<GameObject> btnsLis = new List<GameObject>();

        //��ư ������� ����
        #region ��ư ����
        //Instantiate Button
        //���ӽ��۹�ư
        UIButton newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Start";
        newBtn.btn.onClick.AddListener(Btn_Start);
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //��Ƽ����ư
        if(lastRunData != null)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Continue"; ;
            newBtn.btn.onClick.AddListener(Btn_Continue);

        }
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //����
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Shop";
        newBtn.btn.onClick.AddListener(Btn_Shop);
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //�ɼ�
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Option";
        newBtn.btn.onClick.AddListener(Btn_Option);
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //�����
        if (isTestMode)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Debug";
            newBtn.btn.onClick.AddListener(Btn_Debug);
        }
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //��������
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Exit";
        newBtn.btn.onClick.AddListener(Btn_Exit);
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);


        //��ư ���� ��ġ ����
        ButtonHolder.GetComponent<RectTransform>().anchoredPosition = Vector3.right * -10 + Vector3.up * (-350 + (50) * ButtonHolder.transform.childCount);

        yield return new WaitForSeconds(0.5f);

        foreach(GameObject btn in btnsLis)
        {
            btn.SetActive(true);
            yield return waitForBtn;
        }
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
