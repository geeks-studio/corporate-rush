using UnityEngine;
using System.Collections;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponHolder; // Assign a child object in front of the camera
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
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

        // Wait for animation time
        yield return new WaitForSeconds(0.5f); 

        canAttack = true;
    }
}