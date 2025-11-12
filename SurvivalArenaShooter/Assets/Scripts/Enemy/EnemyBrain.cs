using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyBrain : MonoBehaviour, IPoolable
{
    [Header("References")] 
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private HealthController healthController;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem hitParticles;
    
    private Transform target;
    private EnemyPool pool;

    [Header("AI Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackRange = 1.75f;
    [SerializeField] private float attackRate = 2f;
    public float damage;

    private float nextAttackTime;
    private bool isInitialized;

    private enum EnemyState { Idle, Chase, Attack }
    private EnemyState currentState;

    private float GetDamage()
    {
        damage = LevelManager.instance.RuntimeConfig.enemyDamage;
        return damage;
    }

    public void OnSpawned(Transform targetTransform, EnemyPool originPool)
    {
        pool = originPool;
        target = targetTransform;
        damage = GetDamage();
        if (!isInitialized)
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            healthController = GetComponent<HealthController>();
            
            healthController.OnDeathEvent += OnEnemyDeath;
            isInitialized = true;
        }

        healthController.Initialize();
        agent.enabled = true;
        agent.speed = moveSpeed;

        currentState = EnemyState.Chase;
        healthController.OnHealthChangeEvent += OnEnemyTakeDamage;
        EventBus<EnemyBrain>.Invoke(EventType.OnEnemySpawned, this);
    }

    public void OnDespawned()
    {
        
        EventBus<EnemyBrain>.Invoke(EventType.OnEnemyDespawned, this);
    }

    private void Update()
    {
        if (target == null || !agent.enabled) return;

        float distance = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case EnemyState.Chase:
                HandleChase(distance);
                break;
            case EnemyState.Attack:
                HandleAttack(distance);
                break;
        }
    }

    private void HandleChase(float distance)
    {
        if(!agent.enabled || !target) return;
        
        agent.SetDestination(target.position);
        animator.SetTrigger("isRunning");

        if (distance <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
    }

    private void HandleAttack(float distance)
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            animator.SetTrigger("Attack");
            //TODO: Animation Event Attack ? 
        }

        if (distance > attackRange + 0.5f)
        {
            currentState = EnemyState.Chase;
        }
    }

    private void OnEnemyTakeDamage()
    {
        hitParticles.Play();
    }
    private void OnEnemyDeath()
    {
        Stats.AddKill(1);
        pool.ReturnEnemy(gameObject);
    }
    

    
}
