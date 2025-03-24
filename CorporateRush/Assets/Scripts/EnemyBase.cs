using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected float detectionRadius = 10f;
    [SerializeField] protected int attackDamage = 20;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // List of patrol points
    [SerializeField] private float patrolSpeed = 2f; // Speed when patrolling
    [SerializeField] private float chaseSpeed = 5f; // Speed when chasing
    [SerializeField] private float waitTimeAtPoint = 2f; // Wait before moving to next point
    [SerializeField] private float patrolPointThreshold = 1.0f; // 🔥 Fixed: Distance to switch patrol points
    private int currentPatrolIndex = 0;
    private bool isPatrolling = true;

    [Header("Movement Settings")]
    [SerializeField] protected float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Animator animator;
    protected Transform player;

    [Header("Hit Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private Transform hitEffectPoint;

    protected float currentHealth;
    protected bool isAttacking = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // 🔥 Auto-Find the Player at Runtime
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("❌ No GameObject found with tag 'Player'. Make sure your Player is tagged correctly!");
        }

        // 🔥 Set stopping distance to prevent enemies from standing inside the player
        if (agent != null)
        {
            agent.stoppingDistance = attackRange - 0.5f;
            agent.speed = patrolSpeed; // Start in patrol mode
        }

        if (patrolPoints.Length > 0)
        {
            StartCoroutine(Patrol());
        }
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float speed = agent.velocity.magnitude;

        // 🔥 If player detected, stop patrolling and chase
        if (distanceToPlayer <= detectionRadius)
        {
            isPatrolling = false;
            StopCoroutine(Patrol()); // Stop patrol behavior
            agent.speed = chaseSpeed; // Switch to chase speed
            agent.SetDestination(player.position);

            // 🔥 Switch to running animation
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);

            // 🔥 If in attack range, stop and attack
            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                agent.ResetPath(); // Stop movement when attacking
                StartCoroutine(AttackPlayer());
            }
        }
        else if (!isPatrolling) // 🔥 Resume patrol when player is gone
        {
            isPatrolling = true;
            StartCoroutine(Patrol());
        }
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    protected virtual IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // 🔥 Play Attack Animation (Idle → Attack → Idle)
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Attack");

        Debug.Log("Set trigger attack");

        yield return new WaitForSeconds(attackCooldown);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        isAttacking = false;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Got damage = {damage}");

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, hitEffectPoint != null ? hitEffectPoint.position : transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} Died");
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator Patrol()
    {
        while (isPatrolling && patrolPoints.Length > 0)
        {
            // 🔥 Move to the next patrol point
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);

            // 🔥 Switch to walking animation
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);

            // 🔥 Wait until the enemy reaches the point (Added threshold fix)
            while (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) > patrolPointThreshold)
            {
                yield return null;
            }

            // 🔥 Stop moving and wait before next patrol
            agent.ResetPath(); // Stop at point
            animator.SetBool("IsWalking", false); // Switch to Idle
            yield return new WaitForSeconds(waitTimeAtPoint);

            // 🔥 Move to the next point in the array
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }
}
