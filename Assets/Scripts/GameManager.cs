﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkLobbyManager
{
    public RoomBroadcaster broadcaster;

    public GamePlay[] gamePlayPrefabs;
    public TowerManager[] towerManagerPrefabs;
    public string[] gameLevels;

    public int SelectedGamePlay { get; set; }

    public int SelectedTowerManager { get; set; }

    private int selectedLevel = 0;
    public int SelectedLevel
    {
        get
        {
            return selectedLevel;
        }
        set
        {
            selectedLevel = value;
            playScene = gameLevels[value];
        }
    }

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

    void Start()
    {
        try
        {
            broadcaster.Initialize();
            broadcaster.StartAsClient();
        }
        catch
        {
            Debug.LogWarning("Broadcaster Failed.");
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

    public override void OnStartHost()
    {
        base.OnStartHost();

        try
        {
            broadcaster.Initialize();

            broadcaster.broadcastData = string.Concat(new object[]
            {
            "NetworkManager:",
            Network.player.ipAddress,
            ':',
            networkPort
            });

            broadcaster.StartAsServer();
        }
        catch
        {
            Debug.LogWarning("Broadcaster Failed.");
        }
    }

    void OnGUI()
    {
        if (!showLobbyGUI)
        {
            return;
        }

        GUI.depth = 1;

        string name = SceneManager.GetSceneAt(0).name;
        if (name != lobbyScene)
        {
            return;
        }

        int num = 10;
        int num2 = 40;
        bool flag = client == null || client.connection == null || client.connection.connectionId == -1;
        if (!IsClientConnected() && !NetworkServer.active)
        {
            if (flag)
            {
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Host"))
                    {
                        try
                        {
                            if (broadcaster.running)
                                broadcaster.StopBroadcast();
                        }
                        catch
                        {
                            Debug.LogWarning("Broadcaster Failed.");
                        }
                        StartHost();
                    }
                    num2 += 24;
                }
                if (GUI.Button(new Rect((float)num, (float)num2, 105f, 20f), "Join"))
                {
                    StartClient();
                }
                networkAddress = GUI.TextField(new Rect((float)(num + 100), (float)num2, 95f, 20f), networkAddress);
                num2 += 24;
            }
            else
            {
                GUI.Label(new Rect((float)num, (float)num2, 200f, 20f), string.Concat(new object[]
                {
                        "Connecting to ",
                        networkAddress,
                        ":",
                        networkPort,
                        ".."
                }));
                num2 += 24;
                if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Cancel Connection Attempt"))
                {
                    StopClient();
                }
            }
        }
        else
        {
            if (NetworkServer.active)
            {
                string text = "Server: port=" + networkPort;
                if (useWebSockets)
                {
                    text += " (Using WebSockets)";
                }
                GUI.Label(new Rect((float)num, (float)num2, 300f, 20f), text);
                num2 += 24;
            }
            if (IsClientConnected())
            {
                GUI.Label(new Rect((float)num, (float)num2, 300f, 20f), string.Concat(new object[]
                {
                        "Client: address=",
                        (Network.player.ipAddress == networkAddress) ? networkAddress : networkAddress + '(' + Network.player.ipAddress + ')',
                        " port=",
                        networkPort
                }));
                num2 += 24;
            }
        }

        if (NetworkServer.active || IsClientConnected())
        {
            if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Disconnect"))
            {
                try
                {
                    broadcaster.StopBroadcast();
                    broadcaster.Initialize();
                    broadcaster.StartAsClient();
                }
                catch
                {
                    Debug.LogWarning("Broadcaster Failed.");
                }
                StopHost();
            }
            num2 += 24;

            Rect position = new Rect(90f, 180f, 500f, 150f);
            GUI.Box(position, "Players:");
            broadcaster.showGUI = false;
        }
        else
        {
            if (null != broadcaster)
                broadcaster.showGUI = true;
        }


    }
}
