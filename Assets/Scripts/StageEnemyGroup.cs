using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEnemyGroup : StageMechanic
{
    public List<StageEnemy> gimmics = new List<StageEnemy>();

    public override void Init(Transform target)
    {
        base.Init(target);
        foreach (StageEnemy enemy in gimmics) enemy.Init(target);
    }
    public override void StartAction()
    {
        foreach (StageEnemy enemy in gimmics) enemy.StartAction();
    }

    public override void endAction()
    {
        foreach (StageEnemy enemy in gimmics) enemy.endAction();

    }
}
