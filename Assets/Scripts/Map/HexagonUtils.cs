using UnityEngine;
using System.Collections.Generic;

public class HexagonUtils
{
    public const int MaxHeight = 7;
    public const float Sqrt_3 = 1.732050807568877f;
    public const float ColSpace = Sqrt_3 * 0.5f;
    public const float RowSpace = 1.5f * 0.5f;

    public static readonly HexCoord[] DeltaCoords =
    {
        new HexCoord() { x = 0, y = 1 },
        new HexCoord() { x = 1, y = 1 },
        new HexCoord() { x = 1, y = 0 },
        new HexCoord() { x = 0, y = -1 },
        new HexCoord() { x = -1, y = -1 },
        new HexCoord() { x = -1, y = 0 },
    };

    public static HexCoord Pos2Coord(Vector3 position)
    {
        HexCoord coord = new HexCoord();
        float x = position.x;
        float z = position.z;

        coord.gridX = Mathf.FloorToInt(x / RowSpace + 0.5f);
        coord.gridY = coord.NeedOffset ? (Mathf.FloorToInt(z / ColSpace)) : (Mathf.FloorToInt(z / ColSpace + 0.5f));

        return coord;
    }

    public static Vector3 Coord2Pos(HexCoord coord)
    {
        float x = coord.gridX * RowSpace;
        float z = coord.NeedOffset ? ((coord.gridY + 0.5f) * ColSpace) : (coord.gridY * ColSpace);

        return new Vector3(x, 0, z);
    }

    public static HashSet<HexCoord> NeighborHexagons(HexCoord coord, int range = 1)
    {
        var ret = new HashSet<HexCoord>();

        if (range < 1)
            return ret;

        for (int i = 0; i < DeltaCoords.Length; i++)
        {
            HexCoord c = coord.Add(DeltaCoords[i]);
            ret.Add(c);
            if (range > 1)
            {
                ret.UnionWith(NeighborHexagons(c, range - 1));
            }
        }
        return ret;
    }
}
