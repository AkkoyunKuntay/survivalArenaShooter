using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private Transform poolParent;

    private readonly Queue<GameObject> _poolQueue = new Queue<GameObject>();

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, poolParent);
            enemy.SetActive(false);
            _poolQueue.Enqueue(enemy);
        }
    }

    public GameObject GetEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject enemy;

        if (_poolQueue.Count > 0)
        {
            enemy = _poolQueue.Dequeue();
            enemy.transform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            enemy = Instantiate(enemyPrefab, position, rotation, poolParent);
        }

        enemy.SetActive(true);
        return enemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        _poolQueue.Enqueue(enemy);
    }
}