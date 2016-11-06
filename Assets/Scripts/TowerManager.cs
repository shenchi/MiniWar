using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class TowerManager : NetworkBehaviour
{
    public static TowerManager Instance { get; private set; }

    public bool dontDestroyOnLoad = true;

    public TowerInfo[] towerList;

    public int defaultTower = -1;

    void Awake()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        for (int i = 0; i < towerList.Length; i++)
        {
            ClientScene.RegisterPrefab(towerList[i].gameObject);
        }
    }

    void OnEnable()
    {
        if (Instance != null)
            throw new System.Exception("TowerManager already exists");
        Instance = this;
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    public int GetTowerPrice(int index)
    {
        if (index < 0 || index > towerList.Length)
            return 0;
        return towerList[index].price;
    }

    #region Server

    private Dictionary<int, List<TowerInfo>> playerTowers = new Dictionary<int, List<TowerInfo>>();
    private Dictionary<HexCoord, TowerInfo> mapTowers = new Dictionary<HexCoord, TowerInfo>();

    public IEnumerable<TowerInfo> GetAllTowers()
    {
        return mapTowers.Values;
    }

    public List<TowerInfo> GetTowersOfPlayer(PlayerAgent player)
    {
        if (!playerTowers.ContainsKey(player.SlotId))
            return new List<TowerInfo>();
        return playerTowers[player.SlotId];
    }

    public TowerInfo BuildTower(PlayerAgent player, HexCoord coord, int index = -1)
    {
        if (index < 0)
            index = defaultTower;

        if (index < 0 || index >= towerList.Length)
            return null;

        int price = towerList[index].price;

        if (!playerTowers.ContainsKey(player.SlotId) && index == defaultTower)
        {
            price = 0;
        }

        if (player.Resource < price)
        {
            return null;
        }

        var vision = GetHexagonsInVision(player);
        vision.UnionWith(player.CampVision);
        if (!vision.Contains(coord))
        {
            return null;
        }

        if (mapTowers.ContainsKey(coord))
        {
            return null;
        }

        var tower = Instantiate(towerList[index]);
        NetworkServer.Spawn(tower.gameObject);
        
        if (!playerTowers.ContainsKey(player.SlotId))
            playerTowers.Add(player.SlotId, new List<TowerInfo>());

        playerTowers[player.SlotId].Add(tower);

        mapTowers.Add(coord, tower);
        
        tower.labelColor = player.PlayerColor;
        tower.coord = coord;
        tower.playerSlotId = player.SlotId;

        player.AddResource(-price);

        return tower;
    }

    public void DestroyTower(TowerInfo t)
    {
        if (!mapTowers.ContainsKey(t.coord) || mapTowers[t.coord] != t)
            return;
        if (!playerTowers.ContainsKey(t.playerSlotId))
            return;

        mapTowers.Remove(t.coord);
        playerTowers[t.playerSlotId].Remove(t);

        NetworkServer.Destroy(t.gameObject);
    }

    public List<TowerInfo> GetTowersOfType(PlayerAgent player, TowerType type)
    {
        List<TowerInfo> ret = new List<TowerInfo>();

        if (null != playerTowers && playerTowers.ContainsKey(player.SlotId))
        {
            foreach (var tower in playerTowers[player.SlotId])
            {
                if (tower.type == type)
                    ret.Add(tower);
            }
        }

        return ret;
    }

    public HashSet<HexCoord> GetHexagonsInRange(PlayerAgent player, TowerType type, Func<TowerInfo, int> rangeFunc)
    {
        HashSet<HexCoord> ret = new HashSet<HexCoord>();
        var towerList = GetTowersOfType(player, type);

        foreach (var t in towerList)
        {
            if (MapManager.Instance.Exists(t.coord))
            {
                ret.Add(t.coord);
            }

            var set = HexagonUtils.NeighborHexagons(t.coord, rangeFunc(t));
            set.RemoveWhere(x => { return !MapManager.Instance.Exists(x); });
            ret.UnionWith(set);
        }

        return ret;
    }

    public HashSet<HexCoord> GetHexagonsInRange(PlayerAgent player, TowerType type)
    {
        return GetHexagonsInRange(player, type, x => { return x.range; });
    }


    public HashSet<HexCoord> GetHexagonsInVision(PlayerAgent player)
    {
        return GetHexagonsInRange(player, TowerType.VisionTower, x => { return x.vision; });
    }


    public int SumAttribute(PlayerAgent player, TowerType type, Func<TowerInfo, int> attributeFunc)
    {
        int ret = 0;
        var towerList = GetTowersOfType(player, type);

        foreach (var t in towerList)
        {
            ret += attributeFunc(t);
        }

        return ret;
    }
    #endregion

}
