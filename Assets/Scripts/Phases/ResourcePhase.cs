using UnityEngine;
using System.Collections;

public class ResourcePhase : Phase
{

    public override void OnEnter()
    {
        base.OnEnter();
        
        CurrentPlayer.AddResource(CurrentPlayer.Production);
		UIController.Instance.AddLog ("Your resource is increased by " + CurrentPlayer.Production + ".");
    }
}
