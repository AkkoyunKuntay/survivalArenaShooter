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

    private int currentWave = 0;
    private bool spawningActive = true;

    private void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;
        if (!enemyPool)
            enemyPool = FindObjectOfType<EnemyPool>();

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (currentWave < totalWaves && spawningActive)
        {
            yield return StartCoroutine(SpawnWave());
            currentWave++;
            yield return new WaitForSeconds(waveDelay);
        }
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector3 spawnPoint = GetValidSpawnPosition();
            if (spawnPoint != Vector3.zero)
            {
               GameObject enemy = enemyPool.GetEnemy(spawnPoint, Quaternion.identity);
               EnemyBrain enemyBrain = enemy.GetComponent<EnemyBrain>();
               enemyBrain.InitializeEnemy(transform,enemyPool);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        for (int attempt = 0; attempt < 15; attempt++)
        {
            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 candidatePos = player.position + new Vector3(randomDir2D.x, 0, randomDir2D.y) * distance;

            Vector3 camForward = mainCamera.transform.forward;
            Vector3 dirToSpawn = (candidatePos - player.position).normalized;
            float dot = Vector3.Dot(camForward, dirToSpawn);
            if (dot > 0.3f)
                continue;

            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return player.position - player.forward * maxSpawnDistance;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, maxSpawnDistance);
    }
}
