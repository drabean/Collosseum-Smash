using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSpikes : StageMechanic
{
    Animator[,] spikes = new Animator[10, 8];
    public Animator SpikePrefab;

    bool isActionActive = false;

    //전체 가시를 4가지 구역으로 나눈것
    List<Animator>[] area4 = new List<Animator>[4] { new List<Animator>(), new List<Animator>(), new List<Animator>(), new List<Animator>() };

    void spawnSpikes()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Animator spike = Instantiate(SpikePrefab, (-6.75f + 1.5f * x) * Vector3.right + (-6 + 1.5f * y) * Vector3.up, Quaternion.identity);
                spikes[x, y] = spike;

                //4구역 가시들 초기화
                if ((x >= 0) && (x <= 4) && (y >= 0) && (y <= 3)) area4[0].Add(spike);
                if ((x >= 0) && (x <= 4) && (y >= 4) && (y <= 7)) area4[1].Add(spike);
                if ((x >= 5) && (x <= 9) && (y >= 0) && (y <= 3)) area4[2].Add(spike);
                if ((x >= 5) && (x <= 9) && (y >= 4) && (y <= 7)) area4[3].Add(spike);
            }
        }
    }

    public override void Init(Transform target)
    {
        base.Init(target);
        spawnSpikes();

        StartCoroutine(co_ActionCoroutine());
    }


    public override void StartAction()
    {
        isActionActive = true;
    }

    public override void endAction()
    {
        isActionActive = false;

    }


    IEnumerator co_ActionCoroutine()
    {
        while (true)
        {
            if (isActionActive)
            {
                int idx = Random.Range(0, 4);
                spikeWarning(area4[idx], 1.0f);
                yield return new WaitForSeconds(1.5f);
                showSpikes(area4[idx]);
                yield return new WaitForSeconds(1.5f);
                hideAllSpikes();
                yield return new WaitForSeconds(3.5f);
            }
            else
            {
                yield return null;
            }
        }
    }
    void spikeWarning(List<Animator> spikeLis, float time)
    {
        foreach (Animator spike in spikeLis)
        {
            GameMgr.Inst.AttackEffectLinear(spike.transform.position + Vector3.right * -0.7f, spike.transform.position + Vector3.right * 0.7f, 1.4f, time);
        }
    }

    void showSpikes(List<Animator> spikeLis)
    {
        foreach (Animator spike in spikeLis)
        {
            spike.SetBool("isOn", true);
        }
    }
    void hideAllSpikes()
    {
        foreach (Animator spike in spikes)
        {
            spike.SetBool("isOn", false);
        }
    }
}
