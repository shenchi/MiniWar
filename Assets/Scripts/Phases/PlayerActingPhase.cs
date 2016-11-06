using UnityEngine;
using System.Collections;

public class PlayerActingPhase : Phase
{
    private float remainingTime = 0.0f;

    public override void OnEnter()
    {
        base.OnEnter();

        CurrentPlayer.RpcStartOperationMode();
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
