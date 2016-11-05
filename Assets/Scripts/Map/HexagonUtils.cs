using UnityEngine;
using System.Collections;

public class HexagonUtils
{
    public const int MaxHeight = 7;
    public const float Sqrt_3 = 1.732050807568877f;
    public const float ColSpace = Sqrt_3 * 0.5f;
    public const float RowSpace = 1.5f * 0.5f;

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
}
