using UnityEngine;
using System.Collections;

public class BuildingStateTransitionPhase : Phase
{
    public override void OnEnter()
    {
        base.OnEnter();

        var vision = RangeUtils.GetPlayerVisionServer(CurrentPlayer);

        var towers = TowerManager.Instance.GetTowersOfPlayer(CurrentPlayer);
        for (int i = 0; i < towers.Count; i++)
        {
            if (towers[i].state == TowerInfo.BuildingState.Building)
            {
                if (vision.Contains(towers[i].coord))
                    towers[i].state = TowerInfo.BuildingState.Working;
                else
                    towers[i].state = TowerInfo.BuildingState.OutOfVision;
            }
        }
    }
}
