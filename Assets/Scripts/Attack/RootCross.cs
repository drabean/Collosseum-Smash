using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootCross :Attack
{
    public Attack RootPrefab;
    public float RootInterval;
    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        Instantiate(RootPrefab).Shoot(startPos, targetPos);
        Instantiate(RootPrefab).Shoot(startPos + Vector3.right * RootInterval, targetPos + Vector3.right * RootInterval);
        Instantiate(RootPrefab).Shoot(startPos + Vector3.left * RootInterval, targetPos + Vector3.left * RootInterval);
        Instantiate(RootPrefab).Shoot(startPos + Vector3.up * RootInterval, targetPos + Vector3.up * RootInterval);
        Instantiate(RootPrefab).Shoot(startPos + Vector3.down * RootInterval, targetPos + Vector3.down * RootInterval);

        Destroy(gameObject);
    }

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time, float size = 1)
    {
        GameObject warning = RootPrefab.ShowWarning(startPos, targetPos, time);


        RootPrefab.ShowWarning(startPos, targetPos + Vector3.right * RootInterval, time);

        RootPrefab.ShowWarning(startPos, targetPos + Vector3.left * RootInterval, time );

        RootPrefab.ShowWarning(startPos, targetPos + Vector3.up * RootInterval, time);

        RootPrefab.ShowWarning(startPos, targetPos + Vector3.down * RootInterval, time );

        return warning;
    }

}
