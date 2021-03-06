﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class GamePlay : NetworkBehaviour
{

    [Serializable]
    public struct Round
    {
        public bool once;
        public bool forEachPlayer;
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

    private List<Round> runtimePhases;

    public static GamePlay Instance { get; private set; }

    void OnEnable()
    {
        if (Instance)
            throw new System.Exception("GamePlay already exists!");

        runtimePhases = new List<Round>();
        for (int r = 0; r < phases.Count; r++)
        {
            var round = phases[r];
            round.phaseList = new List<Phase>();
            for (int p = 0; p < phases[r].Count; p++)
            {
                var phase = Instantiate(phases[r][p]);
                phase.transform.parent = transform;
                round.phaseList.Add(phase);
            }
            runtimePhases.Add(round);
        }

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

    private HashSet<int> finishCurrentPhase = new HashSet<int>();
    private bool finishGame = false;

    public enum ConstantName
    {
        AttackCost = 0,
        ConstantCount
    }

    private Dictionary<ConstantName, object> constants = new Dictionary<ConstantName, object>();

    public void SetConstant<T>(ConstantName constantName, T value)
    {
        if (!constants.ContainsKey(constantName))
            constants.Add(constantName, value);
        else
            constants[constantName] = value;
    }

    public T GetConstant<T>(ConstantName constantName, T defaultValue)
    {
        if (constants.ContainsKey(constantName))
        {
            return (T)constants[constantName];
        }
        return defaultValue;
    }

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

        if (!inList || playerTable.ContainsKey(player.SlotId) || player.GameState != PlayerAgent.State.Unregistered)
        {
            return;
        }

        playerTable.Add(player.SlotId, player);
        player.GameState = PlayerAgent.State.Ready;

        if (playerList.Count == playerTable.Count)
        {
            foreach (var p in playerTable.Values)
            {
                p.GameState = PlayerAgent.State.InGame;
            }

            NextPhase();
        }
    }

    public List<PlayerAgent> GetAllPlayers()
    {
        var ret = new List<PlayerAgent>();
        for (int i = 0; i < playerList.Count; i++)
        {
            ret.Add(playerTable[playerList[i]]);
        }
        return ret;
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

        if (phase >= runtimePhases[round].Count)
        {
            phase = 0;
            if (runtimePhases[round].forEachPlayer)
            {
                player++;

                if (player >= playerList.Count)
                {
                    player = 0;
                    NextRound();
                }
            }
            else
            {
                NextRound();
            }
        }

        currentPhase = runtimePhases[round][phase];
        currentPlayer = runtimePhases[round].forEachPlayer ? playerTable[playerList[player]] : null;
        currentPhase.CurrentPlayer = currentPlayer;
        currentPhase.GameOver = false;
        timeElapsed = 0.0f;
        finishCurrentPhase.Clear();

        currentPhase.OnEnter();
    }

    void NextRound()
    {
        if (runtimePhases[round].once)
        {
            runtimePhases.RemoveAt(round);
        }
        else
        {
            round++;
        }
        round %= runtimePhases.Count;
    }

    void Update()
    {
        if (null == currentPhase || finishGame)
            return;

        timeElapsed += Time.deltaTime;
        if (AllPlayerFinishedCurrentPhase() || (currentPhase.timeLimit >= 0 && timeElapsed > currentPhase.timeLimit))
        {
            currentPhase.OnExit();

            if (currentPhase.GameOver)
            {
                // TODO
            }
            else
            {
                NextPhase();
            }
        }
        else
        {
            currentPhase.OnTick();
        }
    }

    bool AllPlayerFinishedCurrentPhase()
    {
        if (null == currentPhase)
            return false;

        if (null == currentPhase.CurrentPlayer)
        {
            var players = GetAllPlayers();
            foreach (var player in players)
            {
                if (!finishCurrentPhase.Contains(player.SlotId))
                    return false;
            }
            return true;
        }

        return finishCurrentPhase.Contains(currentPhase.CurrentPlayer.SlotId);
    }

    public void FinishCurrentPhase(int playerSlot)
    {
        finishCurrentPhase.Add(playerSlot);
    }

    public bool IsPlayerFinishedCurrentPhase(int playerSlot)
    {
        return finishCurrentPhase.Contains(playerSlot);
    }

    public void Surrender(int playerSlot)
    {
        finishGame = true;
        var players = GetAllPlayers();
        foreach (var player in players)
        {
            player.GameState = (player.SlotId == playerSlot) ? PlayerAgent.State.Lose : PlayerAgent.State.Win;
        }
    }

    #endregion

}
