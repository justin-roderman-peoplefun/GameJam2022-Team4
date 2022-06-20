using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUpwardsPlain : MonoBehaviour
{
    public float speed = 1;
    
    Vector3 Position => transform.position;

    private void FixedUpdate()
    {
        transform.position = new Vector3(Position.x, Position.y + (speed / 10f), Position.z);
    }
}
