using UnityEngine;
using System.Collections;

public class ConsumePhase : Phase
{

    public override void OnEnter()
    {
        base.OnEnter();

        int res =
            TowerManager.Instance.SumAttribute(CurrentPlayer, TowerType.ResourceTower, x => { return x.cost; }) +
            TowerManager.Instance.SumAttribute(CurrentPlayer, TowerType.VisionTower, x => { return x.cost; }) +
            TowerManager.Instance.SumAttribute(CurrentPlayer, TowerType.AttackTower, x => { return x.cost; });

        CurrentPlayer.AddResource(-res);

		CurrentPlayer.RpcAddLog ("Your recource is decreased by " + res + ".");
    }
}
