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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _treeLayerNumber)
        {
            if (photonView.IsMine)
            {
                for (int i = 0; i < _playerVisionController.CaughtPlayersTransform.Count; i++)
                {
                    Transform caughtPlayer = _playerVisionController.CaughtPlayersTransform[i];
                    PhotonNetwork.Instantiate(_bars.name, caughtPlayer.position, Quaternion.identity);
                }
            }
        }
    }
}
