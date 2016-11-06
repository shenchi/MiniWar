using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClearTowersPhase : Phase
{
    public override void OnEnter()
    {
        base.OnEnter();

        var allTowers = TowerManager.Instance.GetTowersOfPlayer(CurrentPlayer);
        List<TowerInfo> deleteList = new List<TowerInfo>();

        foreach (var t in allTowers)
        {
            if(t.health <= 0)
            {
                deleteList.Add(t);
            }
        }

        for (int i = 0; i < deleteList.Count; i++)
        {
            TowerManager.Instance.DestroyTower(deleteList[i]);
        }
    }
}
