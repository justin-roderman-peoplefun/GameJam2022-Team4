using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Move()
    {
        Vector2 move = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Debug.Log(move);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
