using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class TutorialMgr : MonoBehaviour
{

    public List<Enemy> TrainingBots = new List<Enemy>();

    public Player player;
    public TextMeshPro progressTMP;
    public SaveData curSaveData;
    public Item throwItem;

    public void Init(Player player, TextMeshPro progressTMP, SaveData curSaveData)
    {
        this.player = player;
        this.progressTMP = progressTMP;
        this.curSaveData = curSaveData;

        TrainingBots.Add(Resources.Load<Enemy>("Prefabs/Enemy/TrainingBot/TrainingBotDoll"));
        TrainingBots.Add(Resources.Load<Enemy>("Prefabs/Enemy/TrainingBot/TrainingBot"));
        TrainingBots.Add(Resources.Load<Enemy>("Prefabs/Enemy/TrainingBot/TrainingBotRanged"));
        throwItem = Resources.Load<Item>("Prefabs/Item/ItemThrowingDagger");
    }
    public void startTutorial()
    {
        StartCoroutine(co_StartTutorial());
    }

    IEnumerator co_StartTutorial()
    {
        yield return new WaitForSeconds(1.0f);
        UIMgr.Inst.progress.ShowStageStart("Toturial");

        yield return new WaitForSeconds(1.0f);
        UIMgr.Inst.progress.HideAll();

        yield return new WaitForSeconds(1.0f);
        SoundMgr.Inst.PlayBGM("Tutorial");
        startP1();
    }
    IEnumerator co_NextPhase(Action next)
    {
        SoundMgr.Inst.Play("Success");
        progressTMP.text = "Great!";
        yield return new WaitForSeconds(1.5f);
        progressTMP.text = "Wait..";
        yield return new WaitForSeconds(1.5f);
        next.Invoke();
    }


    #region p1
    int p1Count;
    //튜토리얼 - 이동
    void startP1()
    {
        player.onMovement += checkP1;
        UIMgr.Inst.progress.ShowNormalUI();
        progressTMP.text = "Tilt joystick to move.";
    }
    void checkP1()
    {
        p1Count++;
        UIMgr.Inst.progress.SetProgress((int)p1Count, 6); ;
        if (p1Count >= 6)
        {
            endP1();
        }

    }
    void endP1()
    {
        player.onMovement -= checkP1;
        UIMgr.Inst.progress.HideAll();

        StartCoroutine(co_NextPhase(startP2));
    }
    #endregion
    #region P2
    //튜토리얼 공격
    int p2Count = 0;
    void startP2()
    {
        UIMgr.Inst.progress.SetProgress((int)p2Count, 2); ;


        EnemyMgr.Inst.SpawnEnemy(TrainingBots[0], new Vector3(2.0f, 1.0f, 0.0f), checkP2);
        EnemyMgr.Inst.SpawnEnemy(TrainingBots[0], new Vector3(-2.0f, 1.0f, 0.0f), checkP2);

        progressTMP.text = "Release joystick to move \ntoward enemy and attack.";
    }

    void checkP2(Vector3 pos)
    {
        p2Count++;
        UIMgr.Inst.progress.SetProgress((int)p2Count, 2); ;
        if (p2Count >= 2)
        {
            endP2();
        }
    }

    void endP2()
    {
        UIMgr.Inst.progress.HideAll();
        StartCoroutine(co_NextPhase(startP3));
    }
    #endregion


    #region P3
    //튜토리얼 투척
    int p3Count = 0;

    void startP3()
    {
        UIMgr.Inst.progress.SetProgress((int)p3Count, 2);
        Instantiate(throwItem, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity)
            .onAcquire += () => EnemyMgr.Inst.SpawnEnemy(TrainingBots[0], new Vector3(0.0f, 1.0f, 0.0f));

     //   EnemyMgr.Inst.SpawnEnemy(TrainingBots[0], new Vector3(0.0f, 1.0f, 0.0f), checkP2);

        player.onThrow += endP3;
        progressTMP.text = "Pick up throwing Item, \n release Joystick to Throw!";
    }

    void endP3()
    {
        UIMgr.Inst.progress.HideAll();
        player.onThrow -= endP3;
        StartCoroutine(co_NextPhase(startP4));
    }

    #endregion

    #region P4
    int p4Count = 0;

    void startP4()
    {
        UIMgr.Inst.progress.SetProgress((int)p4Count, 2);

        EnemyMgr.Inst.SpawnEnemy(TrainingBots[1], new Vector3(-2.5f, -2f, 0f), checkP4);
        EnemyMgr.Inst.SpawnEnemy(TrainingBots[2], new Vector3(2.5f, -2f, 0f), checkP4);

        progressTMP.text = "The enemy's attack locations \nare marked in red.\n Avoid the enemy's attack\n and counterattack!";
    }
    void checkP4(Vector3 pos)
    {
        p4Count++;
        UIMgr.Inst.progress.SetProgress((int)p4Count, 2);

        if (p4Count >= 2)
        {
            endP4();
        }

    }

    void endP4()
    {
        UIMgr.Inst.progress.HideAll();
        StartCoroutine(co_NextPhase(startP5));
    }

    #endregion   
    #region P5
    void startP5()
    {
        clearTutorial();

        Enemy Gong = Instantiate(GameMgr.Inst.GongPrefab);
        Gong.transform.position = Vector3.up * 4;
        //StartNormalStage();
        Gong.onDeath += position =>
        {
            // 무명 함수 내부에서 StartStage() 함수 호출
            StartCoroutine(GameMgr.Inst.StartNormalStage());
        };
        progressTMP.text = "After Smashing Gong, enemies\nwill  begin to spawn.\n prepare your battle!";
    }
    

    #endregion


    void clearTutorial()
    {
        StartCoroutine(SoundMgr.Inst.co_BGMFadeOut());
        curSaveData.ClearAchivement(ACHIEVEMENT.TUTORIALCLEAR);

        UTILS.SaveSaveData(curSaveData);
    }
}