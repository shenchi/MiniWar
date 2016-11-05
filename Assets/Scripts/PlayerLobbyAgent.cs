using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerLobbyAgent : NetworkLobbyPlayer
{
    public Color[] colorList;

    [SyncVar]
    public int playerColorIndex = -1;

    private GUIStyle style = new GUIStyle();

    [Command]
    void CmdNextColor()
    {
        NetworkLobbyManager networkLobbyManager = NetworkManager.singleton as NetworkLobbyManager;
        if (!networkLobbyManager)
        {
            return;
        }

        int newIndex = playerColorIndex;

        bool ok = true;
        do
        {
            newIndex = (newIndex + 1) % colorList.Length;

            ok = true;
            foreach (var player in networkLobbyManager.lobbySlots)
            {
                var agent = player as PlayerLobbyAgent;
                if (!agent)
                    continue;

                if (agent.slot != slot && agent.playerColorIndex == newIndex)
                {
                    ok = false;
                    break;
                }
            }
        } while (!ok);

        playerColorIndex = newIndex;
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        if (playerColorIndex < 0)
            CmdNextColor();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        if (isLocalPlayer)
        {
            GameManager networkLobbyManager = NetworkManager.singleton as GameManager;
            if (networkLobbyManager)
            {
                networkLobbyManager.LocalPlayerSlotId = slot;
            }
        }
    }

    void OnGUI()
    {
        if (!ShowLobbyGUI)
        {
            return;
        }

        NetworkLobbyManager networkLobbyManager = NetworkManager.singleton as NetworkLobbyManager;
        if (networkLobbyManager)
        {
            if (!networkLobbyManager.showLobbyGUI)
            {
                return;
            }
            string name = SceneManager.GetSceneAt(0).name;
            if (name != networkLobbyManager.lobbyScene)
            {
                return;
            }
        }

        Rect position = new Rect((float)(100 + slot * 100), 200f, 90f, 20f);

        GUI.Label(position, "Player " + slot);
        position.y += 25f;

        GUI.Label(position, readyToBegin ? "[Ready]" : "[Not Ready]");
        position.y += 25f;

        if (isLocalPlayer)
        {
            LocalPlayerGUI(position);
        }
        else
        {
            RemotePlayerGUI(position);
        }
    }

    void LocalPlayerGUI(Rect position)
    {
        style.normal.textColor = colorList[playerColorIndex];
        if (GUI.Button(position, "[Color]", style) && !readyToBegin)
        {
            CmdNextColor();
        }
        position.y += 25f;

        if (readyToBegin)
        {
            if (GUI.Button(position, "Stop"))
            {
                SendNotReadyToBeginMessage();
            }
        }
        else
        {
            if (GUI.Button(position, "Ready"))
            {
                SendReadyToBeginMessage();
            }
        }
        position.y += 25f;
    }

    void RemotePlayerGUI(Rect position)
    {
        style.normal.textColor = colorList[playerColorIndex];
        GUI.Label(position, "[Color]", style);
    }
}
