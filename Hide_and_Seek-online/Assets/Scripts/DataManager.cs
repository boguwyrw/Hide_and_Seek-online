using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class DataManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static DataManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private List<PlayerInfo> _allPlayers = new List<PlayerInfo>();

    private int _playerIndex = -1;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData eventData)
    {
        if (eventData.Code < 200)
        {
            EGameEventCodes gameEventCode = (EGameEventCodes)eventData.Code;
            object[] data = (object[])eventData.CustomData;

            switch (gameEventCode)
            {
                case EGameEventCodes.NewPlayerInGame:
                    ReceiveNewPlayer(data);
                    break;

                case EGameEventCodes.AllPlayersInGame:
                    ReceiveAllPlayers(data);
                    break;

                case EGameEventCodes.UpdatePlayerStats:
                    ReceiveUpdatePlayerStats(data);
                    break;
            }

        }
    }

    public void SendNewPlayer(string username)
    {
        object[] package = new object[4]; // 4 - PlayerInfo variables
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = false;
        package[3] = false;

        PhotonNetwork.RaiseEvent((byte)EGameEventCodes.NewPlayerInGame, package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true });
    }

    public void ReceiveNewPlayer(object[] receivedData)
    {
        PlayerInfo playerInfo = new PlayerInfo((string)receivedData[0], (int)receivedData[1], (bool)receivedData[2], (bool)receivedData[3]);

        _allPlayers.Add(playerInfo);

        SendAllPlayers();
    }

    public void SendAllPlayers()
    {
        int numberOfPlayers = _allPlayers.Count;
        object[] packageList = new object[numberOfPlayers];

        for (int i = 0; i < numberOfPlayers; i++)
        {
            object[] packagePart = new object[4]; // 4 - PlayerInfo variables
            packagePart[0] = _allPlayers[i].PlayerName;
            packagePart[1] = _allPlayers[i].PlayerActor;
            packagePart[2] = _allPlayers[i].IsPlayerSeeker;
            packagePart[3] = _allPlayers[i].IsPlayerCatch;

            packageList[i] = packagePart;
        }

        PhotonNetwork.RaiseEvent((byte)EGameEventCodes.AllPlayersInGame, packageList,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }

    public void ReceiveAllPlayers(object[] receivedData)
    {
        _allPlayers.Clear();

        for (int i = 0; i < receivedData.Length; i++)
        {
            object[] dataPart = (object[])receivedData[i];
            PlayerInfo playerInfo = new PlayerInfo((string)dataPart[0], (int)dataPart[1], (bool)dataPart[2], (bool)dataPart[3]);
            _allPlayers.Add(playerInfo);

            if (PhotonNetwork.LocalPlayer.ActorNumber == playerInfo.PlayerActor)
            {
                _playerIndex = i;
            }
        }
    }

    public void SendUpdatePlayerStats(int actorNo, int actionIndex, bool isSeeker, bool isCatch)
    {
        object[] updatePackage = new object[] { actorNo, actionIndex, isSeeker, isCatch };

        PhotonNetwork.RaiseEvent((byte)EGameEventCodes.UpdatePlayerStats, updatePackage,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }

    public void ReceiveUpdatePlayerStats(object[] receivedData)
    {
        int actorNumber = (int)receivedData[0];
        int actionIndex = (int)receivedData[1];
        bool isPlayerSeeker = (bool)receivedData[2];
        bool isPlayerCatch = (bool)receivedData[3];

        for (int i = 0; i < _allPlayers.Count; i++)
        {
            if (_allPlayers[i].PlayerActor == actorNumber)
            {
                switch (actionIndex)
                {
                    case 0: // assign seeker
                        _allPlayers[i].IsPlayerSeeker = true;
                        break;

                    case 1: // catch player
                        _allPlayers[i].IsPlayerCatch = true;
                        break;
                }

                if (i == _playerIndex)
                {

                }

                break;
            }
        }
    }
}
