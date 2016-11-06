using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class PlayerAgent : NetworkBehaviour
{

    IEnumerator WaitForAndThen(Func<bool> condition, Action action)
    {
        while (!condition())
        {
            yield return null;
        }
        action();
    }

    [SyncVar]
    private int slotId;

    public int SlotId { get { return slotId; } }

    [SyncVar]
    private Color playerColor;
    public Color PlayerColor { get { return playerColor; } }

    [SyncVar(hook = "OnPlayerInfoUpdate")]
    private PlayerInfo playerInfo;

    #region Server

    [Command]
    void CmdInitPlayerData(int slotId, Color color)
    {
        this.slotId = slotId;
        playerColor = color;

        playerInfo = new PlayerInfo()
        {
            team = slotId,
            resource = 0,
        };

        StartCoroutine(
            WaitForAndThen(
                delegate ()
                {
                    return (GamePlay.Instance != null && MapManager.Instance != null && TowerManager.Instance != null);
                },
                delegate ()
                {
                    playerInfo.camp = MapManager.Instance.GetStartPointCoord(slotId);
                    TowerManager.Instance.BuildTower(this, playerInfo.camp);
                    GamePlay.Instance.RegisterPlayer(this);
                }
                )
            );
    }

    public void AddResource(int res)
    {
        var temp = playerInfo;
        temp.resource += res;
        playerInfo = temp;
    }

    #endregion

    #region Client

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (isLocalPlayer)
        {
            GameManager mgr = NetworkManager.singleton as GameManager;
            int slotId = -1;
            if (mgr)
            {
                slotId = mgr.LocalPlayerSlotId;
            }

            UIController.Instance.EnableUI = true;

            foreach (var slot in mgr.lobbySlots)
            {
                var agent = slot as PlayerLobbyAgent;
                if (!agent)
                    continue;
                if (agent.slot == slotId)
                {
                    CmdInitPlayerData(agent.slot, agent.colorList[agent.playerColorIndex]);
                    break;
                }
            }


        }
    }

    public override void OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        if (isLocalPlayer)
        {
            UIController.Instance.EnableUI = false;
        }
    }

    void OnPlayerInfoUpdate(PlayerInfo value)
    {
        playerInfo = value;
        if (isLocalPlayer)
        {
            UIController.Instance.SetResource(playerInfo.resource);
        }
    }

    #endregion
}
