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

    //private int _seekerIndex = -1;

    //public int SeekerIndex {  get { return _seekerIndex; } }

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
            object[] data = (object[])eventData.CustomData;

            ReceivePlayers(data);
        }
    }

    public void SendPlayers()
    {
        Player[] allPlayers = PhotonNetwork.PlayerList;
        object[] packageList = new object[allPlayers.Length];

        //int randomSeeker = Random.Range(0, allPlayers.Length);

        for (int i = 0; i < allPlayers.Length; i++)
        {
            object[] packagePart = new object[4]; // 4 - PlayerInfo variables
            packagePart[0] = allPlayers[i].NickName;
            packagePart[1] = allPlayers[i].ActorNumber;
            packagePart[2] = false;
            packagePart[3] = false;
            /*
            if (randomSeeker == i)
            {
                _seekerIndex = i;
                packagePart[2] = true;
            }
            else
            {
                packagePart[2] = false;
            }
            */
            packageList[i] = packagePart;
        }

        PhotonNetwork.RaiseEvent((byte)EGameEventCodes.AllPlayersInGame, packageList,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }

    public void ReceivePlayers(object[] receivedData)
    {
        _allPlayers.Clear();

        for (int i = 0; i < receivedData.Length; i++)
        {
            object[] dataPart = (object[])receivedData[i];

            PlayerInfo playerInfo = new PlayerInfo((string)dataPart[0], (int)dataPart[1], (bool)dataPart[2], (bool)dataPart[3]);
            _allPlayers.Add(playerInfo);
        }
    }
}
