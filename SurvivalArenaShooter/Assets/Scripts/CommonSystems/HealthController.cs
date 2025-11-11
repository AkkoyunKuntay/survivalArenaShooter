using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthController : MonoBehaviour,IDamageable
{
    [Header("References")]
    
    [Header("Health Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float defaultHealth;

    //Events
    public System.Action<float> OnHealthChangeEvent;
    public System.Action OnDeathEvent;
    
    private void Awake()
    {
        defaultHealth = health;
    }

    public void Initialize()
    {
        RestoreHealth();
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        OnHealthChangeEvent?.Invoke(health);
        if (health <= 0)
        {
            Dead();
        }
    }
    public void Dead()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        health = 0;
        //TO DO: Death particles
        yield return new WaitForSeconds(.5f);
        OnDeathEvent?.Invoke();
    }
    private void RestoreHealth()
    {
        health = defaultHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<EnemyBrain>(out EnemyBrain enemy))
        {
            TakeDamage(10);
            Debug.Log("Damage taken!");
        }
    }
}
