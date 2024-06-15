using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerVisionController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Camera _camera;

    [SerializeField] private LayerMask _hitLayer;

    private float _visionLength = 400.0f;

    private Vector3 _hitPosition = new Vector3(0.5f, 0.5f, 0.0f);

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Ray ray = _camera.ViewportPointToRay(_hitPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _visionLength, _hitLayer))
            {
                PhotonView seenPlayerPhotonView = hit.collider.gameObject.GetPhotonView();
                seenPlayerPhotonView.RPC("PlayerRecognizedRPC", RpcTarget.All, photonView.Owner.NickName);
            }
        }
    }

    [PunRPC]
    private void PlayerRecognizedRPC(string observerName)
    {
        PlayerRecognized(observerName);
    }

    private void PlayerRecognized(string observer)
    {
        string playerName = photonView.Owner.NickName;
        Debug.Log("Jestem " + playerName + " i zosta³em zauwazony przez " + observer);
    }
}
