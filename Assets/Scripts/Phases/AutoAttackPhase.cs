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
            if (a.state != TowerInfo.BuildingState.Working)
                continue;

            int targetNumbers = 0;

            var range = RangeUtils.GetRangeOfTower(a, vision);

            int m = RangeUtils.maxTargetCount(a.range);

            float addedDamage = 0.5f / (m * m - m);

            foreach (var t in allTowers)
            {
                if (t.playerSlotId == CurrentPlayer.SlotId)
                    continue;

                if (range.Contains(t.coord)) //Auto attack towers
                {
                    targetNumbers ++;
                }
            }

            float currentDamage = 1.0f / targetNumbers + addedDamage;

            foreach (var t in allTowers)
            {
                if (t.playerSlotId == CurrentPlayer.SlotId)
                    continue;

                //Play attack animation

                if (range.Contains(t.coord)) //Auto attack towers
                {
                    t.healthGP -= currentDamage;
                }
            }
        }
    }
}
