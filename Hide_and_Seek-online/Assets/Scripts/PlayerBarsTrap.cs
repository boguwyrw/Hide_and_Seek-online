using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBarsTrap : MonoBehaviourPunCallbacks
{
    // seeker has to inform caught player that is in circle
    [SerializeField] private PlayerVisionController _playerVisionController;
    [SerializeField] private GameObject _bars;

    [SerializeField] private int _treeLayerNumber = 9;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    [PunRPC]
    private void PlayerCaughtRPC()
    {
        PlayerCaught();
    }

    private void PlayerCaught()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _treeLayerNumber)
        {
            for (int i = 0; i < _playerVisionController.CaughtPlayersPhotonView.Count; i++)
            {
                if (_playerVisionController.CaughtPlayersPhotonView[i].IsMine)
                {
                    PhotonNetwork.Instantiate(_bars.name, transform.position, Quaternion.identity);
                    _playerVisionController.CaughtPlayersPhotonView[i].RPC("PlayerCaughtRPC", RpcTarget.All);
                }
            }
        }
    }
}
