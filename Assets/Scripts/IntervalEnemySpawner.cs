using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervalEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabSpawner;
    [SerializeField] private GameObject[] enemyPool;
    
    public float spawnInterval = 2f;
    
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 1.0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (enemyPool.Length < 1) return;
        
        GameObject go = Instantiate(prefabSpawner);
        PrefabSpawner ps = go.GetComponent<PrefabSpawner>();
        ps.prefabToSpawn = enemyPool[Random.Range(0, enemyPool.Length)];
    }
}
