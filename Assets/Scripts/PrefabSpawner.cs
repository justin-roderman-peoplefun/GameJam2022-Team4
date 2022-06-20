using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mostly an empty GameObject. When a PrefabSpawner collides with the spawn zone outside the camera, spawn the associated prefab.
[RequireComponent(typeof(BoxCollider2D))]
public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnRadius;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("CameraSpawnZone"))
            Instantiate(prefabToSpawn);
    }

    public void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("CameraSpawnZone"))
            Instantiate(prefabToSpawn);
    }
}
