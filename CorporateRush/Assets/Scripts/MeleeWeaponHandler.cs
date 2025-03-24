using UnityEngine;
using System.Collections;

public class MeleeWeaponHandler : MonoBehaviour
{
    [Header("Melee Settings")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private LayerMask enemyLayer;
    private GameObject currentWeapon;
    private bool canAttack = true;

    public void EquipWeapon(GameObject weaponObject, GameObject weaponModel, AudioClip pickupSound)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weaponModel, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        Destroy(weaponObject);
    }

    public bool HasWeapon()
    {
        return currentWeapon != null;
    }

    public void PerformAttack()
    {
        if (!canAttack) return;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        Debug.Log("🗡️ Performing Melee Attack!");

        // 🔥 Play Melee Attack Animation
        if (animator != null)
        {
            animator.SetTrigger("MeleeAttack");
        }

        yield return new WaitForSeconds(0.2f);

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * attackRange * 0.5f, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
            if (enemyScript != null)
            {
                Debug.Log($"💥 Hit enemy: {enemy.name}, dealing {attackDamage} damage");
                enemyScript.TakeDamage(attackDamage);
            }
        }

        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }
}
