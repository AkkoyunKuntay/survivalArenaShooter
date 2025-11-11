using UnityEngine;

public interface IPoolable
{
    void OnSpawned(Transform target, EnemyPool pool);
    void OnDespawned();
}