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
    [SerializeField] private int maxSquidsAtOnce = 1;

    private bool spawnerActivated = false;
    private float timeSinceLastSpawn;
    private List<PrefabSpawner> spawnedEnemies;

    int CurrNumberOfSquids
    {
        get
        {
            int squidsFound = 0;
            foreach (PrefabSpawner obj in spawnedEnemies)
            {
                if (obj.prefabToSpawn.name == "DashAttackEnemy")
                {
                    squidsFound++;
                }
            }
            return squidsFound;
        }
    }
    
    void Start()
    {
        spawnedEnemies = new List<PrefabSpawner>();
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
        if(CurrNumberOfSquids >= maxSquidsAtOnce)
            ps.prefabToSpawn = enemyPool[0];    //Index 0 must not be a squid for obvious reasons.
        else
            ps.prefabToSpawn = enemyPool[Random.Range(0, enemyPool.Length)];

        spawnedEnemies.Add(ps);
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
        foreach (PrefabSpawner obj in spawnedEnemies)
        {
            if(obj == null) continue;
            Transform t = obj.transform;
            while (t.parent != null)
                t = t.parent;
            Destroy(t.gameObject);
        }
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
