using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameModeSelector : NetworkBehaviour
{
    [SyncVar(hook = "OnGamePlayChanged")]
    private int selectedGamePlay = 0;

    [SyncVar(hook = "OnTowerManagerChanged")]
    private int selectedTowerManager = 0;

    [SyncVar(hook = "OnLevelChanged")]
    private int selectedLevel = 0;

    private string[] gamePlayNames = null;
    private string[] towerManagerNames = null;
    private string[] levelNames = null;

    void OnGamePlayChanged(int value)
    {
        selectedGamePlay = value;

        GameManager mgr = NetworkManager.singleton as GameManager;
        mgr.SelectedGamePlay = value;
    }
    
    void OnTowerManagerChanged(int value)
    {
        selectedTowerManager = value;

        GameManager mgr = NetworkManager.singleton as GameManager;
        mgr.SelectedTowerManager = value;
    }

    void OnLevelChanged(int value)
    {
        selectedLevel = value;
        
        GameManager mgr = NetworkManager.singleton as GameManager;
        mgr.SelectedLevel = value;
    }

    void OnGUI()
    {
        GameManager mgr = NetworkManager.singleton as GameManager;
        if (null == gamePlayNames)
        {
            gamePlayNames = new string[mgr.gamePlayPrefabs.Length];
            for (int i = 0; i < mgr.gamePlayPrefabs.Length; i++)
            {
                gamePlayNames[i] = mgr.gamePlayPrefabs[i].name;
            }
        }

        if (null == towerManagerNames)
        {
            towerManagerNames = new string[mgr.towerManagerPrefabs.Length];
            for (int i = 0; i < mgr.towerManagerPrefabs.Length; i++)
            {
                towerManagerNames[i] = mgr.towerManagerPrefabs[i].name;
            }
        }

        if (null == levelNames)
        {
            levelNames = new string[mgr.gameLevels.Length];
            for (int i = 0; i < mgr.gameLevels.Length; i++)
            {
                levelNames[i] = mgr.gameLevels[i];
            }
        }

        Rect position = new Rect(90f, 340f, 500f, 30f);
        GUI.Label(position, "Game Mode:");
        position.y += 30;

        position.width = 120 * gamePlayNames.Length;
        int newSelected = GUI.SelectionGrid(position, selectedGamePlay, gamePlayNames, gamePlayNames.Length);
        if (isServer) selectedGamePlay = newSelected;

        //position.y += 40f;
        //position.width = 500f;
        //GUI.Label(position, "Tower Settings:");
        //position.y += 30;

        //position.width = 120 * towerManagerNames.Length;

        //newSelected = GUI.SelectionGrid(position, selectedTowerManager, towerManagerNames, towerManagerNames.Length);
        if (isServer) selectedTowerManager = newSelected;

        position.y += 40f;
        position.width = 500f;
        GUI.Label(position, "Level:");
        position.y += 30f;

        position.width = 120 * (levelNames.Length / 2);
        position.height = 60f;
        newSelected = GUI.SelectionGrid(position, selectedLevel, levelNames, (levelNames.Length / 2));
        if (isServer) selectedLevel = newSelected;
    }
}
