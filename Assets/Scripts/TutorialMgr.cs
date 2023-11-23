using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class TutorialMgr : MonoBehaviour
{
    public TextMeshPro TMP_Tutorial;
    Player player;
    public List<Enemy> TrainingBots = new List<Enemy>();
    public List<Transform> SpawnPoints = new List<Transform>();
    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>GameMgr.Inst.isPlayerInstantiated);
        player = GameObject.FindObjectOfType<Player>();
        SoundMgr.Inst.PlayBGM("Tutorial");
        startP1();
    }

    IEnumerator co_NextPhase(Action next)
    {
        SoundMgr.Inst.Play("Success");
        TMP_Tutorial.text = "Great!";
        yield return new WaitForSeconds(1.5f);
        TMP_Tutorial.text = "Wait..";
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
        TMP_Tutorial.text = "Touch joystick to move.";
    }
    void checkP1()
    {
        p1Count++;
        UIMgr.Inst.progress.SetProgress((int)p1Count, 10); ;
        if (p1Count >= 10)
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
    int p2Count = 0;
    void startP2()
    {
        UIMgr.Inst.progress.SetProgress((int)p2Count, 2); ;

        for (int i = 0; i < 2; i++)
        {
            EnemyMgr.Inst.SpawnEnemy(TrainingBots[0], SpawnPoints[i].position, checkP2);
        }
        TMP_Tutorial.text = "Release joystick to move \ntoward enemy and attack.";
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
    int p3Count = 0;

    void startP3()
    {
        UIMgr.Inst.progress.SetProgress((int)p3Count, 2);

        EnemyMgr.Inst.SpawnEnemy(TrainingBots[1], SpawnPoints[0].position, checkP3);
        EnemyMgr.Inst.SpawnEnemy(TrainingBots[2], SpawnPoints[1].position, checkP3);

        TMP_Tutorial.text = "Red area warn enemy's attack.\n Avoid it and smash enemy.";
    }
    void checkP3(Vector3 pos)
    {
        p3Count++;
        UIMgr.Inst.progress.SetProgress((int)p3Count, 2);

        if (p3Count >= 2)
        {
            endP3();
        }
    }

    void endP3()
    {
        UIMgr.Inst.progress.HideAll();
        StartCoroutine(co_ToMainScene());
    }
    #endregion


    IEnumerator co_ToMainScene()
    {
        SoundMgr.Inst.Play("Success");
        TMP_Tutorial.text = "Great!";
        yield return new WaitForSeconds(1.5f);
        TMP_Tutorial.text = "Prepare your battle...";
        yield return new WaitForSeconds(3.0f);


        SoundMgr.Inst.StopBGM();
        LoadSceneMgr.LoadSceneAsync("Main");
    }
}