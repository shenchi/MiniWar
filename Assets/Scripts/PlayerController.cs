using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerController : NetworkBehaviour
{
    #region Client

    private Camera cam;

    public PlayerAgent CurrentPlayer { get; private set; }

    public enum State
    {
        Idle,
        SelectBuildingCoord,
        SelectAttactTarget,
    }

    private State state = State.Idle;

    public State CurrentState { get { return state; } }

    public bool Idle { get { return state == State.Idle; } }

    public bool ManualAttackEnabled { get; private set; }

    public int AttackCost { get; private set; }

    private int buildingTowerIndex;

    private TowerInfo attackerTower;
    private HexCoord attackerCoord;
    private HashSet<HexCoord> attackerRange;

    private int terrainMask;
    private int towerMask;

    void Awake()
    {
        terrainMask = LayerMask.GetMask("Terrain");
        towerMask = LayerMask.GetMask("Tower");
    }

    void OnDestroy()
    {
        EndControl();
    }

    public void StartControl(PlayerAgent player, bool manualAttack, int attackCost)
    {
        cam = FindObjectOfType<Camera>();
        CurrentPlayer = player;
        ManualAttackEnabled = manualAttack;
        AttackCost = attackCost;
    }

    public void EndControl()
    {
        SwitchTo(State.Idle);
        CurrentPlayer = null;
        ManualAttackEnabled = false;
        cam = null;
    }

    public void StartBuilding(int towerIndex)
    {
        if (!Idle && state != State.SelectBuildingCoord)
            return;

        if (TowerManager.Instance.GetTowerPrice(towerIndex) > CurrentPlayer.Resource)
            return;

        buildingTowerIndex = towerIndex;
        SwitchTo(State.SelectBuildingCoord);
    }

    void Update()
    {
        if (null == CurrentPlayer)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            switch (state)
            {
                case State.Idle:
                    {
                        if (ManualAttackEnabled && CurrentPlayer.Resource >= AttackCost)
                        {
                            RaycastHit hitInfo;
                            if (Physics.Raycast(ray, out hitInfo, float.MaxValue, towerMask))
                            {
                                var t = hitInfo.collider.GetComponent<TowerInfo>();
                                if (null != t && t.playerSlotId == CurrentPlayer.SlotId && 
                                    t.type == TowerType.AttackTower && t.state == TowerInfo.BuildingState.Working && t.attacked == false)
                                {
                                    attackerTower = t;
                                    attackerCoord = t.coord;
                                    attackerRange = RangeUtils.GetRangeOfTowerForLocalPlayerClient(t);
                                    SwitchTo(State.SelectAttactTarget);
                                }
                            }
                        }
                    }
                    break;
                case State.SelectBuildingCoord:
                    {
                        RaycastHit hitInfo;
                        if (Physics.Raycast(ray, out hitInfo, float.MaxValue, terrainMask))
                        {
                            var h = hitInfo.collider.GetComponent<Hexagon>();
                            if (null != h)
                            {
                                CmdTryBuildTower(CurrentPlayer.SlotId, buildingTowerIndex, h.coord);
                            }
                        }
                    }
                    break;
                case State.SelectAttactTarget:
                    {
                        if (ManualAttackEnabled)
                        {
                            RaycastHit hitInfo;
                            if (Physics.Raycast(ray, out hitInfo, float.MaxValue, towerMask))
                            {
                                var t = hitInfo.collider.GetComponent<TowerInfo>();
                                if (null != t && t.playerSlotId != CurrentPlayer.SlotId)
                                {
                                    if (attackerCoord != HexCoord.Invalid && attackerRange.Contains(t.coord))
                                        CmdTryAttackTower(CurrentPlayer.SlotId, attackerCoord, t.coord);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            SwitchTo(State.Idle);
        }
    }

    void SwitchTo(State newState)
    {
        if (newState == state)
            return;

        // exiting
        switch (state)
        {
            case State.SelectBuildingCoord:
                buildingTowerIndex = -1;
                break;
            case State.SelectAttactTarget:
                {
                    attackerTower.GetComponent<RangeIndicator>().enabled = false;
                    attackerTower.GetComponent<RangeIndicator>().RestoreMaterial();
                    attackerTower.GetComponent<TowerHover>().enabled = true;
                    attackerTower = null;
                    attackerCoord = HexCoord.Invalid;
                    attackerRange = null;
                }
                break;
            default:
                break;
        }

        state = newState;

        // enterting
        switch (state)
        {
            case State.SelectAttactTarget:
                {
                    if (null != attackerTower)
                    {
                        attackerTower.GetComponent<TowerHover>().enabled = false;
                        attackerTower.GetComponent<RangeIndicator>().TintColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                        attackerTower.GetComponent<RangeIndicator>().enabled = true;
                    }
                }
                break;
            default:
                break;
        }

    }

    [ClientRpc]
    void RpcBuildingSuccess(int towerIndex)
    {
        if (state == State.SelectBuildingCoord && buildingTowerIndex == towerIndex)
            SwitchTo(State.Idle);
    }

    [ClientRpc]
    void RpcAttackSuccess(HexCoord attackerCoord)
    {
        if (state == State.SelectAttactTarget && this.attackerCoord == attackerCoord)
            SwitchTo(State.Idle);
    }
    #endregion


    #region Server

    [Command]
    void CmdTryBuildTower(int playerSlot, int towerIndex, HexCoord coord)
    {
        var player = GamePlay.Instance.FindPlayerAgentBySlotId(playerSlot);
        var tower = TowerManager.Instance.BuildTower(player, coord, towerIndex);
        if (null != tower)
        {
            RpcBuildingSuccess(towerIndex);
        }
    }

    [Command]
    void CmdTryAttackTower(int playerSlot, HexCoord attackerCoord, HexCoord attackeeCoord)
    {
        var attacker = TowerManager.Instance.FindTowerByCoord(attackerCoord);
        if (attacker == null || attacker.type != TowerType.AttackTower || attacker.playerSlotId != playerSlot || attacker.state != TowerInfo.BuildingState.Working || attacker.attacked)
            return;

        var attackee = TowerManager.Instance.FindTowerByCoord(attackeeCoord);
        if (attackee == null || attackee.playerSlotId == playerSlot)
            return;

        var attackerPlayer = GamePlay.Instance.FindPlayerAgentBySlotId(attacker.playerSlotId);
        int attackCost = GamePlay.Instance.GetConstant<int>(GamePlay.ConstantName.AttackCost, 0);
        if (attackerPlayer.Resource < attackCost)
            return;

        var vision = TowerManager.Instance.GetHexagonsInVision(playerSlot);
        var range = RangeUtils.GetRangeOfTower(attacker, vision);

        if (range.Contains(attackee.coord))
        {
            attackee.health--;
            if (attackee.health <= 0)
            {
                TowerManager.Instance.DestroyTower(attackee);
            }
            attackerPlayer.AddResource(-attackCost);

            attacker.attacked = true;

            attackerPlayer.RpcAddLog("You attacked an enemy's building, resource is decreased by " + attackCost);
            RpcAttackSuccess(attackerCoord);
        }
    }
    #endregion
}
