using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    #region Client
    public PlayerAgent CurrentPlayer { get; private set; }

    public enum State
    {
        Idle,
        SelectBuildingCoord,
    }

    private State state = State.Idle;

    public State CurrentState { get { return state; } }

    public bool Idle { get { return state == State.Idle; } }

    private int buildingTowerIndex;

    private int terrainMask;

    void Awake()
    {
        terrainMask = LayerMask.GetMask("Terrain");
    }

    void OnDestroy()
    {
        EndControl();
    }

    public void StartControl(PlayerAgent player)
    {
        CurrentPlayer = player;
    }

    public void EndControl()
    {
        CurrentPlayer = null;
        state = State.Idle;
    }

    public void StartBuilding(int towerIndex)
    {
        if (!Idle && state != State.SelectBuildingCoord)
            return;

        if (TowerManager.Instance.GetTowerPrice(towerIndex) > CurrentPlayer.Resource)
            return;

        buildingTowerIndex = towerIndex;
        state = State.SelectBuildingCoord;
    }

    void Update()
    {
        if (state == State.SelectBuildingCoord)
        {
            if (Input.GetMouseButton(0))
            {
                Camera cam = FindObjectOfType<Camera>();
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, float.MaxValue, terrainMask))
                {
                    Hexagon h = hitInfo.collider.GetComponent<Hexagon>();
                    if (null != h)
                    {
                        CmdTryBuildTower(CurrentPlayer.SlotId, buildingTowerIndex, h.coord);
                    }
                }
            }
        }
    }

    [ClientRpc]
    void RpcBuildingSuccess()
    {
        state = State.Idle;
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
            RpcBuildingSuccess();
        }
    }
    #endregion
}
