using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour, IPoolable
{
    [Header("References")] 
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private HealthController healthController;

    private Transform target;
    private EnemyPool pool;

    [Header("AI Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackRange = 1.75f;
    [SerializeField] private float attackRate = 2f;

    private float nextAttackTime;
    private bool isInitialized;

    private enum EnemyState { Idle, Chase, Attack }
    private EnemyState currentState;

    public void OnSpawned(Transform targetTransform, EnemyPool originPool)
    {
        pool = originPool;
        target = targetTransform;

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
        EventBus<EnemyBrain>.Invoke(EventType.OnEnemySpawned, this);
    }

    public void OnDespawned()
    {
        agent.enabled = false;
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
        agent.isStopped = false;
        agent.SetDestination(target.position);
        animator.SetTrigger("isRunning");

        if (distance <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
    }

    private void HandleAttack(float distance)
    {
        agent.isStopped = true;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            animator.SetTrigger("Attack");
            // TODO: burada damage sistemi çalışır
        }

        if (distance > attackRange + 0.5f)
        {
            currentState = EnemyState.Chase;
        }
    }

    private void OnEnemyDeath()
    {
        pool.ReturnEnemy(gameObject);
    }
}
