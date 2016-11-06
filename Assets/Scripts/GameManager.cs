using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : NetworkLobbyManager
{
    public GamePlay gamePlayPrefab;
    public TowerManager towerManagerPrefab;

    private GamePlay gamePlay = null;
    private TowerManager towerManager = null;

    public int LocalPlayerSlotId { get; set; }
    
    public override void OnLobbyServerSceneChanged(string sceneName)
    {
        base.OnLobbyServerSceneChanged(sceneName);
        if (sceneName != "Main")
        {
            if (null == gamePlay)
            {
                gamePlay = Instantiate(gamePlayPrefab);
                gamePlay.manager = this;
                gamePlay.InitPlayerList();
            }
            if (null == towerManager)
            {
                towerManager = Instantiate(towerManagerPrefab);
                NetworkServer.Spawn(towerManager.gameObject);
            }
        }
        else
        {
            gamePlay = null;
            towerManager = null;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        var objs = GameObject.FindGameObjectsWithTag("DestroyOnRestart");
        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }
    }
}
