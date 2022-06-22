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
    [SerializeField] private float timeUntilFirstSpawn = 5f;

    private bool spawnerActivated = false;
    private float timeSinceLastSpawn;
    private List<GameObject> spawnedEnemies;

    void Start()
    {
        spawnedEnemies = new List<GameObject>();
        Invoke(nameof(ActivateSpawner), timeUntilFirstSpawn);
    }

    void ActivateSpawner()
    {
        spawnerActivated = true;
        timeSinceLastSpawn = minimumTimeBetweenSpawns;
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
        if (!spawnerActivated) return;
        
        timeSinceLastSpawn += Time.deltaTime;
        
        if (!StageManager.IsStagePlaying || timeSinceLastSpawn < minimumTimeBetweenSpawns)
            return;
        
        spawnedEnemies = spawnedEnemies.Where(o => o != null).ToList();

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
