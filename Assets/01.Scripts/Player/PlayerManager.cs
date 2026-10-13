using System.Collections.Generic;
using UnityEngine;
using System;

public static class PlayerManager
{
    static readonly List<PlayerCharacter> players = new List<PlayerCharacter>();

    public static IReadOnlyList<PlayerCharacter> Players => players;
    public static event Action OnAllPlayersDead;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() 
    {
        players.Clear();
        OnAllPlayersDead = null;
    }

    public static void Register(PlayerCharacter player) 
    {
        if (player != null && !players.Contains(player)) players.Add(player);
    }

    public static void Unregister(PlayerCharacter player) 
    {
        players.Remove(player);
    }

    public static PlayerCharacter GetClosest(Vector2 position) 
    {
        PlayerCharacter closest = null;
        float closestSqr = float.MaxValue;

        foreach (PlayerCharacter p in players) 
        {
            if (p == null || p.IsDead) continue;
            float sqr = ((Vector2)p.transform.position - position).sqrMagnitude;
            if (sqr < closestSqr) 
            {
                closestSqr = sqr;
                closest = p;
            }
        }
        return closest;
    }

    public static bool AllDead 
    {
        get
        {
            if (players.Count == 0) return false;
            foreach (PlayerCharacter p in players)
                if (p != null && !p.IsDead) return false;
            return true;
        }
    }

    public static void  NotifyPlayerDied(PlayerCharacter player) 
    {
        if (!AllDead) return;
        GameLog.Log("Player", "모든 플레이어 사망");
        OnAllPlayersDead.Invoke();
    }
}
