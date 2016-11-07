using UnityEngine;
using System.Collections;

public class InitializePhase : Phase
{
    public override void OnEnter()
    {
        base.OnEnter();

        var allPlayers = GamePlay.Instance.GetAllPlayers();
        foreach (var player in allPlayers)
        {
            TowerManager.Instance.BuildTower(player, player.PlayerInfo.camp);
        }
    }
}
