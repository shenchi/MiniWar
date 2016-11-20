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

    public PlayerInfo PlayerInfo { get { return playerInfo; } }

    public enum State
    {
        Unregistered,
        Ready,
        InGame,
        Lose,
        Win,
    }

    [SyncVar(hook = "OnGameStateChanged")]
    public State GameState = State.Unregistered;

    private HashSet<HexCoord> campVision = new HashSet<HexCoord>();
    private HexCoord campCoord = new HexCoord() { x = int.MaxValue, y = int.MaxValue };

    public HashSet<HexCoord> CampVision { get { return campVision; } }

    public int Resource { get { return playerInfo.resource; } }

    public int Production { get { return playerInfo.production; } }

    public int Cost { get { return playerInfo.cost; } }

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

    public void SetProduction(int prod)
    {
        var temp = playerInfo;
        temp.production = prod;
        playerInfo = temp;
    }
    
    public void SetCost(int cost)
    {
        var temp = playerInfo;
        temp.cost = cost;
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

            UIController.Instance.EnableInGameUI = true;
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
            if (null != UIController.Instance)
                UIController.Instance.EnableInGameUI = false;
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
            UIController.Instance.SetProduction(playerInfo.production);
            UIController.Instance.SetCost(playerInfo.cost);
        }
    }

    void UpdateCampVision()
    {
        campVision = HexagonUtils.NeighborHexagons(campCoord, 1);
        campVision.Add(campCoord);
        MapManager.Instance.RemoveHexagonsNotExists(campVision);

        if (isLocalPlayer)
        {
            visionController.SetCampVision(campVision);
            visionController.RecalcVision();
        }
    }

    void OnGameStateChanged(State newState)
    {
        if (GameState == newState)
            return;

        // exit
        switch (GameState)
        {
            default:
                break;
        }

        GameState = newState;

        // enter
        switch (GameState)
        {
            case State.InGame:
                if (isLocalPlayer)
                {
                    UIController.Instance.ClearButtonActions();

                    var towerTemplates = TowerManager.Instance.towerList;
                    for (int i = 0; i < towerTemplates.Length; i++)
                    {
                        UIController.Instance.SetBuildButtonText(i, towerTemplates[i].type.ToString() + "\nPrice: " +
                            towerTemplates[i].price + "\nCost: " + towerTemplates[i].cost);

                        UIController.Instance.RegisterButtonAction("BuildButton" + i,
                            delegate (string t)
                            {
                                int idx = int.Parse(t.Replace("BuildButton", string.Empty));
                                playerController.StartBuilding(idx);
                            }
                            );
                    }

                    UIController.Instance.RegisterButtonAction("CancelBuilding", delegate (string t) { CmdFinishCurrentPhase(); });
                }
                break;
            case State.Win:
                if (isLocalPlayer)
                {
                    UIController.Instance.EnableInGameUI = false;
                    UIController.Instance.ShowWin(true);
                }
                break;
            case State.Lose:
                if (isLocalPlayer)
                {
                    UIController.Instance.EnableInGameUI = false;
                    UIController.Instance.ShowLose(true);
                }
                break;
            default:
                break;
        }
    }

    [ClientRpc]
    public void RpcStartOperationMode(bool manualAttack)
    {
        if (!isLocalPlayer)
            return;

        playerController.StartControl(this, manualAttack);

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

        playerController.EndControl();
        UIController.Instance.EnableBuildingPanel = false;
    }


    #endregion
}
