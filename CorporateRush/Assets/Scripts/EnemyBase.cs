using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float attackRange = 2f; // Distance to attack
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected float detectionRadius = 10f;
    [SerializeField] protected int attackDamage = 20;

    [Header("Movement Settings")]
    [SerializeField] protected float rotationSpeed = 10f; // Customize enemy turn speed

    [Header("References")]
    [SerializeField] protected NavMeshAgent agent;
    protected Transform player;

    [Header("Hit Effects")]
    [SerializeField] private GameObject hitEffectPrefab; // Assign different effects for robots & humans
    [SerializeField] private Transform hitEffectPoint;  // Where the effect appears (assign in prefab)

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
            agent.stoppingDistance = attackRange - 0.5f; // Adjust based on enemy size
        }
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // 🔥 Move toward player but stop at attack range
            if (distanceToPlayer > agent.stoppingDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.ResetPath(); // Stop movement when in range
                RotateTowardsPlayer();
            }

            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Keep enemy upright

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    protected virtual IEnumerator AttackPlayer()
    {
        isAttacking = true;
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

        // 🔥 Show hit effect
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
}
