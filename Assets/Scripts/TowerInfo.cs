using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TowerInfo : NetworkBehaviour
{
    public TowerType type;

    [SyncVar]
    public int health;
    public float healthGP;

    public int level;
    public int vision;
    public int range;
    public int price;
    public int cost;

    public enum BuildingState
    {
        Building,
        Working,
        OutOfVision
    }

    [SyncVar]
    public BuildingState state = BuildingState.Building;

    [SyncVar(hook = "OnLabelColorChanged")]
    public Color labelColor;

    [SyncVar(hook = "OnCoordChanged")]
    public HexCoord coord;

    [SerializeField]
    private Renderer label;

    [SyncVar(hook = "OnSlotIdChanged")]
    public int playerSlotId = -1;

    private void OnLabelColorChanged(Color value)
    {
        labelColor = value;
        label.material.color = value;
    }

    private void OnCoordChanged(HexCoord coord)
    {
        this.coord = coord;
        transform.position = MapManager.Instance.GetMountPosition(coord);
    }

    private void OnSlotIdChanged(int slotId)
    {
        if (playerSlotId != slotId)
        {
            if (playerSlotId == VisionController.Instance.LocalPlayerSlotId)
            {
                playerSlotId = slotId;
                VisionController.Instance.RemoveTower(this);
            }
            if (slotId == VisionController.Instance.LocalPlayerSlotId)
            {
                playerSlotId = slotId;
                VisionController.Instance.AddTower(this);
            }
        }
    }

    void OnDestroy()
    {
        if (null != VisionController.Instance && playerSlotId == VisionController.Instance.LocalPlayerSlotId)
        {
            VisionController.Instance.RemoveTower(this);
        }
    }
}
