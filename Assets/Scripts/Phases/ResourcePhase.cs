using UnityEngine;
using System.Collections;

public class ResourcePhase : Phase
{

    public override void OnEnter()
    {
        base.OnEnter();
        
        CurrentPlayer.AddResource(CurrentPlayer.Production);
		CurrentPlayer.RpcAddLog ("Your resource is increased by " + CurrentPlayer.Production + ".");
    }
}
