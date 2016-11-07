using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckGameResultPhase : Phase
{
    public override void OnEnter()
    {
        base.OnEnter();

        var players = GamePlay.Instance.GetAllPlayers();

        var remainingPlayers = new List<PlayerAgent>();

        foreach (var player in players)
        {
            var towers = TowerManager.Instance.GetTowersOfPlayer(player);
            if (towers.Count == 0)
            {
                player.GameState = PlayerAgent.State.Lose;
                continue;
            }

            if (player.Resource < 0)
            {
                player.GameState = PlayerAgent.State.Lose;
                continue;
            }

            remainingPlayers.Add(player);
        }

        if (remainingPlayers.Count == 1)
        {
            remainingPlayers[0].GameState = PlayerAgent.State.Win;
        }

        if (remainingPlayers.Count <= 1)
        {
            GameOver = true;
        }
    }
}
