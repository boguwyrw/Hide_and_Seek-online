using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string PlayerName { get; private set; }
    public int PlayerActor { get; private set; }
    public bool IsPlayerSeeker { get; private set; }
    public bool IsPlayerCatch { get; private set; }

    public PlayerInfo(string playerName, int playerActor, bool isPlayerSeeker, bool isPlayerCatch)
    {
        PlayerName = playerName;
        PlayerActor = playerActor;
        IsPlayerSeeker = isPlayerSeeker;
        IsPlayerCatch = isPlayerCatch;
    }
}
