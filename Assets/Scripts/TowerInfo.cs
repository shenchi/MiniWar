using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TowerInfo : NetworkBehaviour
{
    public int health;
    public int level;
    public int vision;
    public int range;
    public int price;
    public int cost;

    [SyncVar(hook = "OnLabelColorChanged")]
    public Color labelColor;

    [SyncVar(hook = "OnCoordChanged")]
    public HexCoord coord;

    [SerializeField]
    private Renderer label;

    public PlayerAgent player { get; set; }

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
}
