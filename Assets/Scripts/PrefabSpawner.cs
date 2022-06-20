using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

//Mostly an empty GameObject. When a PrefabSpawner collides with the spawn zone outside the camera, spawn the associated prefab.
[RequireComponent(typeof(CircleCollider2D))]
public class PrefabSpawner : MonoBehaviour
{
    private CircleCollider2D bounds;
    
    public GameObject prefabToSpawn;

    private void Start()
    {
        bounds = GetComponent<CircleCollider2D>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Why isn't this printing");
        /*Debug.Log("PrefabSpawner <color=green>" + gameObject.name + "</color>");
        if (other.gameObject.CompareTag("CameraSpawnZone"))
        {
            Debug.Log("PrefabSpawner <color=green>" + gameObject.name + "</color>");
            Instantiate(prefabToSpawn);
        }*/
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("CameraSpawnZone"))
            Instantiate(prefabToSpawn);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!bounds)
            bounds = GetComponent <CircleCollider2D>();
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, bounds.radius);
        if (prefabToSpawn != null)
        {
            GUIStyle styles = new GUIStyle();
            styles.alignment = TextAnchor.MiddleCenter;
            styles.fontSize = (int)(6f * bounds.radius);
            Handles.Label(transform.position, prefabToSpawn.name, styles); 
        }
    }
    #endif
}
