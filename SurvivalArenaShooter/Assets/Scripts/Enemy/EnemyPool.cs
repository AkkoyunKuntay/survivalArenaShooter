using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private Transform poolParent;

    private readonly Queue<GameObject> poolQueue = new();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            var enemy = Instantiate(enemyPrefab, poolParent);
            enemy.SetActive(false);
            poolQueue.Enqueue(enemy);
        }
    }

    public GameObject GetEnemy(Vector3 position, Quaternion rotation, Transform target)
    {
        if(!target) return null;
        
        GameObject enemy = poolQueue.Count > 0 ? poolQueue.Dequeue() : Instantiate(enemyPrefab, poolParent);
        enemy.transform.SetPositionAndRotation(position, rotation);
        enemy.SetActive(true);

        if (enemy.TryGetComponent(out IPoolable poolable))
            poolable.OnSpawned(target, this);

        return enemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy.TryGetComponent(out IPoolable poolable))
            poolable.OnDespawned();

        enemy.SetActive(false);
        poolQueue.Enqueue(enemy);
    }
}