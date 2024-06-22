using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using WebSocketSharp;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Launcher Instance
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
    #endregion

    private readonly string CONNECTING = "Connecting to Network ...";
    private readonly string JOINING_LOBBY = "Joining lobby ...";
    private readonly string CREATING_ROOM = "Creating room ...";
    private readonly string LEAVING_ROOM = "Leaving room ...";
    private readonly string JOINING_ROOM = "Joining room ...";

    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private GameObject _createRoomPanel;
    [SerializeField] private GameObject _createPlayerNamePanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _roomBrowserPanel;
    [SerializeField] private GameObject _startGameButton;

    [SerializeField] private RoomButton _roomButton;

    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_InputField _playerNameInputField;

    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private TMP_Text _playerNameText;

    [SerializeField] private int _gameSceneIndex = 1;

    private bool _hasNickName = false;

    private List<RoomButton> _roomButtons = new List<RoomButton>();

    private List<TMP_Text> _playersNames = new List<TMP_Text>();

    #region MonoBehaviour methods
    private void Start()
    {
        CloseMenus();

        _loadingPanel.SetActive(true);
        _loadingText.text = CONNECTING;

        PhotonNetwork.ConnectUsingSettings();
    }
    #endregion

    #region private methods
    private void CloseMenus()
    {
        _buttonsPanel.SetActive(false);
        _loadingPanel.SetActive(false);
        _errorPanel.SetActive(false);
        _createRoomPanel.SetActive(false);
        _createPlayerNamePanel.SetActive(false);
        _roomPanel.SetActive(false);
        _roomBrowserPanel.SetActive(false);
    }

    private void PlayersInGame()
    {
        foreach (TMP_Text playerName in _playersNames)
        {
            Destroy(playerName.gameObject);
        }
        _playersNames.Clear();

        _playerNameText.gameObject.SetActive(false);

        Player[] players = PhotonNetwork.PlayerList;
        //int randomSeeker = Random.Range(0, players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            CreatePlayerNameText(players[i]);
            GameManager.Instance.SendNewPlayer(players[i].NickName);
        }
    }

    private void CreatePlayerNameText(Player player)
    {
        TMP_Text nameText = Instantiate(_playerNameText, _playerNameText.transform.parent);
        nameText.text = player.NickName;
        nameText.gameObject.SetActive(true);

        _playersNames.Add(nameText);
    }

    private void SwitchMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _startGameButton.SetActive(true);
        }
        else
        {
            _startGameButton.SetActive(false);
        }
    }
    #endregion

    #region Photon override methods
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;

        _loadingText.text = JOINING_LOBBY;
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        _buttonsPanel.SetActive(true);

        if (!_hasNickName)
        {
            CloseMenus();

            _createPlayerNamePanel.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        _roomPanel.SetActive(true);

        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        PlayersInGame();

        SwitchMaster();
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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SwitchMaster();
    }
    #endregion

    #region public methods
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

    public void CreatePlayerNameButton()
    {
        if (!string.IsNullOrEmpty(_playerNameInputField.text))
        {
            PhotonNetwork.NickName = _playerNameInputField.text;

            CloseMenus();
            _buttonsPanel.SetActive(true);

            _hasNickName = true;
        }
    }

    public void StartGameButton()
    {
        // tutaj zrobiæ losowanie, ktory gracz bedzie szuka³
        PhotonNetwork.LoadLevel(_gameSceneIndex);
    }
    #endregion
}
