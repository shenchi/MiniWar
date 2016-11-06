using UnityEngine;
using System.Collections;

public class ResourcePhase : Phase
{

    public override void OnEnter()
    {
        base.OnEnter();
        
        var hexes = TowerManager.Instance.GetHexagonsInRange(CurrentPlayer, TowerType.ResourceTower);
        var vision = TowerManager.Instance.GetHexagonsInVision(CurrentPlayer);
        hexes.IntersectWith(vision);
        hexes.RemoveWhere(x => { return TowerManager.Instance.Occupied(x); });
        CurrentPlayer.AddResource(hexes.Count);
    }
}
