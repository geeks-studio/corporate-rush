using UnityEngine;
using System.Collections;

public class RangedWeaponHandler : MonoBehaviour
{
    [Header("Ranged Settings")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunshotSound;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Animator animator;
    private GameObject currentWeapon;
    private bool canShoot = true;

    public void EquipWeapon(GameObject weaponObject, GameObject weaponModel, AudioClip pickupSound, GameObject bullet, Transform shootPos)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weaponModel, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        bulletPrefab = bullet;
        //shootPoint = shootPos;

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
    
    public void ShowWeapon(bool show)
    {
        if (currentWeapon != null)
            currentWeapon.SetActive(show);
    }



    public void Shoot()
    {
        if (!canShoot || shootPoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("❌ Cannot shoot: Missing weapon, shootPoint, or bulletPrefab!");
            return;
        }
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        canShoot = false;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        if (muzzleFlash != null) muzzleFlash.Play();
        if (gunshotSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }

        // 🔥 Spawn Bullet and Ensure it Moves
        if (shootPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript == null)
            {
                Debug.LogError("❌ Bullet prefab is missing 'Bullet' script!");
            }
        }
        else
        {
            Debug.LogError("❌ shootPoint was destroyed before firing the bullet!");
        }

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    public void Reload()
    {
        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }
    }
}
