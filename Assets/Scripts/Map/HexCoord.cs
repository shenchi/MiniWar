using System;

[Serializable]
public struct HexCoord
{
    public int x;
    public int y;

    public int gridX
    {
        get
        {
            return x;
        }
        set
        {
            int gy = gridY;
            x = value;
            gridY = gy;
        }
    }

    public int gridY
    {
        get
        {
            int halfX = x >> 1;
            return NeedOffset ? y - halfX - 1 : y - halfX;
        }
        set
        {
            int halfX = x >> 1;
            y = NeedOffset ? value + halfX + 1 : value + halfX;
        }
    }

    public bool NeedOffset
    {
        get
        {
            return (x & 1) == 1;
        }
    }

    public HexCoord Add(HexCoord c)
    {
        return new HexCoord() { x = x + c.x, y = y + c.y };
    }

    public override bool Equals(object obj)
    {
        return obj is HexCoord && (HexCoord)obj == this;
    }

    public static bool operator ==(HexCoord a, HexCoord b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(HexCoord a, HexCoord b)
    {
        return !(a == b);
    }

}
