using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class CameraSpawnZone : MonoBehaviour
{
    private Camera mainCamera;
    private BoxCollider2D bounds;
    
    public int skinWidth = 3;

    private void Start()
    {
        mainCamera = Camera.main;
        bounds = GetComponent<BoxCollider2D>();

        transform.position = mainCamera.transform.position;
        bounds.size = new Vector2(2 * mainCamera.orthographicSize * mainCamera.aspect + skinWidth, 2 * mainCamera.orthographicSize + skinWidth);
    }

    private void Update()
    {
        float orthographicSize = mainCamera.orthographicSize;
        float aspect = mainCamera.aspect;
        bounds.size = new Vector2(2 * orthographicSize * aspect + skinWidth, 2 * orthographicSize + skinWidth);
    }

    private void OnDrawGizmos()
    {
        float orthographicSize = mainCamera.orthographicSize;
        float aspect = mainCamera.aspect;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(2 * orthographicSize * aspect + skinWidth, 2 * orthographicSize + skinWidth, 1));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(2 * orthographicSize * aspect, 2 * orthographicSize, 1));
    }
}
