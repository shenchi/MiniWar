using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TowerInfo : NetworkBehaviour
{
    public TowerType type;

    [SyncVar]
    public int health;
    [SyncVar]
    public bool attacked = false;

    public TextMesh healthTagGP;
    public Image stateIcon;

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
    public HexCoord coord = HexCoord.Invalid;

    [SerializeField]
    private Renderer label;

    [SyncVar(hook = "OnSlotIdChanged")]
    public int playerSlotId = -1;

    [SyncVar(hook = "OnHealthGPChanged")]
    public float healthGP;

    public GameObject[] models;

    private void OnHealthGPChanged(float changedHealth)
    {
        healthGP = changedHealth;
        healthTagGP.text = changedHealth.ToString("F");
    }

    private void OnLabelColorChanged(Color value)
    {
        UpdateLabelColor(value);
    }

    public void UpdateLabelColor(Color value)
    {
        labelColor = value;
        if (null != label)
        {
            label.material.color = value;
        }

        if (null != models && models.Length > 1)
        {
            int selected = 0;


            if (value == Color.red)
            {
                selected = 0;
            }
            else
            {
                selected = 1;
            }

            for (int i = 0; i < models.Length; i++)
            {
                models[i].SetActive(i == selected);
            }
        }
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

    void Update()
    {
        if (null != stateIcon)
        {
            stateIcon.gameObject.SetActive(state == BuildingState.Building || attacked == true);
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
