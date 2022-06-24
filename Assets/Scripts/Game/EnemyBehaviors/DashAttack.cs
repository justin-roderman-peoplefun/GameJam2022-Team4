using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashAttack : MonoBehaviour
{
    private Camera mainCamera;
    private PlayerController playerToTarget;
    private Vector2 _targetPos;
    private Vector2 _playerVelocity = Vector2.zero;
    private Rigidbody2D _rb;

    public float timeBetweenDashes = 4f;
    public float dashSpeed = 5f;
    public float maxSpeed = 2f;
    public float overshootAmount = 1f;
    public float maxRotationAngle = 15f;

    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite swimSprite;
    
    Vector2 PlayerPosition => (Vector2)playerToTarget.transform.position;

    private void Start()
    {
        _rb = GetComponentInChildren<Rigidbody2D>();
        playerToTarget = PlayerController.Instance;
        _targetPos = Vector2.zero;
        SelectTargetPosition();
    }
    
    private void SelectTargetPosition()
    {
        Vector2 unitVectorToPlayer = (PlayerPosition - (Vector2)transform.position).normalized;

        _targetPos = PlayerPosition + (unitVectorToPlayer * overshootAmount);
        //GetComponentInChildren<SpriteRenderer>().color = Color.red;

        StartCoroutine(Dash());
    }
    
    private IEnumerator Dash()
    {
        float smoothTime = (1 / dashSpeed);
        float maxSpeed = this.maxSpeed * dashSpeed;

        enemySprite.sprite = swimSprite;
        
        
        while (Vector2.Distance(_rb.position, _targetPos) > 0.05f)
        {
            // If we've hit the target position, move it up a bit so that we drift
            Vector2 targetPos = _targetPos;
            _rb.position = Vector2.SmoothDamp(_rb.position, targetPos, ref _playerVelocity, smoothTime, maxSpeed);

            if(Vector2.Distance(_rb.position, _targetPos) < 0.5f)
                enemySprite.sprite = idleSprite;
            
            yield return null;
        }

        enemySprite.sprite = idleSprite;
        
        float waitForNextDashTimer = 0f;
        
        while (waitForNextDashTimer < timeBetweenDashes)
        {
            Vector2 targetPos = _targetPos + new Vector2(0.05f, 0);
            _rb.position = Vector2.SmoothDamp(_rb.position, targetPos, ref _playerVelocity, smoothTime, maxSpeed);
            
            Vector3 vectorToTarget = (Vector3)targetPos - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5f);
            
            yield return null;
            waitForNextDashTimer += Time.deltaTime;
        }
        
        SelectTargetPosition();
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Vector2.Distance(transform.position, _targetPos) > 0.05f)
        {
            Gizmos.DrawSphere(_targetPos, 0.25f);
        }
    }
#endif
}
