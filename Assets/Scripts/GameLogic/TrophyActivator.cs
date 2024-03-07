using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyActivator : MonoBehaviour
{
    public List<GameObject> trophyPrefabs;
    public List<Transform> trophyPositions;


    private IEnumerator Start()
    {
        while (!LoadedSave.isInit) yield return null;

        InstantiateTrophy();
    }
    void InstantiateTrophy()
    {
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.TUTORIALCLEAR)) Instantiate(trophyPrefabs[0], trophyPositions[0]);
        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.NORMALCLEAR)) Instantiate(trophyPrefabs[1], trophyPositions[1]);
    }
}
