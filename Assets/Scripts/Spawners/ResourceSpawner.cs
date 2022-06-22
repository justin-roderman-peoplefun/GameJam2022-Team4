using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabSpawner;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject shieldPrefab;

    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxResourcesOnScreen = 3;
    [SerializeField] private float resourceSpawnRate = 0.7f;
    [SerializeField] private float minimumTimeBetweenSpawns = 1f;

    private float timeSinceLastSpawn;
    private List<GameObject> spawnedResources;

    private void Awake()
    {
        spawnedResources = new List<GameObject>();
        timeSinceLastSpawn = 0f;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        
        if (!StageManager.IsStagePlaying || timeSinceLastSpawn < minimumTimeBetweenSpawns)
            return;

        Debug.Log("SpawnedResources length before pruning: <color=magenta>" + spawnedResources.Count + "</color>");
        spawnedResources = spawnedResources.Where(o => o != null).ToList();
        Debug.Log("SpawnedResources length AFTER pruning: <color=magenta>" + spawnedResources.Count + "</color>");

        if (spawnedResources.Count >= maxResourcesOnScreen)
            return;

        GameObject go = Instantiate(prefabSpawner);
        go.transform.position = new Vector3(transform.position.x + UnityEngine.Random.Range(-2, 3), transform.position.y, 0f);
        PrefabSpawner ps = go.GetComponent<PrefabSpawner>();
        ps.prefabToSpawn = heartPrefab;
        spawnedResources.Add(go);
        timeSinceLastSpawn = 0;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.magenta;
        Handles.DrawWireDisc(transform.position, transform.forward, 1f);
        GUIStyle styles = new GUIStyle();
        styles.alignment = TextAnchor.MiddleCenter;
        styles.fontSize = 8;
        styles.richText = true;
        Handles.Label(transform.position, "<b><color=cyan>Resource Spawner</color></b>", styles); 
    }
#endif
}
