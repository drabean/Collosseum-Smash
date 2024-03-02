using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//게임 중 한번의 Run에 관련된 데이터
[Serializable]
public class RunData
{
    public RunData() { }
    public RunData(int characterInfoIdx, List<int> item, int stageProgress)
    {
        this.characterInfoIdx = characterInfoIdx;
        this.item = item;
        this.stageProgress = stageProgress;
    }


    public int characterInfoIdx; // 캐릭터 Idx
    public List<int> item = new List<int>(); //소지중인 장비들
    public int stageProgress = 0; // 스테이지 진행도
    public int curHP;

    public bool isTutorial = false;
    public bool isBoss = false; // true일 시, 일반 몬스터 스테이지를 건너뛰고, 보스 스테이지로 직행
    public int normalProgress = 0; // 일반 몬스터 진행치 - 이번 스테이지에서 잡은 애들 수
    public int bossProgress = 0; // 보스 진행치 - 이번스테이지에서 적에게 "입힌 피해량"

    public bool isHardMode;

    public int reviveCount = 0; // 부활 남은 횟수
    public bool isRevivedAd = false; // 광고로 부활 했는지.

    public bool isGameOver;
}