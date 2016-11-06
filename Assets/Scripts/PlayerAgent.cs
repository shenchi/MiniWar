using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

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

    private HashSet<HexCoord> campVision = new HashSet<HexCoord>();
    private HexCoord campCoord = new HexCoord() { x = int.MaxValue, y = int.MaxValue };

    public HashSet<HexCoord> CampVision { get { return campVision; } }

    public int Resource { get { return playerInfo.resource; } }

    #region Server

    [Command]
    void CmdInitPlayerData(int slotId, Color color)
    {
        this.slotId = slotId;
        playerColor = color;

        StartCoroutine(
            WaitForAndThen(
                delegate ()
                {
                    return (GamePlay.Instance != null && MapManager.Instance != null && TowerManager.Instance != null);
                },
                delegate ()
                {
                    playerInfo = new PlayerInfo()
                    {
                        team = slotId,
                        resource = 0,
                        camp = MapManager.Instance.GetStartPointCoord(slotId)
                    };
                    TowerManager.Instance.BuildTower(this, playerInfo.camp);
                    GamePlay.Instance.RegisterPlayer(this);
                }
                )
            );
    }

    [Command]
    void CmdFinishCurrentPhase()
    {
        GamePlay.Instance.FinishCurrentPhase();
    }

    public void AddResource(int res)
    {
        var temp = playerInfo;
        temp.resource += res;
        playerInfo = temp;
    }

    #endregion

    #region Client

    private PlayerController playerController = null;
    private VisionController visionController = null;

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
            visionController = gameObject.AddComponent<VisionController>();
            visionController.LocalPlayerSlotId = slotId;

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

            playerController = gameObject.GetComponent<PlayerController>();
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

        if (campCoord != playerInfo.camp)
        {
            campCoord = playerInfo.camp;
            UpdateCampVision();
        }

        if (isLocalPlayer)
        {
            UIController.Instance.SetResource(playerInfo.resource);
        }
    }

    void UpdateCampVision()
    {
        campVision = new HashSet<HexCoord>();
        campVision.Add(campCoord);
        campVision.UnionWith(HexagonUtils.NeighborHexagons(campCoord, 1));
        campVision.RemoveWhere(x => { return !MapManager.Instance.Exists(x); });

        if (isLocalPlayer)
        {
            visionController.SetCampVision(campVision);
            visionController.RecalcVision();
        }
    }

    [ClientRpc]
    public void RpcStartOperationMode()
    {
        if (!isLocalPlayer)
            return;

        playerController.StartControl(this);
        UIController.Instance.ClearButtonActions();

        UIController.Instance.RegisterButtonAction("Resource", delegate () { playerController.StartBuilding(0); });
        UIController.Instance.RegisterButtonAction("Vision", delegate () { playerController.StartBuilding(1); });
        UIController.Instance.RegisterButtonAction("Attack", delegate () { playerController.StartBuilding(2); });
        UIController.Instance.RegisterButtonAction("CancelBuilding", delegate () { CmdFinishCurrentPhase(); });

        UIController.Instance.EnableBuildingPanel = true;
        UIController.Instance.RemainingTime = 1.0f;
    }

    [ClientRpc]
    public void RpcSetOperationModeRemainingTime(float remainingTime)
    {
        if (!isLocalPlayer)
            return;

        UIController.Instance.RemainingTime = remainingTime;
    }

    [ClientRpc]
    public void RpcEndOperationMode()
    {
        if (!isLocalPlayer)
            return;

        UIController.Instance.ClearButtonActions();
        playerController.EndControl();
        UIController.Instance.EnableBuildingPanel = false;
    }


    #endregion
}
