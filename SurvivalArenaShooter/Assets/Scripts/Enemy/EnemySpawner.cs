using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDistance = 8f;
    [SerializeField] private float maxSpawnDistance = 15f;
    [SerializeField] private int enemiesPerWave = 5;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private float waveDelay = 5f;
    [SerializeField] private int totalWaves = 3;

    private void Start()
    {
        if (!mainCamera) mainCamera = Camera.main;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int w = 0; w < totalWaves; w++)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                Vector3 spawnPos = GetSpawnPosition();
                enemyPool.GetEnemy(spawnPos, Quaternion.identity, player);
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return new WaitForSeconds(waveDelay);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 rand = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 pos = player.position + new Vector3(rand.x, 0, rand.y) * dist;

            Vector3 camDir = mainCamera.transform.forward;
            Vector3 dirToSpawn = (pos - player.position).normalized;
            if (Vector3.Dot(camDir, dirToSpawn) > 0.3f) continue;

            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return player.position - player.forward * maxSpawnDistance;
    }
}