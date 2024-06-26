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

    private float _hasBeenFoundLifetime => GameManager.Instance.FoundLifetime;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (GameManager.Instance.SeekerIndex == i)
            {
                _playerIsSeeker = true;
            }
            else
            {
                _playerIsSeeker = false;
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
