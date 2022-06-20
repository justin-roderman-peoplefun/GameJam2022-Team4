using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSide : MonoBehaviour
{
    private float t = 0;
    [SerializeField] private Transform enemyBody;

    public float verticalSpeed = 1;
    public float horizontalSpeed = 1;
    public float horizontalRange = 1;
    
    Vector3 Position => transform.position;
    
    void FixedUpdate()
    {
        transform.position = new Vector3(Position.x, Position.y + (verticalSpeed / 10f), Position.z);
        
        if (enemyBody != null)
        {
            t += Time.deltaTime * horizontalSpeed;
            enemyBody.position = new Vector3(transform.position.x + (Mathf.Cos(t) * horizontalRange), transform.position.y, transform.position.z);
        }
    }
}
