using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// just for local vision
/// </summary>
public class VisionController : MonoBehaviour
{
    public static VisionController Instance { get; private set; }

    public int LocalPlayerSlotId { get; set; }
    public HashSet<HexCoord> Vision { get; private set; }

    private HashSet<HexCoord> campVision;
    private HashSet<TowerInfo> visionTowers = new HashSet<TowerInfo>();

    void Awake()
    {
        Vision = new HashSet<HexCoord>();
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public void SetCampVision(HashSet<HexCoord> v)
    {
        campVision = v;
    }

    public void AddTower(TowerInfo t)
    {
        if (t.type != TowerType.VisionTower)
            return;

        if (!visionTowers.Contains(t))
            visionTowers.Add(t);

        var v = HexagonUtils.NeighborHexagons(t.coord, t.vision);
        v.Add(t.coord);
        v.RemoveWhere(x => { return !MapManager.Instance.Exists(x); });
        Vision.UnionWith(v);
    }

    public void RemoveTower(TowerInfo t)
    {
        if (visionTowers.Contains(t))
        {
            visionTowers.Remove(t);
            RecalcVision();
        }
    }

    public void RecalcVision()
    {
        Vision.Clear();
        Vision.UnionWith(campVision);

        foreach (var t in visionTowers)
        {
            AddTower(t);
        }
    }

    public bool InVision(HexCoord coord)
    {
        return Vision.Contains(coord);
    }
}
