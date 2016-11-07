using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameModeSelector : NetworkBehaviour
{
    [SyncVar(hook = "OnGamePlayChanged")]
    private int selectedGamePlay = 0;

    [SyncVar(hook = "OnTowerManagerChanged")]
    private int selectedTowerManager = 0;

    private string[] gamePlayNames = null;
    private string[] towerManagerNames = null;

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

        Rect position = new Rect(90f, 340f, 500f, 30f);
        position.width = 120 * gamePlayNames.Length;
        int newSelected = GUI.SelectionGrid(position, selectedGamePlay, gamePlayNames, 2);
        if (isServer) selectedGamePlay = newSelected;

        position.y += 40f;
        position.width = 120 * gamePlayNames.Length;

        newSelected = GUI.SelectionGrid(position, selectedTowerManager, towerManagerNames, 2);
        if (isServer) selectedTowerManager = newSelected;
    }
}
