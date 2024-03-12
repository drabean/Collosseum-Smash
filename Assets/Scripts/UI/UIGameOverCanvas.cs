using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIGameOverCanvas : MonoBehaviour
{
    Player player;
    RunData data;

    public GameObject CanContinueGroup;
    public GameObject GameOvergroup;
    public GameObject ConitnueMessageGroup;
    
    public GameObject ReviveBtnLock;

    public TextMeshProUGUI TMP_ReviveCountLeft;
    public TextMeshProUGUI TMP_ContinueMessage;
    private void Awake()
    {
        data = UTILS.GetRunData();

        if(data == null)
        {
            data = new RunData(0 , new List<int>());
        }

        player = Instantiate(LoadedData.Inst.getCharacterInfoByID(data.characterInfoIdx).playerPrefab, new Vector3(0,2,0), Quaternion.identity);
        player.StartDeadMotion();

        if(!data.isGameOver)
        {
            CanContinueGroup.SetActive(true);
            GameOvergroup.SetActive(false);
            ConitnueMessageGroup.SetActive(false);

            TMP_ReviveCountLeft.text = data.reviveCount.ToString() + " Left";
            if (LoadedSave.Inst.save.Coin < 20)
            {
                Debug.Log("CantRevive!");
                ReviveBtnLock.SetActive(true);
            }
            else
            {
                ReviveBtnLock.SetActive(false);
            }
        }
        else
        {
            CanContinueGroup.SetActive(false);
            GameOvergroup.SetActive(true);
        }
        data.isGameOver = true; // 게임오버씬에서 아무것도 하지 않고, 강제 종료 할 경우 이어서 할 수 없도록 함
        UTILS.SaveRunData(data);

    }

    private void Start()
    {
        SoundMgr.Inst.PlayBGM("GameOver");
    }

    public void BtnContinueStage()
    {
        CanContinueGroup.SetActive(false);
        GameOvergroup.SetActive(false);

        player.EndDeadMotion();
        player.AutoMove(new Vector3(15, 2, 0));
        SoundMgr.Inst.BGMFadeout();
        StartCoroutine(co_ContinueStage());
    }

    IEnumerator co_ContinueStage()
    {
        data.curHP = 10;
        data.reviveCount--;
        data.isGameOver = false; // 정상적인 방법으로 Continue 했으므로, GameOver 되지 않도록 함.
        ConitnueMessageGroup.SetActive(true);
        if(data.reviveCount != 0)TMP_ContinueMessage.text = "You Have " + data.reviveCount + " Chance Left. \n Good Luck!";
        else TMP_ContinueMessage.text = "This is your last chance. \n Wish your Luck!";
        UTILS.SaveRunData(data);

        LoadedSave.Inst.TryAddAchievement(ACHIEVEMENT.FIRSTRETRY);
        LoadedSave.Inst.save.Coin -= 20;
        LoadedSave.Inst.SyncSaveData();
        SoundMgr.Inst.Play("Purchase");
        
        yield return new WaitForSeconds(2.0f);

        LoadSceneMgr.LoadSceneAsync("Main");
    }

    public void BtnEndStage()
    {
        UTILS.DeleteRunData();
        SoundMgr.Inst.BGMFadeout();
        LoadSceneMgr.LoadSceneAsync("Start");
    }
}
