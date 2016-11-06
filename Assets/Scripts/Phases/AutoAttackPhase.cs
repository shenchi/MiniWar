using UnityEngine;
using System.Collections;

public class AutoAttackPhase : Phase
{
    public override void OnEnter()
    {
        base.OnEnter();

        var vision = TowerManager.Instance.GetHexagonsInVision(CurrentPlayer);
        var allAttackers = TowerManager.Instance.GetTowersOfType(CurrentPlayer, TowerType.AttackTower);

        var allTowers = TowerManager.Instance.GetAllTowers();

        foreach (var a in allAttackers)
        {
            var range = HexagonUtils.NeighborHexagons(a.coord, a.range);
            range.IntersectWith(vision);
            range.RemoveWhere(x => { return !MapManager.Instance.Exists(x); });

            foreach (var t in allTowers)
            {
                if (t.playerSlotId == CurrentPlayer.SlotId)
                    continue;

                if (range.Contains(t.coord))
                {
                    t.health--;
                }
            }
        }
    }
}
