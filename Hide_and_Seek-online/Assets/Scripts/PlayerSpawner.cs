using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;

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

    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private Transform[] _spawnPoints;

    private GameObject _player;

    private bool _playerIsSeeker = false;

    //private Dictionary<int, bool> _oneOfSeekers = new Dictionary<int, bool>();

    private float _hasBeenFoundLifetime => GameManager.Instance.FoundLifetime;

    public bool PlayerIsSeeker { get { return _playerIsSeeker; } }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            DataManager.Instance.SendNewPlayer(PhotonNetwork.NickName);
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        Player[] players = PhotonNetwork.PlayerList;

        foreach (Player p in players)
        {
            Debug.Log(p.TagObject);
            if (p.TagObject != null)
            {
                string playerTag = p.TagObject.ToString();
                if (playerTag.Equals("Seeker"))
                {
                    _playerIsSeeker = true;
                }
            }
        }

        int randomPoint = -1;
        if (_playerIsSeeker)
        {
            randomPoint = 0;
        }
        else
        {
            randomPoint = Random.Range(1, _spawnPoints.Length);
        }

        Transform spawnPoint = _spawnPoints[randomPoint];

        _player = PhotonNetwork.Instantiate(_playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController.PlayerColor(_playerIsSeeker);

        DataManager.Instance.SendUpdatePlayerStats(actorNo: PhotonNetwork.LocalPlayer.ActorNumber, actionIndex: 0, isSeeker: true, isCatch: false);
    }

    public void PlayerHasBeenFound(string playerWhoFound)
    {
        UIController.Instance.PlayerNameWhoFound(playerWhoFound);

        if (_player != null)
        {
            StartCoroutine(CoPlayerHasBeenFound());
        }
    }

    private IEnumerator CoPlayerHasBeenFound()
    {
        yield return new WaitForSeconds(_hasBeenFoundLifetime);
        UIController.Instance.OpenCloseFoundPanel(isOpen: false);
        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController.RestorePlayerSpeed();
    }
}
