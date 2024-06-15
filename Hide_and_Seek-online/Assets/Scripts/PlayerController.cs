using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _viewPoint;

    private float _mouseSensitivity = 1.2f;
    private float _maxVerticalRotation = 75.0f;

    private Vector2 _mouseInput;

    private void Start()
    {
        
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        _mouseInput = new Vector2(mouseX, mouseY) * _mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + _mouseInput.x, transform.rotation.eulerAngles.z);
    }
}
