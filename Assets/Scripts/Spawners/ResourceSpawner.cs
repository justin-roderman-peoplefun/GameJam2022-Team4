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
    [SerializeField] private GameObject stageEndPrefab;

    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxResourcesOnScreen = 3;
    [SerializeField] private float resourceSpawnRate = 0.7f;
    [SerializeField] private float minimumTimeBetweenSpawns = 1f;
    [SerializeField] private float heartSpawnRate = 0.7f;

    private float timeSinceLastSpawn;
    private int totalResourcesSpawned;
    private List<GameObject> spawnedResources;
    private bool goalHasSpawned;

    private void Awake()
    {
        spawnedResources = new List<GameObject>();
        timeSinceLastSpawn = 0f;
        totalResourcesSpawned = 0;
        goalHasSpawned = false;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        
        if (!StageManager.IsStagePlaying || timeSinceLastSpawn < minimumTimeBetweenSpawns || goalHasSpawned)
            return;
        
        spawnedResources = spawnedResources.Where(o => o != null).ToList();

        if (spawnedResources.Count >= maxResourcesOnScreen)
            return;

        if (totalResourcesSpawned < poolSize)
        {
            GameObject go = Instantiate(prefabSpawner);
            go.transform.position = new Vector3(transform.position.x + UnityEngine.Random.Range(-2, 3),
                transform.position.y, 0f);
            PrefabSpawner ps = go.GetComponent<PrefabSpawner>();
            bool shieldsOnScreen = false;
            foreach (GameObject obj in spawnedResources)
            {
                if (obj.name == "ShieldResource(Clone)")
                {
                    shieldsOnScreen = true;
                    break;
                }
            }
            if(PlayerController.Instance.IsShielded || shieldsOnScreen)
                ps.prefabToSpawn = heartPrefab;
            else
            {
                ps.prefabToSpawn = UnityEngine.Random.Range(0f, 1f) > heartSpawnRate ? shieldPrefab : heartPrefab;
            }
            spawnedResources.Add(go);
            timeSinceLastSpawn = 0;
            totalResourcesSpawned++;
        }
        else
        {
            GameObject go = Instantiate(prefabSpawner);
            go.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            PrefabSpawner ps = go.GetComponent<PrefabSpawner>();
            ps.prefabToSpawn = stageEndPrefab;
            spawnedResources.Add(go);
            timeSinceLastSpawn = 0;
            goalHasSpawned = true;
        }
    }
    
    private void OnDestroy()
    {
        foreach(GameObject go in spawnedResources)
            Destroy(go);
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
