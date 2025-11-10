using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemyPool enemyPool;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    [SerializeField] private HealthController healthController;

    [Header("AI Settings")] [SerializeField]
    private float moveSpeed = 3.5f;

    [SerializeField] private float desiredStopDistance = 1.75f;
    [SerializeField] private float attackRange = 1.75f;
    [SerializeField] private float attackRate = 3f;

    private float _nextAttackTime;
    private bool _isInitialized;
    
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }
    private EnemyState _currentState = EnemyState.Idle;

    public void InitializeEnemy(Transform targetTransform, EnemyPool pool)
    {
        enemyPool = pool;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        healthController = GetComponent<HealthController>();
        
        healthController.Initialize();
        healthController.OnDeathEvent += OnEnemyDeath;

        agent.speed = moveSpeed;
        agent.stoppingDistance = desiredStopDistance;

        target = targetTransform;
        transform.LookAt(target);

        ChangeState(EnemyState.Chase);
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized || target == null || !agent.enabled) return;
        
        float distance = Vector3.Distance(transform.position, target.position);

        switch (_currentState)
        {
            case EnemyState.Chase:
                HandleChase(distance);
                break;

            case EnemyState.Attack:
                HandleAttack(distance);
                break;
        }
    }

    #region AI State Logic

    private void HandleChase(float distance)
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
        animator.SetTrigger("isRunning");

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void HandleAttack(float distance)
    {
        agent.isStopped = true;

        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + attackRate;
            animator.SetTrigger("Attack");
            // TODO: Damage uygulama fonksiyonunu burada çağır
        }


        if (distance > attackRange + 0.25f)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    private void ChangeState(EnemyState newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case EnemyState.Chase:
                agent.isStopped = false;
                animator.ResetTrigger("Attack");
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                break;
        }
    }

    #endregion

    #region Subscribed Events Logic

    private void OnEnemyDeath()
    {
        enemyPool.ReturnEnemy(gameObject);
    }

    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            //Enemy Health Controller Test
            healthController.TakeDamage(10);
        }
    }

   
}

