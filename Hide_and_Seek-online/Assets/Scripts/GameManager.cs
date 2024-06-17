using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GameManager Instance;

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

    [SerializeField] private int _mainMenuSceneIndex = 0;

    private int _ourIndex = -1;

    private float _foundLifetime = 5.0f;

    private List<PlayerInfo> _allPlayers = new List<PlayerInfo>();

    public float FoundLifetime { get { return _foundLifetime; } }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(_mainMenuSceneIndex);
        }
        else
        {
            SendNewPlayer(PhotonNetwork.NickName);
        }
    }

    private void Update()
    {
        
    }

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
            GameEventCodes gameEventCode = (GameEventCodes)eventData.Code;
            object[] data = (object[])eventData.CustomData;

            switch(gameEventCode)
            {
                case GameEventCodes.PlayerNew:
                    ReceiveNewPlayer(data);
                    break;
                case GameEventCodes.ListPlayers:
                    ReceiveListPlayers(data);
                    break;
                case GameEventCodes.ChangeStatus:
                    ReceiveChangeStatus(data);
                    break;
            }
        }
    }

    public void SendNewPlayer(string username)
    {
        int playerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        
        Player[] players = PhotonNetwork.PlayerList;

        int randomSeeker = Random.Range(0, players.Length);

        for (int i = 0; i < players.Length; i++)
        {
            if (playerActorNumber == players[i].ActorNumber)
            {
                _ourIndex = i;
            }
        }
        
        object[] dataPackage = new object[3]; // 3 - PlayerInfo variables
        dataPackage[0] = username;
        dataPackage[1] = playerActorNumber;
        //dataPackage[2] = false; // replace correct value
        
        if (_ourIndex == randomSeeker)
            dataPackage[2] = true;
        else dataPackage[2] = false;
        
        PhotonNetwork.RaiseEvent((byte)GameEventCodes.PlayerNew, dataPackage,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true});
    }

    public void ReceiveNewPlayer(object[] receivedData)
    {
        PlayerInfo playerInfo = new PlayerInfo((string)receivedData[0], (int)receivedData[1], (bool)receivedData[2]);

        _allPlayers.Add(playerInfo);

        SendListPlayers();
    }

    public void SendListPlayers()
    {
        object[] packageList = new object[_allPlayers.Count];

        for (int i = 0; i < _allPlayers.Count; i++)
        {
            object[] packagePart = new object[3]; // 3 - PlayerInfo variables
            packagePart[0] = _allPlayers[i].PlayerName;
            packagePart[1] = _allPlayers[i].PlayerActor;
            packagePart[2] = _allPlayers[i].IsPlayerSeeker;

            packageList[i] = packagePart;
        }

        PhotonNetwork.RaiseEvent((byte)GameEventCodes.ListPlayers, packageList,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }

    public void ReceiveListPlayers(object[] receivedData)
    {
        _allPlayers.Clear();

        for (int i = 0; i < receivedData.Length; i++)
        {
            object[] dataPart = (object[])receivedData[i];

            PlayerInfo playerInfo = new PlayerInfo((string)dataPart[0], (int)dataPart[1], (bool)dataPart[3]);
            _allPlayers.Add(playerInfo);
        }
    }

    public void SendChangeStatus()
    {

    }

    public void ReceiveChangeStatus(object[] receivedData)
    {

    }

    public bool IsSeeker()
    {
        return _allPlayers[_ourIndex].IsPlayerSeeker;
    }
}
