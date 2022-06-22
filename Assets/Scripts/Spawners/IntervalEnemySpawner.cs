using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class IntervalEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabSpawner;
    [SerializeField] private GameObject[] enemyPool;
    
    [SerializeField] private int maxEnemiesOnScreen = 3;
    [SerializeField] private float minimumTimeBetweenSpawns = 1f;
    
    private float timeSinceLastSpawn;
    private List<GameObject> spawnedEnemies;

    void Start()
    {
        spawnedEnemies = new List<GameObject>();
    }

    void SpawnEnemy()
    {
        if (enemyPool.Length < 1) return;
        
        GameObject go = Instantiate(prefabSpawner);
        go.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        PrefabSpawner ps = go.GetComponent<PrefabSpawner>();
        ps.prefabToSpawn = enemyPool[Random.Range(0, enemyPool.Length)];
        spawnedEnemies.Add(go);
        timeSinceLastSpawn = 0;
    }
    
    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        
        if (!StageManager.IsStagePlaying || timeSinceLastSpawn < minimumTimeBetweenSpawns)
            return;

        Debug.Log("SpawnedResources length before pruning: <color=magenta>" + spawnedEnemies.Count + "</color>");
        spawnedEnemies = spawnedEnemies.Where(o => o != null).ToList();
        Debug.Log("SpawnedResources length AFTER pruning: <color=magenta>" + spawnedEnemies.Count + "</color>");

        if (spawnedEnemies.Count >= maxEnemiesOnScreen)
            return;
        
        SpawnEnemy();
    }

    private void OnDestroy()
    {
        foreach(GameObject go in spawnedEnemies)
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
        Handles.Label(transform.position, "<b><color=cyan>Enemy Spawner</color></b>", styles); 
    }
#endif
}
