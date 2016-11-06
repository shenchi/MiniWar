using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeUtils
{

    public static HashSet<HexCoord> GetRange(HexCoord center, int range, HashSet<HexCoord> vision)
    {
        var ret = HexagonUtils.NeighborHexagons(center, range);
        ret.IntersectWith(vision);
        MapManager.Instance.RemoveHexagonsNotExists(ret);
        return ret;
    }

    public static HashSet<HexCoord> GetRangeOfTower(TowerInfo tower, HashSet<HexCoord> vision)
    {
        var range = HexagonUtils.NeighborHexagons(tower.coord, tower.range);
        range.IntersectWith(vision);
        MapManager.Instance.RemoveHexagonsNotExists(range);
        return range;
    }

    public static HashSet<HexCoord> GetRangeClient(HexCoord center, int range)
    {
        return GetRange(center, range, VisionController.Instance.Vision);
    }

    public static HashSet<HexCoord> GetRangeOfTowerForLocalPlayerClient(TowerInfo tower)
    {
        return GetRangeOfTower(tower, VisionController.Instance.Vision);
    }

    public static HashSet<HexCoord> GetPlayerVisionServer(PlayerAgent player)
    {
        var vision = TowerManager.Instance.GetHexagonsInVision(player.SlotId);
        vision.UnionWith(player.CampVision);
        return vision;
    }
}
