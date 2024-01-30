using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyActivator : MonoBehaviour
{
    public List<GameObject> trophyPrefabs;
    public List<Transform> trophyPositions;


    private void Awake()
    {
        InstantiateTrophy();
    }
    void InstantiateTrophy()
    {
        SaveData data = UTILS.LoadSaveData();
        if (data.checkAchivement(ACHIEVEMENT.TUTORIALCLEAR)) Instantiate(trophyPrefabs[0], trophyPositions[0]);
        if (data.checkAchivement(ACHIEVEMENT.NORMALCLEAR)) Instantiate(trophyPrefabs[1], trophyPositions[1]);
    }
}
