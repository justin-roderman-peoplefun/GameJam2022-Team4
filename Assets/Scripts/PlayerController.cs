using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;

    public float playerSpeed;
    public float maxPlayerSpeed;
    public float maxRotationAngle;

    public float doubleTapTime;
    public float boostTime;
    public float boostFactor;

    private Rigidbody2D _rb;

    private Vector2 _targetPos;
    private Vector2 _playerVelocity = Vector2.zero;

    private float _lastTap = Single.NegativeInfinity;
    private bool _boost;
    private float _lastBoostTime;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _targetPos = _rb.position;
    }

    private void InputMovement()
    {
        if (Input.GetMouseButton(0))
        {
            _targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(0) && !_boost)
        {
            float time = Time.time;
            if (time - _lastTap <= doubleTapTime)
            {
                _boost = true;
                _lastBoostTime = time;
                GetComponent<SpriteRenderer>().color = Color.red;
            }
            _lastTap = time;
        }
    }

    private void Move()
    {
        float smoothTime = 1 / (playerSpeed * (_boost ? boostFactor : 1));
        float maxSpeed = maxPlayerSpeed * (_boost ? boostFactor : 1);
        // If we've hit the target position, move it up a bit so that we drift
        Vector2 targetPos = _targetPos + (Vector2.Distance(_rb.position, _targetPos) > 0.05f ? Vector2.zero : new Vector2(0.05f, 0));
        _rb.position = Vector2.SmoothDamp(_rb.position, targetPos, ref _playerVelocity, smoothTime, maxSpeed);

        float rot = maxRotationAngle * Mathf.Clamp(_playerVelocity.x, -1, 1);
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    private void UpdateBoostTime()
    {
        if (_boost && Time.time - _lastBoostTime >= boostTime)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            _boost = false;
        }
    }

    private void Update()
    {
        InputMovement();
        Move();
        
        UpdateBoostTime();
    }
}
