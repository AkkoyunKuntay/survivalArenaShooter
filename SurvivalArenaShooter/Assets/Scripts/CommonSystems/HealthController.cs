using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthController : MonoBehaviour,IDamageable
{
       
    [Header("Health Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float defaultHealth;

    [SerializeField] private bool isInvincible;
    //Events
    public System.Action OnHealthChangeEvent;
    public System.Action OnDeathEvent;
    
    private void Awake()
    {
        defaultHealth = health;
    }

    private void Start()
    {
        LevelManager.instance.TimerFinishedEvent += OnTimeFinished;
    }

    private void OnTimeFinished()
    {
        SetInvincible(true);
    }
    public void Initialize()
    {
        RestoreHealth();
    }
    public void TakeDamage(float damage)
    {
        if(isInvincible) return;
        
        health -= damage;
        OnHealthChangeEvent?.Invoke();
        if (health <= 0)
        {
            Dead();
        }
    }
    public void Dead()
    {
        StartCoroutine(DeathRoutine());
    }

    public void SetHealth(float newHealth)
    {
        defaultHealth = newHealth;
        health = newHealth;
    }
    private void SetInvincible(bool value)
    {
        isInvincible = value;
    }
    private IEnumerator DeathRoutine()
    {
        health = 0;
        
        yield return new WaitForSeconds(.1f);
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
            TakeDamage(enemy.damage);
            Debug.Log("Damage taken!");
        }
    }
}
