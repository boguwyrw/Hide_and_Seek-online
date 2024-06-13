using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

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

    private readonly string CONNECTING = "Connecting to Network ...";
    private readonly string JOINING_LOBBY = "Joining lobby ...";
    private readonly string CREATING_ROOM = "Creating room ...";
    private readonly string LEAVING_ROOM = "Leaving room ...";
    private readonly string JOINING_ROOM = "Joining room ...";

    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private GameObject _createRoomPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _roomBrowserPanel;

    [SerializeField] private RoomButton _roomButton;

    [SerializeField] private TMP_InputField _roomNameInputField;

    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private TMP_Text _playerNameText;

    private List<RoomButton> _roomButtons = new List<RoomButton>();

    private List<TMP_Text> _playersNames = new List<TMP_Text>();

    private void Start()
    {
        CloseMenus();

        _loadingPanel.SetActive(true);
        _loadingText.text = CONNECTING;

        PhotonNetwork.ConnectUsingSettings();
    }

    private void CloseMenus()
    {
        _buttonsPanel.SetActive(false);
        _loadingPanel.SetActive(false);
        _errorPanel.SetActive(false);
        _createRoomPanel.SetActive(false);
        _roomPanel.SetActive(false);
        _roomBrowserPanel.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        _loadingText.text = JOINING_LOBBY;
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        _buttonsPanel.SetActive(true);

        PhotonNetwork.NickName = Random.Range(10, 1000).ToString();
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        _roomPanel.SetActive(true);

        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        PlayersInGame();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errorText.text = "Failed to Create Room: " + message;

        CloseMenus();
        _errorPanel.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        _buttonsPanel.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomButton rb in _roomButtons)
        {
            Destroy(rb.gameObject);
        }
        _roomButtons.Clear();

        _roomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomList.Count; i++)
        {
            bool canCreateRoomButton = roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList;
            if (canCreateRoomButton)
            {
                RoomButton roomButton = Instantiate(_roomButton, _roomButton.transform.parent);
                roomButton.SetButtonDetails(roomList[i]);
                roomButton.gameObject.SetActive(true);

                _roomButtons.Add(roomButton);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreatePlayerNameText(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayersInGame();
    }

    private void PlayersInGame()
    {
        foreach(TMP_Text playerName in _playersNames)
        {
            Destroy(playerName.gameObject);
        }
        _playersNames.Clear();

        _playerNameText.gameObject.SetActive(false);

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            CreatePlayerNameText(players[i]);
        }
    }

    private void CreatePlayerNameText(Player player)
    {
        TMP_Text nameText = Instantiate(_playerNameText, _playerNameText.transform.parent);
        nameText.text = player.NickName;
        nameText.gameObject.SetActive(true);

        _playersNames.Add(nameText);
    }

    public void OpenCreateRoomPanel()
    {
        CloseMenus();
        _createRoomPanel.SetActive(true);
    }

    public void CreateRoomButton()
    {
        if (!string.IsNullOrEmpty(_roomNameInputField.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 10;
            PhotonNetwork.CreateRoom(_roomNameInputField.text, roomOptions);

            CloseMenus();

            _loadingPanel.SetActive(true);
            _loadingText.text = CREATING_ROOM;
        }
    }

    public void CloseErrorButton()
    {
        CloseMenus();
        _buttonsPanel.SetActive(true);
    }

    public void LeaveRoomButton()
    {
        PhotonNetwork.LeaveRoom();

        CloseMenus();

        _loadingPanel.SetActive(true);
        _loadingText.text = LEAVING_ROOM;
    }

    public void OpenRoomBrowser()
    {
        CloseMenus();
        _roomBrowserPanel.SetActive(true);
    }

    public void CloseRoomBrowser()
    {
        CloseMenus();
        _buttonsPanel.SetActive(true);
    }

    public void JoinRoom(RoomInfo roomInfoInput)
    {
        PhotonNetwork.JoinRoom(roomInfoInput.Name);

        CloseMenus();
        _loadingPanel.SetActive(true);
        _loadingText.text = JOINING_ROOM;
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
