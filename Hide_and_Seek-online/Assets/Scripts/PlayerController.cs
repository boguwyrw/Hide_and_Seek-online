using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _viewPoint; // not used
    [SerializeField] private Transform _camera;

    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private Renderer _renderer;

    private int _groundedNumber = 6;

    private float _mouseSensitivity = 1.4f;
    private float _maxVerticalRotation = 75.0f;
    private float _verticalRotation = 0.0f;

    private float _walkSpeed = 4.0f;
    private float _runSpeed = 12.0f;
    private float _currentSpeed = 0.0f;

    private float _jumpForce = 5.0f;

    private bool _canJump = false;
    private bool _isGrounded = true;
    private bool _isSeeker = false;

    private Vector2 _mouseInput;

    private Vector3 _movement;
    private Vector3 _moveDirection;

    private Color[] _colors = new Color[] { Color.red, Color.green, Color.blue, Color.gray, Color.cyan, Color.black, new Color(1.0f, 0.5f, 0.31f), new Color(1.0f, 0.84f, 0.0f), new Color(1.0f, 0.55f, 0.0f), new Color(1.0f, 0.41f, 0.706f) };
    private Color _playerColor;

    #region MonoBehaviour methods
    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main.transform;
        }

        _currentSpeed = _walkSpeed;

        if (_isSeeker)
        {
            ChangeColor(_colors[0]);
        }
        else
        {
            int indexColor = Random.Range(1, _colors.Length);
            ChangeColor(_colors[indexColor]);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            PlayerMovement();

            PlayerRotation();

            ReleaseJump();
        }
    }
    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            PlayerJump();
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            CameraFollow();
        }
    }
    #endregion

    #region private methods
    private void PlayerMovement()
    {
        float moveLeftRight = Input.GetAxisRaw("Horizontal");
        float moveForwardBack = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector3(moveLeftRight, 0.0f, moveForwardBack);

        Vector3 movementLeftRight = transform.right * _moveDirection.x;
        Vector3 movementForwardBack = transform.forward * _moveDirection.z;
        _movement = (movementLeftRight + movementForwardBack).normalized;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = _runSpeed;
        }
        else
        {
            _currentSpeed = _walkSpeed;
        }

        transform.position += _movement * _currentSpeed * Time.deltaTime;
    }

    private void PlayerRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        _mouseInput = new Vector2(mouseX, mouseY) * _mouseSensitivity;

        Vector3 playerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(playerRotation.x, playerRotation.y + _mouseInput.x, playerRotation.z);

        Vector3 cameraRotation = _viewPoint.rotation.eulerAngles;
        _verticalRotation += _mouseInput.y;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_maxVerticalRotation, _maxVerticalRotation);
        _viewPoint.rotation = Quaternion.Euler(-_verticalRotation, cameraRotation.y, cameraRotation.z);
    }

    private void ReleaseJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _canJump = true;
        }
    }

    private void PlayerJump()
    {
        if (_canJump && _isGrounded)
        {
            _canJump = false;
            _isGrounded = false;
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private void CameraFollow()
    {
        _camera.position = _viewPoint.position;
        _camera.rotation = _viewPoint.rotation;
    }

    private void ChangeColor(Color newColor)
    {
        if (photonView.IsMine)
        {
            _playerColor = newColor;
            UpdatePlayerColor(_playerColor);

            SetPlayerColorProperty(_playerColor);
        }
    }

    private void SetPlayerColorProperty(Color color)
    {
        Hashtable props = new Hashtable
        {
            { "playerColor", new Vector3(color.r, color.g, color.b) }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void UpdatePlayerColor(Color color)
    {
        _renderer.material.color = color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == _groundedNumber)
        {
            _isGrounded = true;
        }
    }
    #endregion

    #region public methods
    public void PlayerRecognizedSpeed(float speedValue)
    {
        _runSpeed = speedValue;
        _walkSpeed = speedValue;
        _currentSpeed = speedValue;
    }

    public void RestorePlayerSpeed()
    {
        _walkSpeed = 4.0f;
        _runSpeed = 12.0f;
    }
    /*
    public void PlayerRecognizedColor()
    {
        _renderer.material.color = new Color32(255, 120, 0, 255);
    }
    */
    public void PlayerColor(bool isSeeker)
    {
        _isSeeker = isSeeker;
    }
    #endregion

    #region Photon override methods
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("playerColor") && targetPlayer == photonView.Owner)
        {
            Vector3 color = (Vector3)changedProps["playerColor"];
            _playerColor = new Color(color.x, color.y, color.z);
            UpdatePlayerColor(_playerColor);
        }
    }
    #endregion
}
