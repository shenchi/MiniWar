using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkLobbyManager
{
    public GamePlay[] gamePlayPrefabs;
    public TowerManager[] towerManagerPrefabs;
    
    public int SelectedGamePlay { get; set; }

    public int SelectedTowerManager { get; set; }

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
                gamePlay = Instantiate(gamePlayPrefabs[SelectedGamePlay]);
                gamePlay.manager = this;
                gamePlay.InitPlayerList();
            }
            if (null == towerManager)
            {
                towerManager = Instantiate(towerManagerPrefabs[SelectedTowerManager]);
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

    void OnGUI()
    {
        if (!showLobbyGUI)
        {
            return;
        }
        string name = SceneManager.GetSceneAt(0).name;
        if (name != lobbyScene)
        {
            return;
        }
        Rect position = new Rect(90f, 180f, 500f, 150f);
        GUI.Box(position, "Players:");
        
    }
}
