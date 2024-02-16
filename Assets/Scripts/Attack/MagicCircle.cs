using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagicCircle : MonoBehaviour
{
    public int maxCharge;
    int curCharge = 0;

    public Attack magic;

    public TextMeshPro TMP;
    public TargetedProjecitile chargeEffect;
    [SerializeField] ModuleHit hitModule;
    public Item ItemPrefab;
    Item curItem;


    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(itemSpawnCoroutine());
    }

    IEnumerator itemSpawnCoroutine()
    {
        while (true)
        {
            if (curItem == null)
            {
                spawnItem();
            }
            yield return new WaitForSeconds(3.0f);
        }
    }

    void spawnItem()
    {
        Debug.Log("SpawnITem");
        curItem = Instantiate(ItemPrefab, EnemyMgr.Inst.getRandomPos(), Quaternion.identity);
        curItem.onAcquire += showEffect;
    }

    void showEffect()
    {
        TargetedProjecitile effect = Instantiate(chargeEffect, curItem.transform.position, Quaternion.identity);
        effect.Shoot(curItem.transform.position, gameObject);
        effect.onTouch += Charge;

    }

    public void Charge()
    {
        hitModule.FlashWhite(0.1f);
        if (curCharge < maxCharge-1)
        {
            curCharge++;
            TMP.text = curCharge + " / " + maxCharge;
        }
        else
        {
            Shoot();
            curCharge = 0;
            TMP.text = "Fire!";
        }

    }

    public void Shoot()
    {
        //상하좌우폭격

        for (int i = 0; i < 8; i++)
        {
            // 각도를 라디안으로 변환
            float radians = (i * 45) * Mathf.Deg2Rad;

            // 좌표 계산
            Vector3 fireVec = (Vector3.right * Mathf.Cos(radians) + Vector3.up * Mathf.Sin(radians)).normalized;
            Instantiate(magic, transform.position + fireVec * 1f, Quaternion.identity).Shoot(transform.position + fireVec * 1f, transform.position + fireVec * 1.5f);
        }
        GameMgr.Inst.MainCam.Shake(0.3f, 30f, 0.1f, 0f);
    }
}
