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
    private GameObject spawnedInstance;
    
    public GameObject prefabToSpawn;

    private void Start()
    {
        bounds = GetComponent<CircleCollider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (prefabToSpawn == null)
        {
            Debug.Log("PrefabSpawner <color=green>" + gameObject.name + "</color> collided with the spawn zone but had no associated prefab.");
            return;
        }
        if (other.gameObject.CompareTag("CameraSpawnZone") && spawnedInstance == null)
        {
            Debug.Log("PrefabSpawner <color=cyan>" + gameObject.name + "</color> collided with spawn zone, instantiating <color=cyan>" + prefabToSpawn.name + "</color>.");
            spawnedInstance = Instantiate(prefabToSpawn, transform.position, transform.rotation);
            transform.SetParent(spawnedInstance.transform);
            transform.localPosition = Vector3.zero;
            GetComponent<ScrollUpwardsPlain>().enabled = false;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CameraSpawnZone"))
        {
            Destroy(spawnedInstance);
        }
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
