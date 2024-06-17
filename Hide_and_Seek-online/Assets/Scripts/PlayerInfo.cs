using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string PlayerName;
    public int PlayerActor;
    public bool IsPlayerSeeker;

    public PlayerInfo(string playerName, int playerActor, bool isPlayerSeeker)
    {
        PlayerName = playerName;
        PlayerActor = playerActor;
        IsPlayerSeeker = isPlayerSeeker;
    }
}
