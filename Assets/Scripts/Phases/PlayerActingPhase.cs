using UnityEngine;
using System.Collections;

public class PlayerActingPhase : Phase
{
    public bool manualAttack = false;
    public int attackCost = 0;
    private float remainingTime = 0.0f;

    public float minAttackDamage;

    public override void OnEnter()
    {
        base.OnEnter();

        GamePlay.Instance.SetConstant<float>(GamePlay.ConstantName.AttackCost, attackCost);
        CurrentPlayer.RpcStartOperationMode(manualAttack, attackCost);
        remainingTime = timeLimit;
    }

    public override void OnTick()
    {
        base.OnTick();

        remainingTime -= Time.deltaTime;

        CurrentPlayer.RpcSetOperationModeRemainingTime(Mathf.Clamp01(remainingTime / timeLimit));
    }

    public override void OnExit()
    {
        base.OnExit();

        CurrentPlayer.RpcEndOperationMode();
    }
}
