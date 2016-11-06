using UnityEngine;
using System.Collections;

public class ResourcePhase : Phase
{

    public override void OnEnter()
    {
        base.OnEnter();
        
        var hexes = TowerManager.Instance.GetHexagonsInRange(CurrentPlayer, TowerType.ResourceTower);
        CurrentPlayer.AddResource(hexes.Count);
    }
}
