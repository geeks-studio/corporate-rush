using System.Collections;
using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    protected override IEnumerator AttackPlayer()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Ranged Attack!");
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody>().linearVelocity = (player.position - firePoint.position).normalized * 10f;
        }

        isAttacking = false;
    }
}