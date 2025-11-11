using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Animator animator;
    
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int bulletDamage = 10;

    [Header("Debug")]
    [SerializeField] private EnemyBrain currentTarget;

    private readonly List<EnemyBrain> detectedEnemies = new();
    private float nextAttackTime;

    private void OnEnable()
    {
        EventBus<EnemyBrain>.Subscribe(EventType.OnEnemySpawned, RegisterEnemy);
        EventBus<EnemyBrain>.Subscribe(EventType.OnEnemyDespawned, UnregisterEnemy);
    }

    private void OnDisable()
    {
        EventBus<EnemyBrain>.Unsubscribe(EventType.OnEnemySpawned, RegisterEnemy);
        EventBus<EnemyBrain>.Unsubscribe(EventType.OnEnemyDespawned, UnregisterEnemy);
    }

    private void RegisterEnemy(EnemyBrain enemy)
    {
        if (!detectedEnemies.Contains(enemy))
            detectedEnemies.Add(enemy);
    }
    private void UnregisterEnemy(EnemyBrain enemy)
    {
        detectedEnemies.Remove(enemy);
    }

    private void Update()
    {
        currentTarget = GetClosestEnemy();
        if (currentTarget == null)
            return;

        Transform targetTransform = currentTarget.transform;

       
        Vector3 toTarget = targetTransform.position - transform.position;
        float sqrDist = toTarget.sqrMagnitude;
        if (sqrDist > attackRange * attackRange)
            return;
        
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toTarget.normalized);
            transform.rotation = lookRot;
        }
        
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            ShootAt(targetTransform);
            
        }
    }


    private EnemyBrain GetClosestEnemy()
    {
        EnemyBrain closestEnemy = null;
        float minSqrDistance = Mathf.Infinity;

        foreach (EnemyBrain enemy in detectedEnemies)
        {
            if (enemy == null) 
                continue;

            if (!enemy.gameObject.activeInHierarchy) 
                continue;

            Vector3 diff = enemy.transform.position - transform.position;
            float sqrDist = diff.sqrMagnitude; 

            if (sqrDist < minSqrDistance)
            {
                minSqrDistance = sqrDist;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
    
    private void ShootAt(Transform target)
    {
        GameObject bulletObject = bulletPool.GetBullet(firePoint.position, firePoint.rotation);
        bulletObject.GetComponent<Bullet>().InitializeBullet(
            firePoint.position,
            firePoint.rotation, 
            target,bulletSpeed, 
            bulletDamage,bulletPool);
    }
}
