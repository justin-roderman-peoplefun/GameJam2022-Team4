using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        go.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        PrefabSpawner ps = go.GetComponent<PrefabSpawner>();
        ps.prefabToSpawn = enemyPool[Random.Range(0, enemyPool.Length)];
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
