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
    [SerializeField] UIAchievementCanvas Achivement;
    [SerializeField] UIAchievementShowCanvas AchivementShow;
    [SerializeField] GameObject OptionPanel;
    [SerializeField] GameObject DebugPanel;
    [SerializeField] GameObject AchivementBtn;

    [SerializeField] Animator title;
    [SerializeField] TextMeshPro ProgressTMP;
    public bool isTestMode;

    WaitForSeconds waitForBtn = new WaitForSeconds(0.1f);
    private IEnumerator Start()
    {
        while (!MainGameLogic.isInit) yield return null; // 데이터 초기화까지 대기

        Application.targetFrameRate = 60;

        yield return new WaitForSeconds(0.5f);
        ProgressTMP.text = "Done!";
        SoundMgr.Inst.PlayBGM("StartScene");
        yield return new WaitForSeconds(1.0f);
        ProgressTMP.text = "";
        //메인 씬 글자 생성 애니메이션 보여주기
        title.SetTrigger("doSpawn");
        //세이브 데이터 확인 및 배경 캐릭터 동기화
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

        //버튼 순서대로 생성
        #region 버튼 생성
        //Instantiate Button
        //게임시작버튼
        UIButton newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Start";
        newBtn.btn.onClick.AddListener(Btn_Start);
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //컨티뉴버튼
        if(lastRunData != null)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Continue"; ;
            newBtn.btn.onClick.AddListener(Btn_Continue);

        }
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //상점
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Shop";
        newBtn.btn.onClick.AddListener(Btn_Shop);
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //옵션
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Option";
        newBtn.btn.onClick.AddListener(Btn_Option);
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //디버그
        if (isTestMode)
        {
            newBtn = Instantiate(ButtonPrefab, ButtonHolder);
            newBtn.txt.text = "Debug";
            newBtn.btn.onClick.AddListener(Btn_Debug);
        }
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        //게임종료
        newBtn = Instantiate(ButtonPrefab, ButtonHolder);
        newBtn.txt.text = "Exit";
        newBtn.btn.onClick.AddListener(Btn_Exit);
        btnsLis.Add(newBtn.gameObject);
        newBtn.gameObject.SetActive(false);

        btnsLis.Add(AchivementBtn);

        //버튼 묶음 위치 조절
        ButtonHolder.GetComponent<RectTransform>().anchoredPosition = Vector3.right * -10 + Vector3.up * (-350 + (50) * ButtonHolder.transform.childCount);

        yield return new WaitForSeconds(0.5f);

        foreach(GameObject btn in btnsLis)
        {
            btn.SetActive(true);
            yield return waitForBtn;
        }
        #endregion


        checkAchievements();
        showAchivementClear();
    }

    #region 업적 확인하기

    void checkAchievements()
    {
        if (LoadedSave.Inst.save.BossKill >= 5) LoadedSave.Inst.TryAddAchievement(ACHIEVEMENT.SMASHBOSS5);
        if (LoadedSave.Inst.save.BossKill >= 10) LoadedSave.Inst.TryAddAchievement(ACHIEVEMENT.SMASHBOSS10);
        if (LoadedSave.Inst.save.NormalKill >= 50) LoadedSave.Inst.TryAddAchievement(ACHIEVEMENT.SMASHNORMAL50);
        if (LoadedSave.Inst.save.NormalKill >= 100) LoadedSave.Inst.TryAddAchievement(ACHIEVEMENT.SMASHNORMAL100);
    }
    #endregion


    #region 업적 보여주기

    public void showAchivementClear()
    {
        if (LoadedSave.Inst.achivementShowList.list.Count == 0)
        {
            AchivementShow.gameObject.SetActive(false);
            return;
        }
        ACHIEVEMENT ac = LoadedSave.Inst.achivementShowList.GetAchievement();
        LoadedSave.Inst.SyncAchivementShowList();

        if (LoadedSave.Inst.save.CheckAchivement(ac))
        {
            showAchivementClear(); // 이미 클리어한 업적을 중복 클리어 하는 상황 방지.
            return;
        }

        StartCoroutine(co_ShowAchievementClear(ac));
    }

    IEnumerator co_ShowAchievementClear(ACHIEVEMENT Achievement)
    {
        AchievementInfo info = LoadedData.Inst.getAchivementInfo(Achievement);

        LoadedSave.Inst.save.ClearAchivement(info.Achievement);
        switch (info.RewardType)
        {
            case REWARDTYPE.COIN:
                LoadedSave.Inst.save.Coin += info.RewardValue;
                SoundMgr.Inst.Play("Coin");
                break;
        }
        LoadedSave.Inst.SyncSaveData();

        yield return AchivementShow.Show(LoadedData.Inst.getAchivementInfo(Achievement));

        yield return new WaitForSeconds(1.0f);
        showAchivementClear();
    }
    #endregion

    #region 버튼 체인 함수
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

    public void Btn_Achivement()
    {
        Achivement.OpenPanel();
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
    #endregion
}
