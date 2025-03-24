using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    protected override IEnumerator AttackPlayer()
    {
        yield return base.AttackPlayer(); // 🔥 Calls the base attack logic (including animation)

        Debug.Log("Melee Attack!");

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}