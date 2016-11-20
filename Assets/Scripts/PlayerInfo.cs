using System;

[Serializable]
public struct PlayerInfo
{
    public int team;
    public int resource;
    public int production;
    public int cost;
    public HexCoord camp;
}
