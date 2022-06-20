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

        bounds.size = new Vector2(2 * mainCamera.orthographicSize * mainCamera.aspect + skinWidth, 2 * mainCamera.orthographicSize + skinWidth);
    }

    private void Update()
    {
        bounds.size = new Vector2(2 * mainCamera.orthographicSize * mainCamera.aspect + skinWidth, 2 * mainCamera.orthographicSize + skinWidth);
    }
}
