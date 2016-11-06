using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class GamePlay : NetworkBehaviour
{

    public enum PhaseType
    {
        ResourceProducing,
        PlayerActing,
        Consuming,
    }

    [Serializable]
    public class Phase
    {
        public PhaseType type;
        public int timeLimit = 0;
    }

    [Serializable]
    public class Round
    {
        public List<Phase> phaseList;
        public int Count { get { return phaseList.Count; } }
        public Phase this[int index]
        {
            get
            {
                return phaseList[index];
            }
        }
    }

    public List<Round> phases;

    #region Client

    public static GamePlay Instance { get; private set; }

    void OnEnable()
    {
        if (Instance)
            throw new System.Exception("GamePlay already exists!");
        Instance = this;
    }

    #endregion

    #region Server

    public GameManager manager { get; set; }

    private List<int> playerList;
    private Dictionary<int, PlayerAgent> playerTable;

    private int round = -1;
    private int player = -1;
    private int phase = -1;

    private Phase currentPhase = null;
    private PlayerAgent currentPlayer = null;
    private float timeElapsed = 0.0f;

    public void InitPlayerList()
    {
        playerList = new List<int>();
        foreach (var slot in manager.lobbySlots)
        {
            var player = slot as PlayerLobbyAgent;
            if (player)
            {
                playerList.Add(player.slot);
            }
        }
    }

    public void RegisterPlayer(PlayerAgent player)
    {
        if (null == playerTable)
            playerTable = new Dictionary<int, PlayerAgent>();

        bool inList = false;

        foreach (int slotId in playerList)
        {
            if (slotId == player.SlotId)
            {
                inList = true;
                break;
            }
        }

        if (!inList || playerTable.ContainsKey(player.SlotId))
        {
            return;
        }

        playerTable.Add(player.SlotId, player);

        if (playerList.Count == playerTable.Count)
        {
            NextPhase();
        }
    }

    void NextPhase()
    {
        if (round < 0)
            round = 0;

        if (player < 0)
            player = 0;

        if (phase < 0)
            phase = 0;
        else
            phase++;

        if (phase >= phases[round].Count)
        {
            phase = 0;
            player++;
        }

        if (player >= playerList.Count)
        {
            player = 0;
            round = (round + 1) % phases.Count;
        }

        currentPhase = phases[round][phase];
        currentPlayer = playerTable[playerList[player]];
        timeElapsed = 0.0f;

        switch (currentPhase.type)
        {
            case PhaseType.ResourceProducing:
                CollectResources();
                break;
            case PhaseType.PlayerActing:

                break;
            case PhaseType.Consuming:
                DeductMaintenanceCost();
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (null == currentPhase)
            return;

        timeElapsed += Time.deltaTime;
        if (timeElapsed > currentPhase.timeLimit)
        {
            NextPhase();
        }
    }

    protected PlayerAgent CurrentPlayer { get { return currentPlayer; } }

    protected virtual void CollectResources()
    {
        var hexes = TowerManager.Instance.GetHexagonsInRange(CurrentPlayer, TowerType.ResourceTower);
        CurrentPlayer.AddResource(hexes.Count);
    }

    protected virtual void DeductMaintenanceCost()
    {
        int res =
            TowerManager.Instance.SumAttribute(CurrentPlayer, TowerType.ResourceTower, x => { return x.cost; }) +
            TowerManager.Instance.SumAttribute(CurrentPlayer, TowerType.VisionTower, x => { return x.cost; }) +
            TowerManager.Instance.SumAttribute(CurrentPlayer, TowerType.AttackTower, x => { return x.cost; });

        CurrentPlayer.AddResource(-res);
    }

    #endregion

}
