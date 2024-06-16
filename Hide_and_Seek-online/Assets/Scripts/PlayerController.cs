using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _viewPoint;
    [SerializeField] private Transform _camera;

    [SerializeField] private Rigidbody _rigidbody;

    private int _groundedNumber = 6;

    private float _mouseSensitivity = 1.4f;
    private float _maxVerticalRotation = 75.0f;
    private float _verticalRotation = 0.0f;

    private float _walkSpeed = 4.0f;
    private float _runSpeed = 12.0f;
    private float _currentSpeed = 0.0f;

    private float _jumpForce = 5.0f;

    private bool _canJump = false;
    private bool _isSeen = false;
    private bool _isGrounded = true;

    private Vector2 _mouseInput;

    private Vector3 _movement;
    private Vector3 _moveDirection;

    public Transform ViewPoint { get { return _viewPoint; } }

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main.transform;
        }

        _currentSpeed = _walkSpeed;
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
        else if (!_isSeen)
        {
            _currentSpeed = _walkSpeed;
        }
        else
        {
            _currentSpeed = 0.0f;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == _groundedNumber)
        {
            _isGrounded = true;
        }
    }

    public void SetIsSeen()
    {
        _isSeen = true;
    }

    public void PlayerRecognizedSpeed(float speedValue)
    {
        _runSpeed = speedValue;
        _walkSpeed = speedValue;
        _currentSpeed = speedValue;
    }
}
