using UnityEngine;
using System.Collections;

public class GP_ResourcePhase : Phase
{

    public override void OnEnter()
    {
        base.OnEnter();

        var hexes = TowerManager.Instance.GetHexagonsInRange(CurrentPlayer, TowerType.ResourceTower);
        var vision = RangeUtils.GetPlayerVisionServer(CurrentPlayer);
        hexes.IntersectWith(vision);
        TowerManager.Instance.RemoveHexagonsOccupied(hexes);
        CurrentPlayer.AddResource(hexes.Count);
    }
}