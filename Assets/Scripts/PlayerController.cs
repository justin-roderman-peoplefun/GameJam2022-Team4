using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;

    public float playerSpeed;
    public float maxRotationAngle;

    private Rigidbody2D _rb;

    private Vector2 _lastMousePos;
    private Vector2 _playerVelocity = Vector2.zero;
    private Quaternion _lookRotation;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _lastMousePos = _rb.position;
    }

    void InputMovement()
    {
        if (Input.GetMouseButton(0))
        {
            _lastMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void Move()
    {
        _rb.position = Vector2.SmoothDamp(_rb.position, _lastMousePos, ref _playerVelocity, 1 / playerSpeed);

        float rot = maxRotationAngle * Mathf.Clamp(_playerVelocity.x, -1, 1);
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    // Update is called once per frame
    void Update()
    {
        InputMovement();
        Move();
    }
}
