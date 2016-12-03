using UnityEngine;
using System.Collections;

public class PlayerActingPhase : Phase
{
    public bool foreachPlayer = false;
    public bool manualAttack = false;
    public int attackCost = 0;
    private float remainingTime = 0.0f;

    public float minAttackDamage;

    public override void OnEnter()
    {
        base.OnEnter();

        if (manualAttack)
            GamePlay.Instance.SetConstant<int>(GamePlay.ConstantName.AttackCost, attackCost);

        if (foreachPlayer)
        {
            var players = GamePlay.Instance.GetAllPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                players[i].RpcStartOperationMode(manualAttack, attackCost, foreachPlayer);
            }
        }
        else
        {
            CurrentPlayer.RpcStartOperationMode(manualAttack, attackCost, foreachPlayer);
        }
        remainingTime = timeLimit;
    }

    public override void OnTick()
    {
        base.OnTick();

        remainingTime -= Time.deltaTime;

        if (foreachPlayer)
        {
            var players = GamePlay.Instance.GetAllPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                players[i].RpcSetOperationModeRemainingTime(Mathf.Clamp01(remainingTime / timeLimit));
            }
        }
        else
        {
            CurrentPlayer.RpcSetOperationModeRemainingTime(Mathf.Clamp01(remainingTime / timeLimit));
        }
    }

    public override void OnExit()
    {
        base.OnExit();


        if (foreachPlayer)
        {
            var players = GamePlay.Instance.GetAllPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                players[i].RpcEndOperationMode();
                ClearAttackFlags(players[i]);
            }
        }
        else
        {
            CurrentPlayer.RpcEndOperationMode();
            ClearAttackFlags(CurrentPlayer);
        }
    }

    private void ClearAttackFlags(PlayerAgent player)
    {
        var towers = TowerManager.Instance.GetTowersOfPlayer(player);
        foreach (var tower in towers)
        {
            if (tower.type == TowerType.AttackTower)
                tower.attacked = false;
        }
    }
}
