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

    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private GameObject _createRoomPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _roomBrowserPanel;

    [SerializeField] private TMP_InputField _roomNameInputField;

    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;

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
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        _roomPanel.SetActive(true);

        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;
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
}