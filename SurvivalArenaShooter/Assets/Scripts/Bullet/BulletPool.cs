using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private Transform poolParent;

    private readonly Queue<GameObject> poolQueue = new();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, poolParent);
            bullet.SetActive(false);
            poolQueue.Enqueue(bullet);
        }
    }

    public GameObject GetBullet(Vector3 pos, Quaternion rot)
    {
        GameObject bullet = poolQueue.Count > 0 ? poolQueue.Dequeue() : Instantiate(bulletPrefab, poolParent);
        bullet.transform.SetPositionAndRotation(pos, rot);
        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        poolQueue.Enqueue(bullet);
    }
}