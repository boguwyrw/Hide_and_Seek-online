using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerVisionController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Camera _camera;

    [SerializeField] private LayerMask _hitLayer;

    [SerializeField] private GameObject _chain;

    private float _visionLength = 400.0f;
    private float _playerSlowSpeed = 0.80f;

    private bool _canUseChain = false;

    private Vector3 _hitPosition = new Vector3(0.5f, 0.5f, 0.0f);

    private List<Transform> _caughtPlayersTransform = new List<Transform>();

    public List<Transform> CaughtPlayersTransform {  get { return _caughtPlayersTransform; } }

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        if (photonView.IsMine)
        {

        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && PlayerSpawner.Instance.PlayerIsSeeker)
        {
            _canUseChain = true;
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (_canUseChain)
            {
                _canUseChain = false;

                Ray ray = _camera.ViewportPointToRay(_hitPosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, _visionLength, _hitLayer))
                {
                    GameObject hitPlayer = hit.collider.gameObject;
                    PhotonNetwork.Instantiate(_chain.name, hitPlayer.transform.position, Quaternion.identity);

                    PhotonView seenPlayerPhotonView = hitPlayer.GetPhotonView();
                    _caughtPlayersTransform.Add(seenPlayerPhotonView.transform);
                    seenPlayerPhotonView.RPC("PlayerRecognizedRPC", RpcTarget.All, photonView.Owner.NickName, PhotonNetwork.LocalPlayer.ActorNumber, _playerSlowSpeed);
                }
            }
        }
    }

    [PunRPC]
    private void PlayerRecognizedRPC(string observerName, int observerActorNo, float playerSlowSpeed)
    {
        PlayerRecognized(observerName, observerActorNo, playerSlowSpeed);
    }

    private void PlayerRecognized(string observer, int observerNo, float slowSpeed)
    {
        if (photonView.IsMine)
        {
            string playerName = photonView.Owner.NickName;

            PlayerSpawner.Instance.PlayerHasBeenFound(observer);
            DataManager.Instance.SendUpdatePlayerStats(actorNo: observerNo, actionIndex: 1, isSeeker: false, isCatch: true);

            PlayerController playerController = gameObject.GetComponent<PlayerController>();
            playerController.PlayerRecognizedSpeed(slowSpeed);
            //playerController.PlayerRecognizedColor();
        } 
    }
}
