using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class GamePlay : NetworkBehaviour
{

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
    
    public static GamePlay Instance { get; private set; }

    void OnEnable()
    {
        if (Instance)
            throw new System.Exception("GamePlay already exists!");
        Instance = this;
    }

    void OnDisable()
    {
        if (this == Instance)
            Instance = null;
    }
    
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

    public PlayerAgent FindPlayerAgentBySlotId(int slotId)
    {
        if (playerTable.ContainsKey(slotId))
            return playerTable[slotId];
        return null;
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
        currentPhase.CurrentPlayer = currentPlayer;
        timeElapsed = 0.0f;

        currentPhase.OnEnter();
    }

    void Update()
    {
        if (null == currentPhase)
            return;

        timeElapsed += Time.deltaTime;
        if (timeElapsed > currentPhase.timeLimit)
        {
            currentPhase.OnExit();
            NextPhase();
        }
        else
        {
            currentPhase.OnTick();
        }
    }

    #endregion

}
