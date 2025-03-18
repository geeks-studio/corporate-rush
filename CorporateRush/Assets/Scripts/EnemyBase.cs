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

    [Header("References")]
    [SerializeField] protected NavMeshAgent agent;
    protected Transform player;

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
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            agent.SetDestination(player.position);
            RotateTowardsPlayer(); // 🔥 Now enemy rotates while following

            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        // Get direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Only rotate on the Y-axis (prevent tilting)
        directionToPlayer.y = 0;

        // Smoothly rotate towards player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
