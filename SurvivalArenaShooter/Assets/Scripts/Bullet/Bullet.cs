using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   [SerializeField] private EnemyBrain targetEnemy;
   private float _speed;
   private int _damage;
   private BulletPool pool;
   public void InitializeBullet(Vector3 position, Quaternion rotation,Transform target,float bulletSpeed,int bulletDamage,BulletPool bulletPool)
   {
      transform.position = position;  
      transform.rotation = rotation;
      targetEnemy = target.GetComponent<EnemyBrain>();
      
      _speed = bulletSpeed;
      _damage = bulletDamage;
      pool = bulletPool;
   }

   private void Update()
   {
     transform.position = Vector3.MoveTowards(transform.position,targetEnemy.transform.position,Time.deltaTime*_speed);
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.TryGetComponent<EnemyBrain>(out EnemyBrain enemy))
      {
        HealthController healthController = enemy.GetComponent<HealthController>();
        healthController.TakeDamage(_damage);
        pool.ReturnBullet(this.gameObject);
        Debug.Log("Bullet Hit");
      }
   }
}