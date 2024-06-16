using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [SerializeField] private Transform _chainModel;

    private int _playerLayer = 7;

    private float _chainLifetime = 5.0f;
    private float _chainRotationSpeed = 1.80f;

    private void Start()
    {
        Destroy(gameObject, _chainLifetime);
    }

    private void Update()
    {
        _chainModel.Rotate(0.0f, _chainRotationSpeed, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _playerLayer)
        {
            transform.parent = other.transform;
        }
    }
}
