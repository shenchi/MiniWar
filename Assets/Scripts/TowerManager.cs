using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

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

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    #region Server

    private Dictionary<int, List<TowerInfo>> playerTowers = null;

    public TowerInfo BuildTower(PlayerAgent player, int index = -1)
    {
        if (index < 0)
            index = defaultTower;

        if (index < 0 || index >= towerList.Length)
            return null;

        var tower = Instantiate(towerList[index]);
        NetworkServer.Spawn(tower.gameObject);

        tower.player = player;
        tower.labelColor = player.PlayerColor;

        if (null == playerTowers)
            playerTowers = new Dictionary<int, List<TowerInfo>>();

        if (!playerTowers.ContainsKey(player.SlotId))
            playerTowers.Add(player.SlotId, new List<TowerInfo>());

        playerTowers[player.SlotId].Add(tower);

        return tower;
    }


    public TowerInfo BuildTower(PlayerAgent player, HexCoord coord, int index = -1)
    {
        var tower = BuildTower(player, index);
        tower.coord = coord;
        return tower;
    }
    #endregion
    
    //public override void OnStartClient()
    //{
    //    if (isLocalPlayer)
    //    {
    //        for (int i = 0; i < towerList.Length; i++)
    //        {
    //            ClientScene.RegisterPrefab(towerList[i].gameObject);
    //        }
    //    }
    //}
}
