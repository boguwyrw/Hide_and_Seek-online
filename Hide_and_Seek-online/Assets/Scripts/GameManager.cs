using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
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

    private int _seekerIndex = -1;
    private float _foundLifetime = 5.0f;

    public int SeekerIndex {  get { return _seekerIndex; } }
    public float FoundLifetime { get { return _foundLifetime; } }

    private void Start()
    {
        Player[] allPlayers = PhotonNetwork.PlayerList;
        _seekerIndex = Random.Range(0, allPlayers.Length);

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(_mainMenuSceneIndex);
        }
    }

    private void Update()
    {
        
    }
}
