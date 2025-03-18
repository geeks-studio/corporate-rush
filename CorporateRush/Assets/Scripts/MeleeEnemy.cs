using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    protected override IEnumerator AttackPlayer()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Melee Attack!");
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        isAttacking = false;
    }
}