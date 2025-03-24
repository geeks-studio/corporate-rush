using UnityEngine;
using System.Collections;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponHolder; // Assign a child object in front of the camera
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRange = 2f; // Damage range
    [SerializeField] private int attackDamage = 25; // Damage amount
    [SerializeField] private LayerMask enemyLayer; // Assign "Enemy" layer in Inspector
    private GameObject currentWeapon;
    
    private bool canAttack = true;
    
    public void EquipWeapon(GameObject weaponObject, GameObject weaponModel, AudioClip pickupSound)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon); // Remove existing weapon
        }

        // Attach weapon to player
        currentWeapon = Instantiate(weaponModel, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // Play sound effect
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // Destroy the pickup object
        Destroy(weaponObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentWeapon != null && canAttack) // Left-click to attack
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        animator.SetTrigger("Attack"); // Play attack animation

        yield return new WaitForSeconds(0.2f); // Small delay before applying damage

        // 🔥 Check for enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * attackRange * 0.5f, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(0.5f); // Cooldown before next attack

        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        // 🔥 Show attack range in Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange * 0.5f, attackRange);
    }
}
