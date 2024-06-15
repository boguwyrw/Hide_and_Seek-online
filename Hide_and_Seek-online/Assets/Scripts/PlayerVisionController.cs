using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisionController : MonoBehaviour
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
        Ray ray = _camera.ViewportPointToRay(_hitPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _visionLength, _hitLayer))
        {
            Debug.Log("Did Hit " + hit.collider.name);
        }
    }
}
