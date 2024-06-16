using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
        int randomPoint = Random.Range(0, _spawnPoints.Length);
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
    }
}
