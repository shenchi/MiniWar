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

    public List<List<Phase>> phases;

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
    private int phase = -1;

    private Phase currentPhase = null;
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
            //NextPhase();
        }
    }

    void NextPhase()
    {
        if (round < 0)
            round = 0;

        if (phase < 0)
            phase = 0;
        else
            phase++;

        if (phase >= phases[round].Count)
        {
            phase = 0;
            round = (round + 1) % phases.Count;
        }

        currentPhase = phases[round][phase];
        timeElapsed = 0.0f;

        switch (currentPhase.type)
        {
            case PhaseType.ResourceProducing:
                break;
            case PhaseType.PlayerActing:
                break;
            case PhaseType.Consuming:
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (!isServer || null == currentPhase)
            return;

        timeElapsed += Time.deltaTime;
        if (timeElapsed > currentPhase.timeLimit)
        {
            NextPhase();
        }
    }

    #endregion

}
