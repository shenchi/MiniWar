using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    #region Client

    public Material ghostMat;

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


    private GameObject ghostTower = null;
    private int ghostTowerIndex = -1;
    private Material ghostTowerMat = null;

    public Color buildingAllowed;
    public Color buildingForbiden;

    private TowerInfo selectedTower;
    private HexCoord towerCoord;
    private HashSet<HexCoord> towerRange;

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
        UIController.Instance.HideActionPanel();
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

        GetGhostTower(buildingTowerIndex);
        ghostTowerMat.color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        ghostTower.SetActive(false);
    }

    public GameObject GetGhostTower(int towerIndex)
    {
        if (null == ghostTower || ghostTowerIndex != towerIndex)
        {
            if (null != ghostTower)
            {
                Destroy(ghostTower);
            }

            ghostTower = Instantiate(TowerManager.Instance.towerList[(int)towerIndex]).gameObject;

            var towerInfo = ghostTower.GetComponent<TowerInfo>();
            towerInfo.stateIcon.gameObject.SetActive(false);
            towerInfo.stateIcon = null;
            towerInfo.UpdateLabelColor(CurrentPlayer.PlayerColor);

            var culler = ghostTower.GetComponentsInChildren<CullByVision>();
            foreach (var c in culler)
            {
                Destroy(c);
            }
            var hover = ghostTower.GetComponentsInChildren<TowerHover>();
            foreach (var h in hover)
            {
                Destroy(h);
            }

            ghostTower.layer = LayerMask.NameToLayer("Ghost");
            
            var renderers = ghostTower.GetComponentsInChildren<Renderer>();
            ghostTowerMat = new Material(ghostMat);

            foreach (var r in renderers)
            {
                r.enabled = true;
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = ghostTowerMat;
                }
                r.sharedMaterials = mats;
            }
        }
        return ghostTower;
    }

    private HexCoord lastHoveringCoord = HexCoord.Invalid;

    void Update()
    {
        if (null == CurrentPlayer)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && FindObjectOfType<EventSystem>().currentSelectedGameObject == null)
        {
            switch (state)
            {
                case State.Idle:
                    {
                        selectedTower = null;

                        RaycastHit hitInfo;
                        if (Physics.Raycast(ray, out hitInfo, float.MaxValue, towerMask))
                        {
                            var t = hitInfo.collider.GetComponent<TowerInfo>();
                            if (null != t && t.playerSlotId == CurrentPlayer.SlotId && VisionController.Instance.InVision(t.coord))
                            {
                                selectedTower = t;
                                towerCoord = t.coord;
                                towerRange = RangeUtils.GetRangeOfTowerForLocalPlayerClient(t);
                                if (t.type == TowerType.AttackTower && t.attacked == false && ManualAttackEnabled)
                                {
                                    UIController.Instance.ShowActionPanel(hitInfo.point, new string[] { "A", "X" }, new UnityAction[] { SelectAttackee, DestroyTower });
                                }
                                else
                                {
                                    UIController.Instance.ShowActionPanel(hitInfo.point, new string[] { "X" }, new UnityAction[] { DestroyTower });
                                }
                            }
                        }

                        if (null == selectedTower)
                        {
                            UIController.Instance.HideActionPanel();
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
                                    if (towerCoord != HexCoord.Invalid && towerRange.Contains(t.coord))
                                        CmdTryAttackTower(CurrentPlayer.SlotId, towerCoord, t.coord);
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
        else
        {
            if (state == State.SelectBuildingCoord)
            {
                HexCoord coord = HexCoord.Invalid;

                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, float.MaxValue, terrainMask))
                {
                    var h = hitInfo.collider.GetComponent<Hexagon>();

                    if (null != h)
                        coord = h.coord;
                }

                if (coord != lastHoveringCoord)
                {
                    if (coord != HexCoord.Invalid)
                    {
                        ghostTower.SetActive(true);
                        ghostTowerMat.color = buildingAllowed;
                        ghostTower.transform.position = MapManager.Instance.GetMountPosition(coord);

                        if (!VisionController.Instance.InVision(coord))
                        {
                            ghostTowerMat.color = buildingForbiden;
                        }
                        else
                        {
                            var allTowers = FindObjectsOfType<TowerInfo>();
                            for (int i = 0; i < allTowers.Length; ++i)
                            {
                                if (allTowers[i].coord == coord)
                                {
                                    ghostTowerMat.color = buildingForbiden;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ghostTower.SetActive(false);
                    }

                    lastHoveringCoord = coord;
                }
            }
        }
    }

    void SelectAttackee()
    {
        if (null == selectedTower)
            return;

        if (selectedTower.type == TowerType.AttackTower && selectedTower.state == TowerInfo.BuildingState.Working && selectedTower.attacked == false)
        {
            SwitchTo(State.SelectAttactTarget);
        }

        UIController.Instance.HideActionPanel();
    }

    void DestroyTower()
    {
        CmdTryDestroyTower(CurrentPlayer.SlotId, towerCoord);
        UIController.Instance.HideActionPanel();
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
                    selectedTower.GetComponent<RangeIndicator>().enabled = false;
                    selectedTower.GetComponent<RangeIndicator>().RestoreMaterial();
                    selectedTower.GetComponent<TowerHover>().enabled = true;
                    selectedTower = null;
                    towerCoord = HexCoord.Invalid;
                    towerRange = null;
                }
                break;
            default:
                break;
        }

        state = newState;

        // enterting
        switch (state)
        {
            case State.Idle:
                if (null != ghostTower)
                    Destroy(ghostTower);
                break;
            case State.SelectAttactTarget:
                {
                    if (null != selectedTower)
                    {
                        selectedTower.GetComponent<TowerHover>().enabled = false;
                        selectedTower.GetComponent<RangeIndicator>().TintColor = buildingForbiden;
                        selectedTower.GetComponent<RangeIndicator>().enabled = true;
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
        if (state == State.SelectAttactTarget && this.towerCoord == attackerCoord)
            SwitchTo(State.Idle);
    }

    [ClientRpc]
    void RpcDestroySuccess()
    {

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

    [Command]
    void CmdTryDestroyTower(int playerSlot, HexCoord coord)
    {
        var tower = TowerManager.Instance.FindTowerByCoord(coord);
        if (null == tower || tower.playerSlotId != playerSlot)
            return;

        TowerManager.Instance.DestroyTower(tower);
        RpcDestroySuccess();
    }
    #endregion
}
